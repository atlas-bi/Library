using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Reports;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Atlas_Web.Pages.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface IReportsApiService
{
    Task<ReportListResponseDto> GetReportsAsync(
        ClaimsPrincipal user,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<ReportDetailDto> GetReportAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TermSummaryDto>> GetReportTermsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<ReportQueriesResponseDto> GetReportQueriesAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<ReportRelationshipsResponseDto> GetReportRelationshipsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<ReportMaintenanceStatusDto> GetReportMaintenanceStatusAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<ReportDetailDto> UpdateReportAsync(
        ClaimsPrincipal user,
        int id,
        UpdateReportDocumentRequestDto request,
        CancellationToken cancellationToken
    );
    Task<ReportImageDto> AddImageAsync(
        ClaimsPrincipal user,
        int id,
        IFormFile file,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<LookupDto>> GetLookupValuesAsync(
        string lookupArea,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ReportSearchResultDto>> SearchTermsAsync(
        string search,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ReportSearchResultDto>> SearchCollectionsAsync(
        string search,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ReportSearchResultDto>> SearchUsersAsync(
        string search,
        CancellationToken cancellationToken
    );
}

public sealed partial class ReportsApiService : IReportsApiService
{
    private const int MaxPageSize = 100;
    private readonly IAuthorizationService _authorizationService;
    private readonly Atlas_WebContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ISolrReadOnlyOperations<SolrAtlas> _solr;
    private readonly ISolrReadOnlyOperations<SolrAtlasLookups> _solrLookup;

    public ReportsApiService(
        Atlas_WebContext context,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ISolrReadOnlyOperations<SolrAtlas> solr,
        ISolrReadOnlyOperations<SolrAtlasLookups> solrLookup
    )
    {
        _context = context;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _solr = solr;
        _solrLookup = solrLookup;
    }

    public async Task<ReportListResponseDto> GetReportsAsync(
        ClaimsPrincipal user,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = _context
            .ReportObjects.AsNoTracking()
            .Where(x => x.DefaultVisibilityYn == "Y")
            .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N");

        var total = await query.CountAsync(cancellationToken);
        var reports = await query
            .OrderBy(x => x.DisplayTitle ?? x.Name)
            .Select(x => new ReportListItemDto
            {
                Id = x.ReportObjectId,
                Name = x.DisplayTitle ?? x.Name,
                Description = x.Description,
                Type = x.ReportObjectType != null ? x.ReportObjectType.ShortName : null,
                Url = x.ReportObjectUrl,
                LastModified = x.LastModifiedDate,
            })
            .Skip((safePage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        await PopulateRunAuthorizationAsync(user, reports, cancellationToken);

        return new ReportListResponseDto
        {
            Reports = reports,
            Total = total,
            Page = safePage,
            PageSize = safePageSize,
        };
    }

    public async Task<ReportDetailDto> GetReportAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        return await GetReportCoreAsync(user, id, cancellationToken, visibleOnly: true);
    }

    public async Task<IReadOnlyList<TermSummaryDto>> GetReportTermsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var exists = await ReportExistsAsync(id, cancellationToken);
        if (!exists || !IsFeatureEnabled("features:enable_terms"))
        {
            return Array.Empty<TermSummaryDto>();
        }

        return await GetTermsAsync(id, cancellationToken);
    }

    public async Task<ReportQueriesResponseDto> GetReportQueriesAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!await ReportExistsAsync(id, cancellationToken))
        {
            return null;
        }

        var detail = await GetReportCoreAsync(user, id, cancellationToken, visibleOnly: true);
        if (detail == null)
        {
            return null;
        }

        return new ReportQueriesResponseDto
        {
            Queries = detail.Queries,
            ComponentQueries = detail.ComponentQueries,
        };
    }

    public async Task<ReportRelationshipsResponseDto> GetReportRelationshipsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!await ReportExistsAsync(id, cancellationToken))
        {
            return null;
        }

        var detail = await GetReportCoreAsync(user, id, cancellationToken, visibleOnly: true);
        if (detail == null)
        {
            return null;
        }

        return new ReportRelationshipsResponseDto
        {
            CanViewGroups = detail.CanViewGroups,
            Groups = detail.Groups,
            Collections = detail.Collections,
            Children = detail.Children,
            Parents = detail.Parents,
        };
    }

    public async Task<ReportMaintenanceStatusDto> GetReportMaintenanceStatusAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!await ReportExistsAsync(id, cancellationToken))
        {
            return null;
        }

        var detail = await GetReportCoreAsync(user, id, cancellationToken, visibleOnly: true);
        return detail?.MaintenanceStatus;
    }

    public async Task<ReportDetailDto> UpdateReportAsync(
        ClaimsPrincipal user,
        int id,
        UpdateReportDocumentRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var reportExists = await _context.ReportObjects.AnyAsync(
            x => x.ReportObjectId == id,
            cancellationToken
        );
        if (!reportExists)
        {
            return null;
        }

        var existingDocument = await _context.ReportObjectDocs.SingleOrDefaultAsync(
            x => x.ReportObjectId == id,
            cancellationToken
        );

        if (existingDocument == null)
        {
            existingDocument = new ReportObjectDoc
            {
                ReportObjectId = id,
                CreatedDateTime = DateTime.UtcNow,
                CreatedBy = user.GetUserId(),
            };
            await _context.ReportObjectDocs.AddAsync(existingDocument, cancellationToken);
        }

        existingDocument.GitLabProjectUrl = request.GitLabProjectUrl;
        existingDocument.DeveloperDescription = request.DeveloperDescription;
        existingDocument.KeyAssumptions = request.KeyAssumptions;
        existingDocument.OperationalOwnerUserId = request.OperationalOwnerUserId;
        existingDocument.Requester = request.RequesterUserId;
        existingDocument.OrganizationalValueId = request.OrganizationalValueId;
        existingDocument.EstimatedRunFrequencyId = request.EstimatedRunFrequencyId;
        existingDocument.FragilityId = request.FragilityId;
        existingDocument.ExecutiveVisibilityYn = request.ExecutiveVisibilityYn;
        existingDocument.MaintenanceScheduleId = request.MaintenanceScheduleId;
        existingDocument.EnabledForHyperspace = request.EnabledForHyperspace;
        existingDocument.DoNotPurge = request.DoNotPurge;
        existingDocument.Hidden = request.Hidden;
        existingDocument.DeveloperNotes = request.DeveloperNotes;
        existingDocument.LastUpdateDateTime = DateTime.UtcNow;
        existingDocument.UpdatedBy = user.GetUserId();

        await SynchronizeTermsAsync(id, request.TermIds, cancellationToken);
        await SynchronizeCollectionsAsync(id, request.CollectionIds, cancellationToken);
        await SynchronizeFragilityTagsAsync(id, request.FragilityTagIds, cancellationToken);
        await SynchronizeImagesAsync(id, request.ImageIds, cancellationToken);
        await SynchronizeServiceRequestsAsync(id, request.ServiceRequestIds, cancellationToken);
        await AddMaintenanceLogAsync(user, id, request.NewMaintenanceLog, cancellationToken);
        await AddServiceRequestAsync(id, request.NewServiceRequest, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetReportCoreAsync(user, id, cancellationToken, visibleOnly: false);
    }

    public async Task<ReportImageDto> AddImageAsync(
        ClaimsPrincipal user,
        int id,
        IFormFile file,
        CancellationToken cancellationToken
    )
    {
        if (!await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == id, cancellationToken))
        {
            return null;
        }

        ValidateImageUpload(file);

        var nextOrdinal =
            await _context.ReportObjectImagesDocs.Where(x => x.ReportObjectId == id)
                .MaxAsync(x => (int?)x.ImageOrdinal, cancellationToken) ?? -1;

        var image = new ReportObjectImagesDoc
        {
            ReportObjectId = id,
            ImageOrdinal = nextOrdinal + 1,
        };

        await using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream, cancellationToken);
            image.ImageData = stream.ToArray();
        }

        await _context.ReportObjectImagesDocs.AddAsync(image, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ReportImageDto
        {
            Id = image.ImageId,
            Ordinal = image.ImageOrdinal,
            Source = image.ImageSource,
        };
    }

    public Task<IReadOnlyList<LookupDto>> GetLookupValuesAsync(
        string lookupArea,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var indexType = lookupArea switch
        {
            "org-value" => "organizational_value",
            "run-freq" => "run_frequency",
            "fragility" => "fragility",
            "maint-sched" => "maintenance_schedule",
            "ro-fragility" => "fragility_tag",
            "maint-log-stat" => "maintenance_log_status",
            _ => null,
        };

        if (string.IsNullOrEmpty(indexType))
        {
            return Task.FromResult<IReadOnlyList<LookupDto>>(Array.Empty<LookupDto>());
        }

        var values = _solrLookup
            .Query(
                new SolrQuery($"item_type:({indexType})"),
                new QueryOptions
                {
                    RequestHandler = new RequestHandlerParameters("/query"),
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 9999,
                }
            )
            .Select(x => new LookupDto
            {
                Id = x.AtlasId,
                Name = x.Name,
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<LookupDto>>(values);
    }

    public Task<IReadOnlyList<ReportSearchResultDto>> SearchTermsAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ReportSearchResultDto>>(
            SearchObjects(search, "/aterms")
        );
    }

    public Task<IReadOnlyList<ReportSearchResultDto>> SearchCollectionsAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ReportSearchResultDto>>(
            SearchObjects(search, "/collections")
        );
    }

    public Task<IReadOnlyList<ReportSearchResultDto>> SearchUsersAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var queryString = IndexModel.BuildSearchString(
            search ?? string.Empty,
            _httpContextAccessor.HttpContext?.Request.Query ?? new QueryCollection()
        );
        var results = _solr
            .Query(
                new SolrQuery(queryString),
                new QueryOptions
                {
                    RequestHandler = new RequestHandlerParameters("/users"),
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 10,
                }
            )
            .Select(x => new ReportSearchResultDto
            {
                Id = x.AtlasId,
                Name = x.Name,
                Description = x.Email,
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<ReportSearchResultDto>>(results);
    }
}
