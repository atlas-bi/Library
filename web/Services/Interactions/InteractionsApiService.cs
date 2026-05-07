using System.Text.Json;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Interactions;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Atlas_Web.Pages.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface IInteractionsApiService
{
    Task<ToggleStarResponseDto> ToggleStarAsync(
        ClaimsPrincipal user,
        ToggleStarRequestDto request,
        CancellationToken cancellationToken
    );
    Task<ShareMailResponseDto> SendShareMailAsync(
        ClaimsPrincipal user,
        ShareMailRequestDto request,
        CancellationToken cancellationToken
    );
    Task<JsonElement> SendFeedbackAsync(
        ClaimsPrincipal user,
        ShareFeedbackRequestDto request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<RecipientSearchResultDto>> SearchRecipientsAsync(
        string search,
        bool includeGroups,
        CancellationToken cancellationToken
    );
}

public sealed class InteractionsApiService : IInteractionsApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _config;
    private readonly IRazorPartialToStringRenderer _renderer;
    private readonly IEmailService _emailer;
    private readonly IMemoryCache _cache;
    private readonly ISolrReadOnlyOperations<SolrAtlas> _solr;

    public InteractionsApiService(
        Atlas_WebContext context,
        IConfiguration config,
        IRazorPartialToStringRenderer renderer,
        IEmailService emailer,
        IMemoryCache cache,
        ISolrReadOnlyOperations<SolrAtlas> solr
    )
    {
        _context = context;
        _config = config;
        _renderer = renderer;
        _emailer = emailer;
        _cache = cache;
        _solr = solr;
    }

    public async Task<ToggleStarResponseDto> ToggleStarAsync(
        ClaimsPrincipal user,
        ToggleStarRequestDto request,
        CancellationToken cancellationToken
    )
    {
        if (request == null || request.Id <= 0 || string.IsNullOrWhiteSpace(request.Type))
        {
            throw new InvalidOperationException("A valid star target is required.");
        }

        var userId = user.GetUserId();
        var type = request.Type.Trim().ToLowerInvariant();

        return type switch
        {
            "report" => await ToggleReportStarAsync(userId, request.Id, cancellationToken),
            "collection" => await ToggleCollectionStarAsync(userId, request.Id, cancellationToken),
            _ => throw new InvalidOperationException("Unsupported star target type."),
        };
    }

    public async Task<ShareMailResponseDto> SendShareMailAsync(
        ClaimsPrincipal user,
        ShareMailRequestDto request,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            throw new InvalidOperationException("Request body is required.");
        }

        var recipients = request.To.Where(x => x != null && x.UserId > 0).ToList();
        if (recipients.Count == 0)
        {
            throw new InvalidOperationException("No recipients specified.");
        }

        var userIds = recipients
            .Where(x => !string.Equals(x.Type, "g", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.UserId)
            .Distinct()
            .ToList();
        var groupIds = recipients
            .Where(x => string.Equals(x.Type, "g", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        var directUsers = await _context.Users.Where(x => userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);
        var groupUsers = await _context.UserGroupsMemberships.Include(x => x.User)
            .Where(x => groupIds.Contains(x.GroupId))
            .ToListAsync(cancellationToken);

        if (directUsers.Count == 0 && groupUsers.Count == 0)
        {
            throw new InvalidOperationException("No recipients specified.");
        }

        var message = new MailMessage
        {
            Subject = request.Subject ?? string.Empty,
            Message = request.Message ?? string.Empty,
            MessagePlainText = request.Text ?? string.Empty,
            SendDate = DateTime.Now,
            FromUserId = user.GetUserId(),
        };

        await _context.MailMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.MailRecipients.AddRangeAsync(
            directUsers.Select(x => new MailRecipient
            {
                MessageId = message.MessageId,
                ToUserId = x.UserId,
            }),
            cancellationToken
        );
        await _context.MailRecipients.AddRangeAsync(
            groupUsers.Select(x => new MailRecipient
            {
                MessageId = message.MessageId,
                ToUserId = x.UserId,
                ToGroupId = x.GroupId,
            }),
            cancellationToken
        );

        if (request.DraftId.GetValueOrDefault() >= 0)
        {
            _context.RemoveRange(_context.MailDrafts.Where(x => x.DraftId == request.DraftId.Value));
        }

        await _context.SaveChangesAsync(cancellationToken);

        var shareCount = 0;
        if (request.Share)
        {
            shareCount = await CreateSharesAsync(
                user.GetUserId(),
                request,
                directUsers,
                groupUsers,
                cancellationToken
            );
        }

        return new ShareMailResponseDto
        {
            Message = "Successfully shared.",
            RecipientCount = directUsers.Count + groupUsers.Count,
            ShareCount = shareCount,
        };
    }

    public async Task<JsonElement> SendFeedbackAsync(
        ClaimsPrincipal user,
        ShareFeedbackRequestDto request,
        CancellationToken cancellationToken
    )
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ReportName))
        {
            throw new InvalidOperationException("Feedback target is required.");
        }

        using var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };
        using var client = new HttpClient(handler)
        {
            DefaultRequestVersion = new Version(1, 1),
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
        };

        client.DefaultRequestHeaders.Add("Accept", "application/vnd.manageengine.sdp.v3+json");
        client.DefaultRequestHeaders.Add(
            "authtoken",
            _config["AppSettings:manage_engine_tech_key"]
        );

        var payload = new
        {
            request = new
            {
                subject = "Atlas Feedback",
                description =
                    $"<b>Atlas feedback on <a href='{request.ReportUrl}'>{request.ReportName}</a></b><br/><br/><p>{request.Description}</p>",
                requester = BuildRequester(user),
                template = new { name = "WebAPI" },
                status = new { name = "Open" },
                category = new { name = "Epic Request" },
                subcategory = new { name = "Atlas" },
                item = new { name = "Feedback" },
                udf_fields = new
                {
                    udf_sline_5791 = request.ReportName,
                    udf_sline_5790 = request.ReportUrl,
                },
            },
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "input_data", json } }
        );

        var url = _config["AppSettings:manage_engine_server"] + "/api/v3/requests";
        using var response = await client.PostAsync(url, content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(responseBody).RootElement.Clone();
    }

    public Task<IReadOnlyList<RecipientSearchResultDto>> SearchRecipientsAsync(
        string search,
        bool includeGroups,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryString = IndexModel.BuildSearchString(
            search ?? string.Empty,
            new QueryCollection()
        );
        var results = QueryRecipients(queryString, "/users", "u");
        if (includeGroups)
        {
            results.AddRange(QueryRecipients(queryString, "/groups", "g"));
        }

        return Task.FromResult<IReadOnlyList<RecipientSearchResultDto>>(
            results.GroupBy(x => new { x.Type, x.Id }).Select(x => x.First()).ToList()
        );
    }

    private async Task<ToggleStarResponseDto> ToggleReportStarAsync(
        int userId,
        int reportId,
        CancellationToken cancellationToken
    )
    {
        if (
            !await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == reportId, cancellationToken)
        )
        {
            return null;
        }

        var existing = await _context.StarredReports.Where(x =>
                x.Ownerid == userId && x.Reportid == reportId
            )
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;

        if (isStarred)
        {
            await _context.StarredReports.AddAsync(
                new StarredReport { Ownerid = userId, Reportid = reportId },
                cancellationToken
            );
        }
        else
        {
            _context.StarredReports.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove($"report-{reportId}");

        return new ToggleStarResponseDto
        {
            Type = "report",
            Id = reportId,
            IsStarred = isStarred,
            Count = await _context.StarredReports.CountAsync(
                x => x.Reportid == reportId,
                cancellationToken
            ),
        };
    }

    private async Task<ToggleStarResponseDto> ToggleCollectionStarAsync(
        int userId,
        int collectionId,
        CancellationToken cancellationToken
    )
    {
        if (
            !await _context.Collections.AnyAsync(x => x.CollectionId == collectionId, cancellationToken)
        )
        {
            return null;
        }

        var existing = await _context.StarredCollections.Where(x =>
                x.Ownerid == userId && x.Collectionid == collectionId
            )
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;

        if (isStarred)
        {
            await _context.StarredCollections.AddAsync(
                new StarredCollection { Ownerid = userId, Collectionid = collectionId },
                cancellationToken
            );
        }
        else
        {
            _context.StarredCollections.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove($"collection-{collectionId}");
        _cache.Remove("collections");

        return new ToggleStarResponseDto
        {
            Type = "collection",
            Id = collectionId,
            IsStarred = isStarred,
            Count = await _context.StarredCollections.CountAsync(
                x => x.Collectionid == collectionId,
                cancellationToken
            ),
        };
    }

    private async Task<int> CreateSharesAsync(
        int senderUserId,
        ShareMailRequestDto request,
        IReadOnlyList<User> directUsers,
        IReadOnlyList<UserGroupsMembership> groupUsers,
        CancellationToken cancellationToken
    )
    {
        var sender = await _context.Users.SingleAsync(x => x.UserId == senderUserId, cancellationToken);
        var shareCount = 0;

        foreach (var recipient in directUsers)
        {
            await CreateShareAsync(sender, recipient, request, cancellationToken);
            shareCount++;
        }

        foreach (var groupRecipient in groupUsers)
        {
            if (groupRecipient.User == null)
            {
                continue;
            }

            await CreateShareAsync(sender, groupRecipient.User, request, cancellationToken);
            shareCount++;
        }

        return shareCount;
    }

    private async Task CreateShareAsync(
        User sender,
        User recipient,
        ShareMailRequestDto request,
        CancellationToken cancellationToken
    )
    {
        await _context.SharedItems.AddAsync(
            new SharedItem
            {
                SharedFromUserId = sender.UserId,
                SharedToUserId = recipient.UserId,
                ShareDate = DateTime.Now,
                Name = request.ShareName,
                Url = request.ShareUrl,
            },
            cancellationToken
        );
        await _context.SaveChangesAsync(cancellationToken);

        var setting = await _context.UserSettings.Where(x =>
                x.Name == "share_notification" && x.UserId == recipient.UserId
            )
            .Select(x => x.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(recipient.Email) || setting == "N")
        {
            return;
        }

        var viewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary()
        )
        {
            ["Subject"] = $"New share from {sender.FullnameCalc}",
            ["Body"] = HtmlHelpers.MarkdownToHtml(request.Message ?? string.Empty, _config),
            ["Sender"] = sender,
            ["Receiver"] = recipient,
        };

        var body = await _renderer.RenderPartialToStringAsync("_EmailTemplate", viewData);
        await _emailer.SendAsync(
            $"New share from {sender.FullnameCalc}",
            HtmlHelpers.MinifyHtml(body),
            sender.Email,
            recipient.Email
        );
    }

    private object BuildRequester(ClaimsPrincipal user)
    {
        var email = user.GetUserEmail();
        var name = user.GetUserName();

        if (string.IsNullOrWhiteSpace(email))
        {
            return new { name };
        }

        return new { email_id = email };
    }

    private List<RecipientSearchResultDto> QueryRecipients(
        string queryString,
        string handler,
        string type
    )
    {
        return _solr
            .Query(
                new SolrQuery(queryString),
                new QueryOptions
                {
                    RequestHandler = new RequestHandlerParameters(handler),
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 20,
                }
            )
            .Select(x => new RecipientSearchResultDto
            {
                Id = x.AtlasId,
                Name = x.Name,
                Type = type,
                Email = x.Email,
            })
            .ToList();
    }
}
