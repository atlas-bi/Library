using System.Text.RegularExpressions;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Search;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface ISearchApiService
{
    Task<SearchResponseDto> SearchAsync(
        ClaimsPrincipal user,
        string q,
        string type,
        int page,
        int pageSize,
        string field,
        bool advanced,
        IReadOnlyDictionary<string, string> filters,
        CancellationToken cancellationToken
    );
}

public sealed class SearchApiService : ISearchApiService
{
    private const int MaxPageSize = 100;

    private static readonly string[] FacetOrder =
    [
        "epic_master_file_text",
        "organizational_value_text",
        "estimated_run_frequency_text",
        "maintenance_schedule_text",
        "fragility_text",
        "executive_visiblity_text",
        "visible_text",
        "certification_text",
        "report_type_text",
        "type",
    ];

    private static readonly string ReRankQuery =
        "(type:collections^1.2 OR type:reports^2 OR documented:Y^0.1 OR executive_visibility:Y^0.2"
        + " OR certification:\"Analytics Certified\"^0.4 OR certification:\"Analytics Reviewed\"^0.4)";

    private readonly Atlas_WebContext _context;
    private readonly ISolrReadOnlyOperations<SolrAtlas> _solr;

    public SearchApiService(
        Atlas_WebContext context,
        ISolrReadOnlyOperations<SolrAtlas> solr
    )
    {
        _context = context;
        _solr = solr;
    }

    public async Task<SearchResponseDto> SearchAsync(
        ClaimsPrincipal user,
        string q,
        string type,
        int page,
        int pageSize,
        string field,
        bool advanced,
        IReadOnlyDictionary<string, string> filters,
        CancellationToken cancellationToken
    )
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        var handler = MapTypeToHandler(type);
        var searchQuery = BuildSearchQuery(q, field);
        var filterQueries = BuildFilterQueries(user, advanced, filters);

        var hlField = string.IsNullOrEmpty(field) ? "*" : field;
        var hlRequireMatch = string.IsNullOrEmpty(field) ? "false" : "true";

        var solrResults = await _solr.QueryAsync(
            new SolrQuery(searchQuery),
            new QueryOptions
            {
                RequestHandler = new RequestHandlerParameters(handler),
                StartOrCursor = new StartOrCursor.Start((safePage - 1) * safePageSize),
                Rows = safePageSize,
                FilterQueries = filterQueries,
                ExtraParams = new Dictionary<string, string>
                {
                    { "rq", "{!rerank reRankQuery=$rqq reRankDocs=1000 reRankWeight=5}" },
                    { "rqq", ReRankQuery },
                    { "hl.fl", hlField },
                    { "hl.requireFieldMatch", hlRequireMatch },
                },
            }
        );

        var isAdvanced =
            advanced && user.HasPermission("Show Advanced Search");

        var results = solrResults
            .OrderBy(x => x.Type == "collections" ? 0 : 1)
            .Select(x => new SearchResultDto
            {
                Id = x.Id,
                AtlasId = x.AtlasId,
                Type = x.Type,
                Name = x.Name,
                Description = x.Description?.FirstOrDefault(),
                Url = x.ReportObjectUrl,
                ReportType = x.ReportType,
                Email = x.Email,
                EpicMasterFile = x.EpicMasterFile,
                EpicRecordId = x.EpicRecordId,
                EpicTemplateId = x.EpicTemplateId,
                ReportServerPath = x.ReportServerPath,
                ExecutiveVisibility = x.ExecutiveVisiblity,
                SourceServer = x.SourceServer,
                GroupType = x.GroupType,
                Certifications = x.Certification?.ToList() ?? [],
                Documented = x.Documented,
            })
            .ToList();

        await EnrichWithStarredStatusAsync(user, results, cancellationToken);

        return new SearchResponseDto
        {
            Results = results,
            Facets = BuildFacets(solrResults.FacetFields),
            Highlights = BuildHighlights(solrResults.Highlights),
            FilterFields = BuildFilterFields(type),
            Total = solrResults.NumFound,
            Page = safePage,
            PageSize = safePageSize,
            QTime = solrResults.Header.QTime,
            IsAdvancedSearch = isAdvanced,
        };
    }

    private static string MapTypeToHandler(string type)
    {
        return type switch
        {
            "reports" => "/reports",
            "terms" => "/aterms",
            "collections" => "/collections",
            "initiatives" => "/initiatives",
            "users" => "/users",
            "groups" => "/groups",
            _ => "/query",
        };
    }

    private static ISolrQuery[] BuildFilterQueries(
        ClaimsPrincipal user,
        bool advanced,
        IReadOnlyDictionary<string, string> filters
    )
    {
        var filterList = new List<SolrQuery>();

        if (!user.HasPermission("Show Advanced Search") || !advanced)
        {
            filterList.Add(new SolrQuery("visible_text:(Y)"));
        }

        foreach (var (key, value) in filters)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                filterList.Add(new SolrQuery($"{{!tag={key}}}{key}:({value.Trim()})"));
            }
        }

        return [.. filterList];
    }

    private static IReadOnlyList<FacetDto> BuildFacets(
        IDictionary<string, ICollection<KeyValuePair<string, int>>> facetFields
    )
    {
        return facetFields
            .OrderByDescending(x => Array.IndexOf(FacetOrder, x.Key))
            .Select(f => new FacetDto
            {
                Key = f.Key,
                Values = f.Value
                    .Select(v => new FacetValueDto { Value = v.Key, Count = v.Value })
                    .ToList(),
            })
            .ToList();
    }

    private static IReadOnlyList<HighlightDto> BuildHighlights(
        IDictionary<string, SolrNet.Impl.HighlightedSnippets> highlights
    )
    {
        return highlights
            .Select(h => new HighlightDto
            {
                Id = h.Key,
                Fields = h.Value
                    .Select(f => new HighlightFieldDto
                    {
                        Field = f.Key,
                        Snippet = f.Value.FirstOrDefault(),
                    })
                    .ToList(),
            })
            .ToList();
    }

    private static IReadOnlyList<FilterFieldDto> BuildFilterFields(string type)
    {
        if (type != "reports")
        {
            return [];
        }

        return
        [
            new FilterFieldDto { Key = "name", Label = "Name" },
            new FilterFieldDto { Key = "description", Label = "Description" },
            new FilterFieldDto { Key = "query", Label = "Query" },
            new FilterFieldDto { Key = "epic_record_id", Label = "Epic ID" },
            new FilterFieldDto { Key = "epic_template", Label = "Epic Template ID" },
        ];
    }

    private async Task EnrichWithStarredStatusAsync(
        ClaimsPrincipal user,
        IReadOnlyList<SearchResultDto> results,
        CancellationToken cancellationToken
    )
    {
        if (results.Count == 0)
        {
            return;
        }

        var userId = user.GetUserId();

        var reportIds = results.Where(x => x.Type == "reports").Select(x => x.AtlasId).ToList();
        var collectionIds = results
            .Where(x => x.Type == "collections")
            .Select(x => x.AtlasId)
            .ToList();
        var termIds = results.Where(x => x.Type == "terms").Select(x => x.AtlasId).ToList();
        var initiativeIds = results
            .Where(x => x.Type == "initiatives")
            .Select(x => x.AtlasId)
            .ToList();
        var userIds = results.Where(x => x.Type == "users").Select(x => x.AtlasId).ToList();
        var groupIds = results.Where(x => x.Type == "groups").Select(x => x.AtlasId).ToList();

        var starredReports = reportIds.Count > 0
            ? (await _context.StarredReports
                .Where(x => x.Ownerid == userId && reportIds.Contains(x.Reportid))
                .Select(x => x.Reportid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        var starredCollections = collectionIds.Count > 0
            ? (await _context.StarredCollections
                .Where(x => x.Ownerid == userId && collectionIds.Contains(x.Collectionid))
                .Select(x => x.Collectionid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        var starredTerms = termIds.Count > 0
            ? (await _context.StarredTerms
                .Where(x => x.Ownerid == userId && termIds.Contains(x.Termid))
                .Select(x => x.Termid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        var starredInitiatives = initiativeIds.Count > 0
            ? (await _context.StarredInitiatives
                .Where(x => x.Ownerid == userId && initiativeIds.Contains(x.Initiativeid))
                .Select(x => x.Initiativeid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        var starredUsers = userIds.Count > 0
            ? (await _context.StarredUsers
                .Where(x => x.Ownerid == userId && userIds.Contains(x.Userid))
                .Select(x => x.Userid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        var starredGroups = groupIds.Count > 0
            ? (await _context.StarredGroups
                .Where(x => x.Ownerid == userId && groupIds.Contains(x.Groupid))
                .Select(x => x.Groupid)
                .ToListAsync(cancellationToken)).ToHashSet()
            : [];

        foreach (var result in results)
        {
            result.IsStarred = result.Type switch
            {
                "reports" => starredReports.Contains(result.AtlasId),
                "collections" => starredCollections.Contains(result.AtlasId),
                "terms" => starredTerms.Contains(result.AtlasId),
                "initiatives" => starredInitiatives.Contains(result.AtlasId),
                "users" => starredUsers.Contains(result.AtlasId),
                "groups" => starredGroups.Contains(result.AtlasId),
                _ => false,
            };
        }
    }

    // Mirrors the query-building logic from the Search Razor Page.
    private static string BuildSearchQuery(string searchString, string field)
    {
        string[] illegalChars =
        [
            "\\", "+", "-", "&&", "||", "!", "(", ")", "{", "}", "[", "]", "^", "~", "*", "?",
            ":", "/",
        ];

        foreach (var ch in illegalChars)
        {
            searchString = searchString.Replace(ch, "\\" + ch);
        }

        searchString = Regex.Replace(
            searchString,
            @"\b(OR|AND|NOT)\b",
            m => m.ToString().ToLower(),
            RegexOptions.None,
            TimeSpan.FromSeconds(1)
        );

        var exactMatches = new List<string>();
        var literals = Regex.Matches(searchString, @"("")(.+?)("")", RegexOptions.None, TimeSpan.FromSeconds(1));

        foreach (Match literal in literals)
        {
            if (!string.IsNullOrEmpty(field))
            {
                exactMatches.Add($"{field}:\"{literal.Groups[2].Value}\"");
            }
            else
            {
                var v = literal.Groups[2].Value;
                exactMatches.Add(string.Join(
                    " ",
                    $"name:\"{v}\"^8 OR",
                    $"description:\"{v}\"^5 OR",
                    $"email:\"{v}\" OR",
                    $"external_url:\"{v}\" OR",
                    $"financial_impact:\"{v}\" OR",
                    $"fragility_tags:\"{v}\" OR",
                    $"group_type:\"{v}\" OR",
                    $"linked_description:\"{v}\" OR",
                    $"maintenance_schedule:\"{v}\" OR",
                    $"operations_owner:\"{v}\" OR",
                    $"organizational_value:\"{v}\" OR",
                    $"related_collections:\"{v}\" OR",
                    $"related_initiatives:\"{v}\" OR",
                    $"related_reports:\"{v}\" OR",
                    $"related_terms:\"{v}\" OR",
                    $"report_last_updated_by:\"{v}\" OR",
                    $"report_type:\"{v}\" OR",
                    $"requester:\"{v}\" OR",
                    $"source_database:\"{v}\" OR",
                    $"strategic_importance:\"{v}\" OR",
                    $"updated_by:\"{v}\" OR",
                    $"user_groups:\"{v}\" OR",
                    $"user_roles:\"{v}\""
                ));
            }
        }

        searchString = Regex.Replace(searchString, @"("".+?"")", "", RegexOptions.None, TimeSpan.FromSeconds(1))
            .Replace("\"", "\\\"")
            .Trim();

        static string Combine(string wild, List<string> exact)
        {
            if (exact.Count == 0)
            {
                return wild;
            }

            var exactPart = string.Join(" AND ", exact);
            return wild == "" ? exactPart : $"{exactPart} AND ({wild})";
        }

        if (searchString == "")
        {
            return Combine("", exactMatches);
        }

        if (!string.IsNullOrEmpty(field))
        {
            return Combine($"{field}:({searchString})^60", exactMatches);
        }

        return Combine(
            $"name:({searchString})^12 OR name_split:({searchString})^6"
            + $" OR description:({searchString})^5 OR description_split:({searchString})^3"
            + $" OR ({searchString})",
            exactMatches
        );
    }
}
