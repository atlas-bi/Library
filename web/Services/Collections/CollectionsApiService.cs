using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Collections;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface ICollectionsApiService
{
    Task<CollectionListResponseDto> GetCollectionsAsync(
        ClaimsPrincipal user,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<CollectionDetailDto> GetCollectionAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<CollectionDetailDto> CreateCollectionAsync(
        ClaimsPrincipal user,
        CreateCollectionRequestDto request,
        CancellationToken cancellationToken
    );
    Task<CollectionDetailDto> UpdateCollectionAsync(
        ClaimsPrincipal user,
        int id,
        UpdateCollectionRequestDto request,
        CancellationToken cancellationToken
    );
    Task<bool> DeleteCollectionAsync(
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<CollectionSearchResultDto>> SearchTermsAsync(
        string search,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<CollectionSearchResultDto>> SearchReportsAsync(
        string search,
        CancellationToken cancellationToken
    );
}

public sealed class CollectionsApiService : ICollectionsApiService
{
    private const int MaxPageSize = 100;
    private readonly Atlas_WebContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public CollectionsApiService(
        Atlas_WebContext context,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        IMemoryCache cache
    )
    {
        _context = context;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<CollectionListResponseDto> GetCollectionsAsync(
        ClaimsPrincipal user,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        var query = _context.Collections.AsNoTracking();

        var total = await query.CountAsync(cancellationToken);
        var collections = await query
            .OrderBy(x => x.Name)
            .Select(x => new CollectionListItemDto
            {
                Id = x.CollectionId,
                Name = x.Name,
                Description = x.Description,
                Purpose = x.Purpose,
                Hidden = x.Hidden,
                LastModified = x.LastUpdateDate,
                StarCount = x.StarredCollections.Count,
                IsStarred = x.StarredCollections.Any(y => y.Ownerid == currentUserId),
            })
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return new CollectionListResponseDto
        {
            Collections = collections,
            Total = total,
            Page = safePage,
            PageSize = safePageSize,
        };
    }

    public async Task<CollectionDetailDto> GetCollectionAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var features = BuildFeatureFlags();
        var canViewUserProfiles = user.HasPermission("View Other User")
            && features.UserProfilesEnabled;
        var currentUserId = user.GetUserId();

        var collection = await _context
            .Collections.AsNoTracking()
            .Where(x => x.CollectionId == id)
            .Select(x => new CollectionDetailDto
            {
                Id = x.CollectionId,
                Name = x.Name,
                Description = x.Description,
                Purpose = x.Purpose,
                Hidden = x.Hidden,
                LastModified = x.LastUpdateDate,
                LastModifiedDisplay = ModelHelpers.RelativeDate(x.LastUpdateDate),
                IsStarred = x.StarredCollections.Any(y => y.Ownerid == currentUserId),
                StarCount = x.StarredCollections.Count,
                CanCreateCollection = user.HasPermission("Create Collection"),
                CanEditCollection = user.HasPermission("Edit Collection"),
                CanDeleteCollection = user.HasPermission("Delete Collection"),
                CanViewUserProfiles = canViewUserProfiles,
                Features = features,
                LastUpdatedBy = x.LastUpdateUserNavigation == null
                    ? null
                    : new CollectionUserSummaryDto
                    {
                        Id = x.LastUpdateUserNavigation.UserId,
                        Username = x.LastUpdateUserNavigation.Username,
                        FullName = x.LastUpdateUserNavigation.FullnameCalc
                            ?? x.LastUpdateUserNavigation.DisplayName,
                        Email = x.LastUpdateUserNavigation.Email,
                    },
                Initiative = x.Initiative == null
                    ? null
                    : new InitiativeSummaryDto
                    {
                        Id = x.Initiative.InitiativeId,
                        Name = x.Initiative.Name,
                        Description = x.Initiative.Description,
                    },
                Terms = features.TermsEnabled
                    ? x.CollectionTerms.OrderBy(y => y.Rank).ThenBy(y => y.Term.Name)
                        .Select(y => new CollectionTermDto
                        {
                            Id = y.TermId,
                            Name = y.Term.Name,
                            Summary = y.Term.Summary,
                            Rank = y.Rank,
                        })
                        .ToList()
                    : new List<CollectionTermDto>(),
                Reports = x.CollectionReports.OrderBy(y => y.Rank).ThenBy(y => y.Report.Name)
                    .Select(y => new CollectionReportDto
                    {
                        Id = y.ReportId,
                        Name = y.Report.DisplayTitle ?? y.Report.Name,
                        Description = y.Report.Description,
                        Type = y.Report.ReportObjectType != null
                            ? y.Report.ReportObjectType.ShortName
                            : null,
                        Url = y.Report.ReportObjectUrl,
                        LastModified = y.Report.LastModifiedDate,
                        AttachmentCount = y.Report.ReportObjectAttachments.Count,
                        Rank = y.Rank,
                        IsStarred = y.Report.StarredReports.Any(z => z.Ownerid == currentUserId),
                    })
                    .ToList(),
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (collection == null)
        {
            return null;
        }

        await PopulateRunAuthorizationAsync(user, collection.Reports, cancellationToken);
        return collection;
    }

    public async Task<CollectionDetailDto> CreateCollectionAsync(
        ClaimsPrincipal user,
        CreateCollectionRequestDto request,
        CancellationToken cancellationToken
    )
    {
        await ValidateLinkedIdsAsync(request.TermIds, request.ReportIds, cancellationToken);

        var collection = new Collection
        {
            Name = request.Name,
            Description = request.Description,
            Purpose = request.Purpose,
            Hidden = NormalizeFlag(request.Hidden),
            LastUpdateUser = user.GetUserId(),
            LastUpdateDate = DateTime.Now,
        };

        await _context.Collections.AddAsync(collection, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await SynchronizeTermsAsync(collection.CollectionId, request.TermIds, cancellationToken);
        await SynchronizeReportsAsync(collection.CollectionId, request.ReportIds, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateCollectionCaches(collection.CollectionId, request.TermIds, request.ReportIds);

        return await GetCollectionAsync(user, collection.CollectionId, cancellationToken);
    }

    public async Task<CollectionDetailDto> UpdateCollectionAsync(
        ClaimsPrincipal user,
        int id,
        UpdateCollectionRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var collection = await _context.Collections.SingleOrDefaultAsync(
            x => x.CollectionId == id,
            cancellationToken
        );
        if (collection == null)
        {
            return null;
        }

        var existingTermIds = await _context.CollectionTerms.Where(x => x.CollectionId == id)
            .Select(x => x.TermId)
            .ToListAsync(cancellationToken);
        var existingReportIds = await _context.CollectionReports.Where(x => x.CollectionId == id)
            .Select(x => x.ReportId)
            .ToListAsync(cancellationToken);

        await ValidateLinkedIdsAsync(request.TermIds, request.ReportIds, cancellationToken);

        collection.Name = request.Name;
        collection.Description = request.Description;
        collection.Purpose = request.Purpose;
        collection.Hidden = NormalizeFlag(request.Hidden);
        collection.LastUpdateUser = user.GetUserId();
        collection.LastUpdateDate = DateTime.Now;

        await SynchronizeTermsAsync(collection.CollectionId, request.TermIds, cancellationToken);
        await SynchronizeReportsAsync(collection.CollectionId, request.ReportIds, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateCollectionCaches(
            collection.CollectionId,
            existingTermIds.Union(request.TermIds).ToArray(),
            existingReportIds.Union(request.ReportIds).ToArray()
        );

        return await GetCollectionAsync(user, collection.CollectionId, cancellationToken);
    }

    public async Task<bool> DeleteCollectionAsync(int id, CancellationToken cancellationToken)
    {
        var relatedTermIds = await _context.CollectionTerms.Where(x => x.CollectionId == id)
            .Select(x => x.TermId)
            .ToListAsync(cancellationToken);
        var relatedReportIds = await _context.CollectionReports.Where(x => x.CollectionId == id)
            .Select(x => x.ReportId)
            .ToListAsync(cancellationToken);
        var collection = await _context.Collections.SingleOrDefaultAsync(
            x => x.CollectionId == id,
            cancellationToken
        );
        if (collection == null)
        {
            return false;
        }

        var collectionReports = await _context.CollectionReports.Where(x => x.CollectionId == id)
            .ToListAsync(cancellationToken);
        var collectionTerms = await _context.CollectionTerms.Where(x => x.CollectionId == id)
            .ToListAsync(cancellationToken);

        _context.CollectionReports.RemoveRange(collectionReports);
        _context.CollectionTerms.RemoveRange(collectionTerms);
        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateCollectionCaches(id, relatedTermIds, relatedReportIds);

        return true;
    }

    public async Task<IReadOnlyList<CollectionSearchResultDto>> SearchTermsAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Array.Empty<CollectionSearchResultDto>();
        }

        var termSearch = search.Trim();
        return await _context.Terms.AsNoTracking()
            .Where(x => x.Name.Contains(termSearch) || x.Summary.Contains(termSearch))
            .OrderBy(x => x.Name)
            .Select(x => new CollectionSearchResultDto
            {
                Id = x.TermId,
                Name = x.Name,
                Description = x.Summary,
            })
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CollectionSearchResultDto>> SearchReportsAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Array.Empty<CollectionSearchResultDto>();
        }

        var reportSearch = search.Trim();
        return await _context.ReportObjects.AsNoTracking()
            .Where(x =>
                (x.DisplayTitle ?? x.Name).Contains(reportSearch)
                || x.Description.Contains(reportSearch)
            )
            .OrderBy(x => x.DisplayTitle ?? x.Name)
            .Select(x => new CollectionSearchResultDto
            {
                Id = x.ReportObjectId,
                Name = x.DisplayTitle ?? x.Name,
                Description = x.Description,
            })
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    private CollectionFeatureFlagsDto BuildFeatureFlags()
    {
        return new CollectionFeatureFlagsDto
        {
            TermsEnabled = IsFeatureEnabled("features:enable_terms"),
            UserProfilesEnabled = IsFeatureEnabled("features:enable_user_profile"),
            FeedbackEnabled = IsFeatureEnabled("features:enable_feedback"),
            SharingEnabled = IsFeatureEnabled("features:enable_sharing"),
        };
    }

    private bool IsFeatureEnabled(string key)
    {
        var value = _configuration[key];
        return string.IsNullOrWhiteSpace(value)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private async Task PopulateRunAuthorizationAsync(
        ClaimsPrincipal user,
        IReadOnlyList<CollectionReportDto> reports,
        CancellationToken cancellationToken
    )
    {
        if (reports.Count == 0)
        {
            return;
        }

        var reportIds = reports.Select(x => x.Id).ToArray();
        var authorizationReports = await _context.ReportObjects.AsNoTracking()
            .Where(x => reportIds.Contains(x.ReportObjectId))
            .Include(x => x.ReportObjectType)
            .Include(x => x.ReportGroupsMemberships)
            .Include(x => x.ReportObjectHierarchyChildReportObjects)
                .ThenInclude(x => x.ParentReportObject)
                    .ThenInclude(x => x.ReportGroupsMemberships)
            .ToListAsync(cancellationToken);
        var authorizationLookup = authorizationReports.ToDictionary(x => x.ReportObjectId);

        foreach (var report in reports)
        {
            report.CanRun =
                authorizationLookup.TryGetValue(report.Id, out var authorizationReport)
                && (
                    await _authorizationService.AuthorizeAsync(
                        user,
                        authorizationReport,
                        "ReportRunPolicy"
                    )
                ).Succeeded;
        }
    }

    private async Task SynchronizeTermsAsync(
        int collectionId,
        IReadOnlyList<int> termIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = termIds.Distinct().ToList();
        var existing = await _context.CollectionTerms.Where(x => x.CollectionId == collectionId)
            .ToListAsync(cancellationToken);

        _context.CollectionTerms.RemoveRange(existing.Where(x => !normalizedIds.Contains(x.TermId)));

        for (var index = 0; index < normalizedIds.Count; index++)
        {
            var termId = normalizedIds[index];
            var existingLink = existing.FirstOrDefault(x => x.TermId == termId);
            if (existingLink == null)
            {
                await _context.CollectionTerms.AddAsync(
                    new CollectionTerm
                    {
                        CollectionId = collectionId,
                        TermId = termId,
                        Rank = index,
                    },
                    cancellationToken
                );
            }
            else
            {
                existingLink.Rank = index;
            }
        }
    }

    private async Task SynchronizeReportsAsync(
        int collectionId,
        IReadOnlyList<int> reportIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = reportIds.Distinct().ToList();
        var existing = await _context.CollectionReports.Where(x => x.CollectionId == collectionId)
            .ToListAsync(cancellationToken);

        _context.CollectionReports.RemoveRange(
            existing.Where(x => !normalizedIds.Contains(x.ReportId))
        );

        for (var index = 0; index < normalizedIds.Count; index++)
        {
            var reportId = normalizedIds[index];
            var existingLink = existing.FirstOrDefault(x => x.ReportId == reportId);
            if (existingLink == null)
            {
                await _context.CollectionReports.AddAsync(
                    new CollectionReport
                    {
                        CollectionId = collectionId,
                        ReportId = reportId,
                        Rank = index,
                    },
                    cancellationToken
                );
            }
            else
            {
                existingLink.Rank = index;
            }
        }
    }

    private static string NormalizeFlag(string value)
    {
        return string.Equals(value, "Y", StringComparison.OrdinalIgnoreCase) ? "Y" : "N";
    }

    private async Task ValidateLinkedIdsAsync(
        IReadOnlyList<int> termIds,
        IReadOnlyList<int> reportIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedTermIds = termIds.Distinct().ToArray();
        if (normalizedTermIds.Length > 0)
        {
            var existingTermIds = await _context.Terms.AsNoTracking()
                .Where(x => normalizedTermIds.Contains(x.TermId))
                .Select(x => x.TermId)
                .ToListAsync(cancellationToken);
            var missingTermIds = normalizedTermIds.Except(existingTermIds).ToArray();
            if (missingTermIds.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Unknown term ids: {string.Join(", ", missingTermIds)}"
                );
            }
        }

        var normalizedReportIds = reportIds.Distinct().ToArray();
        if (normalizedReportIds.Length > 0)
        {
            var existingReportIds = await _context.ReportObjects.AsNoTracking()
                .Where(x => normalizedReportIds.Contains(x.ReportObjectId))
                .Select(x => x.ReportObjectId)
                .ToListAsync(cancellationToken);
            var missingReportIds = normalizedReportIds.Except(existingReportIds).ToArray();
            if (missingReportIds.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Unknown report ids: {string.Join(", ", missingReportIds)}"
                );
            }
        }
    }

    private void InvalidateCollectionCaches(
        int collectionId,
        IEnumerable<int> termIds,
        IEnumerable<int> reportIds
    )
    {
        _cache.Remove("collections");
        _cache.Remove("collection-" + collectionId);
        _cache.Remove("search-collection-" + collectionId);
        _cache.Remove("terms");

        foreach (var termId in termIds.Distinct())
        {
            _cache.Remove("term-" + termId);
        }

        foreach (var reportId in reportIds.Distinct())
        {
            _cache.Remove("report-" + reportId);
            _cache.Remove("report-terms-" + reportId);
            _cache.Remove("report-comp-queries-" + reportId);
            _cache.Remove("report-children-" + reportId);
            _cache.Remove("report-parents-" + reportId);
            _cache.Remove("search-report-" + reportId);
        }
    }
}
