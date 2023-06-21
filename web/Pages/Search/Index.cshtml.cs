using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Atlas_Web.Pages.Search
{
    public class SmallData
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }

        public SmallData(string Id, string Value)
        {
            ObjectId = Id;
            Name = Value;
        }


    }

    [ResponseCache(NoStore = true)]
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        private readonly ISolrReadOnlyOperations<SolrAtlas> _solr;
        private readonly ISolrReadOnlyOperations<SolrAtlasLookups> _solrLookup;
        private readonly IMemoryCache _cache;

        public IndexModel(
            Atlas_WebContext context,
            IMemoryCache cache,
            ISolrReadOnlyOperations<SolrAtlas> solr,
            ISolrReadOnlyOperations<SolrAtlasLookups> solrLookup
        )
        {
            _context = context;

            _cache = cache;
            _solr = solr;
            _solrLookup = solrLookup;
        }

        public SolrAtlasResults SearchResults { get; set; }

        [BindProperty]
        public List<ObjectSearch> ObjectSearch { get; set; }

        [BindProperty]
        public List<ObjectSearch> UserSearch { get; set; }

        [BindProperty]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> AppliedFilters { get; set; }

        public static string BuildSearchString(
            string search_string,
            Microsoft.AspNetCore.Http.IQueryCollection query
        )
        {
            string[] illegal_chars =
            {
                "\\",
                "+",
                "-",
                "&&",
                "||",
                "!",
                "(",
                ")",
                "{",
                "}",
                "[",
                "]",
                "^",
                "~",
                "*",
                "?",
                ":",
                "/",
            };

            for (var x = 0; x < illegal_chars.Length; x++)
            {
                string current = illegal_chars[x];
                search_string = search_string.Replace(current, "\\" + current);
            }

            // make all caps lower for OR AND NOT
            search_string = Regex.Replace(
                search_string,
                @"\b(OR|AND|NOT)\b",
                m => m.ToString().ToLower()
            );

            // find exact match strings
            List<string> ExactMatches = new();

            var literals = Regex.Matches(search_string, @"("")(.+?)("")");
            if (literals.Count > 0)
            {
#pragma warning disable S3267
                foreach (Match literal in literals)
                {
#pragma warning restore S3267
                    if (query.Keys.Contains("field"))
                    {
                        ExactMatches.Add($"{query["field"]}:\"{literal.Groups[2].Value}\"");
                    }
                    else
                    {
                        ExactMatches.Add(
                            String.Join(
                                " ",
                                $"name:\"{literal.Groups[2].Value}\"^8 OR",
                                $"description: \"{literal.Groups[2].Value}\"^5 OR",
                                $"email: \"{literal.Groups[2].Value}\" OR",
                                $"external_url: \"{literal.Groups[2].Value}\" OR",
                                $"financial_impact: \"{literal.Groups[2].Value}\" OR",
                                $"fragility_tags: \"{literal.Groups[2].Value}\" OR",
                                $"group_type: \"{literal.Groups[2].Value}\" OR",
                                $"linked_description: \"{literal.Groups[2].Value}\" OR",
                                $"maintenance_schedule: \"{literal.Groups[2].Value}\" OR",
                                $"operations_owner: \"{literal.Groups[2].Value}\" OR",
                                $"organizational_value: \"{literal.Groups[2].Value}\" OR",
                                $"related_collections: \"{literal.Groups[2].Value}\" OR",
                                $"related_initiatives: \"{literal.Groups[2].Value}\" OR",
                                $"related_reports: \"{literal.Groups[2].Value}\" OR",
                                $"related_terms: \"{literal.Groups[2].Value}\" OR",
                                $"report_last_updated_by: \"{literal.Groups[2].Value}\" OR",
                                $"report_type: \"{literal.Groups[2].Value}\" OR",
                                $"requester: \"{literal.Groups[2].Value}\" OR",
                                $"source_database: \"{literal.Groups[2].Value}\" OR",
                                $"strategic_importance: \"{literal.Groups[2].Value}\" OR",
                                $"updated_by: \"{literal.Groups[2].Value}\" OR",
                                $"user_groups: \"{literal.Groups[2].Value}\" OR",
                                $"user_roles: \"{literal.Groups[2].Value}\" OR",
                                $"source_database: \"{literal.Groups[2].Value}\""
                            )
                        );
                    }
                }
                search_string = Regex.Replace(search_string, @"("".+?"")", "");
            }

            // clean double quote from search string
            search_string = search_string.Replace("\"", "\\\"").Trim();

            static string BuildExact(string wild, List<string> exact)
            {
                if (exact.Count == 0)
                {
                    return wild;
                }
                var exact_string = string.Join(" AND ", exact);

                if (wild == "")
                {
                    return exact_string;
                }

                return $"{exact_string} AND ({wild})";
            }

            if (search_string == "")
            {
                return BuildExact("", ExactMatches);
            }

            if (query.Keys.Contains("field"))
            {
                string field = query["field"];
                return BuildExact($"{field}:({search_string})^60", ExactMatches);
            }

            return BuildExact(
                $"name:({search_string})^12 OR name_split:({search_string})^6 OR description:({search_string})^5 OR description_split:({search_string})^3 OR ({search_string})",
                ExactMatches
            );
        }

        public async Task<IActionResult> OnGet(string Query)
        {
            if (string.IsNullOrEmpty(Query))
            {
                return RedirectToPage("/Index/Index");
            }

            Query = HttpUtility.UrlDecode(Query).Trim();

            int PageIndex = Int32.Parse(Request.Query["PageIndex"].FirstOrDefault() ?? "1");
            string Type = Request.Query["type"].FirstOrDefault() ?? "query";

            static IReadOnlyList<HighlightModel> BuildHighlightModels(
                IDictionary<string, SolrNet.Impl.HighlightedSnippets> highlightResults
            )
            {
                return highlightResults
                    .Select(
                        f =>
                            new HighlightModel(
                                Key: f.Key,
                                Values: f.Value
                                    .Select(v => new HighlightValueModel(v.Key, v.Value.First()))
                                    .ToList()
                            )
                    )
                    .ToList();
            }

            static IReadOnlyList<FilterFields> BuildFilterFields(string query)
            {
                List<FilterFields> filters = new();

                if (query == "reports")
                {
                    filters.Add(new FilterFields("name", "Name"));
                    filters.Add(new FilterFields("description", "Description"));
                    filters.Add(new FilterFields("query", "Query"));
                    filters.Add(new FilterFields("epic_record_id", "Epic ID"));
                    filters.Add(new FilterFields("epic_template", "Epic Template ID"));
                }

                return filters;
            }

            static IReadOnlyList<FacetModel> BuildFacetModels(
                IDictionary<string, ICollection<KeyValuePair<string, int>>> facetResults
            )
            {
                // set the order of some facets. Otherwise solr is count > alpha
                String[] FacetOrder =
                {
                    "epic_master_file_text",
                    "organizational_value_text",
                    "estimated_run_frequency_text",
                    "maintenance_schedule_text",
                    "fragility_text",
                    "executive_visiblity_text",
                    "visible_text",
                    "certification_text",
                    "report_type_text",
                    "type"
                };
                return facetResults
                    .OrderByDescending(x => Array.IndexOf(FacetOrder, x.Key))
                    .Select(
                        f =>
                            new FacetModel(
                                Key: f.Key,
                                Values: f.Value
                                    .Select(v => new FacetValueModel(v.Key, v.Value))
                                    .ToList()
                            )
                    )
                    .ToList();
            }

            static ISolrQuery[] BuildFilterQuery(
                Microsoft.AspNetCore.Http.IQueryCollection query,
                System.Security.Claims.ClaimsPrincipal User
            )
            {
                var FilterQuery = new List<SolrQuery>();

                if (
                    !User.HasPermission("Show Advanced Search")
                    || !query.ContainsKey("advanced")
                    || query.ContainsKey("advanced") && query["advanced"] != "Y"
                )
                {
                    // user cannot access advanced search, so we pass an arg to hide numbers for
                    // hidden objects.
                    FilterQuery.Add(new SolrQuery("visible_text:(Y)"));
                }
                // also exclude the two global keywords, EPIC and msg.
                var ExcludedKeys = new List<string>
                {
                    "PageIndex",
                    "Query",
                    "type",
                    "EPIC",
                    "msg",
                    "field",
                    "advanced",
                    "error",
                    "success",
                    "warning"
                };

                foreach (string key in query.Keys)
                {
                    if (ExcludedKeys.IndexOf(key) == -1)
                    {
                        FilterQuery.Add(
                            new SolrQuery($"{{!tag={key}}}{key}:({query[key].ToString().Trim()})")
                        );
                    }
                }

                return FilterQuery.ToArray();
            }

            static Dictionary<string, string> BuildFilterDict(
                Microsoft.AspNetCore.Http.IQueryCollection query
            )
            {
                var FilterQuery = new Dictionary<string, string>();
                foreach (string key in query.Keys)
                {
                    FilterQuery.Add(key.ToLowerInvariant(), query[key][0]);
                }

                return FilterQuery;
            }

            if (Query != null)
            {
                string hl = "*";
                string hl_match = "false";
                if (Request.Query.Keys.Contains("field"))
                {
                    hl = Request.Query["field"];
                    hl_match = "true";
                }

                var search_string_built = BuildSearchString(Query, Request.Query);
                var search_filter_built = BuildFilterQuery(Request.Query, User);

                var results = await _solr.QueryAsync(
                    new SolrQuery(search_string_built),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters(
#pragma warning disable S1075
                            "/" + Type.Replace("terms", "aterms")
#pragma warning restore S1075
                        ),
                        StartOrCursor = new StartOrCursor.Start((PageIndex - 1) * 20),
                        Rows = 20,
                        FilterQueries = search_filter_built,
                        ExtraParams = new Dictionary<string, string>
                        {
                            { "rq", "{!rerank reRankQuery=$rqq reRankDocs=1000 reRankWeight=5}" },
                            {
                                "rqq",
                                "(type:collections^2.8 OR type:reports^2 OR documented:Y^0.1 OR executive_visibility:Y^0.2  OR certification:\"Analytics Certified\"^0.4 OR certification:\"Analytics Reviewed\"^0.4)"
                            },
                            { "hl.fl", hl },
                            { "hl.requireFieldMatch", hl_match }
                        }
                    }
                );

                var advanced = "N";
                if (
                    User.HasPermission("Show Advanced Search")
                    && Request.Query.ContainsKey("advanced")
                    && Request.Query["advanced"] == "Y"
                )
                {
                    advanced = "Y";
                }

                SolrAtlasParameters parameters =
                    new()
                    {
                        Query = Query,
                        PageIndex = PageIndex,
                        Filters = BuildFilterDict(Request.Query)
                    };

                SearchResults = new SolrAtlasResults(
                    results
                        .OrderBy(x => x.Type == "collections" ? 0 : 1)
                        .Select(
                            x =>
                                new ResultModel
                                {
                                    Id = x.Id,
                                    report =
                                        x.Type == "reports"
                                            ? _cache.GetOrCreate<ReportObject>(
                                                  "search-report-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.ReportObjects
                                                          .Include(x => x.ReportObjectDoc)
                                                          .Include(x => x.ReportObjectAttachments)
                                                          .Include(x => x.StarredReports)
                                                          .Include(x => x.ReportObjectType)
                                                          .Include(x => x.ReportTagLinks)
                                                          .ThenInclude(x => x.Tag)
                                                          // for authentication
                                                          .Include(
                                                              x =>
                                                                  x.ReportObjectHierarchyChildReportObjects
                                                          )
                                                          .ThenInclude(x => x.ParentReportObject)
                                                          .ThenInclude(
                                                              x => x.ReportGroupsMemberships
                                                          )
                                                          .AsSingleQuery()
                                                          .AsNoTracking()
                                                          .SingleOrDefault(
                                                              y => y.ReportObjectId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null,
                                    collection =
                                        x.Type == "collections"
                                            ? _cache.GetOrCreate<Collection>(
                                                  "search-collection-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.Collections
                                                          .Include(x => x.StarredCollections)
                                                          .AsSingleQuery()
                                                          .AsNoTracking()
                                                          .SingleOrDefault(
                                                              y => y.CollectionId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null,
                                    term =
                                        x.Type == "terms"
                                            ? _cache.GetOrCreate<Term>(
                                                  "search-term-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.Terms
                                                          .Include(x => x.StarredTerms)
                                                          .AsSingleQuery()
                                                          .SingleOrDefault(
                                                              y => y.TermId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null,
                                    initiative =
                                        x.Type == "initiatives"
                                            ? _cache.GetOrCreate<Initiative>(
                                                  "search-initaitive-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.Initiatives
                                                          .Include(x => x.StarredInitiatives)
                                                          .AsSingleQuery()
                                                          .SingleOrDefault(
                                                              y => y.InitiativeId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null,
                                    user =
                                        x.Type == "users"
                                            ? _cache.GetOrCreate<User>(
                                                  "search-user-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.Users
                                                          .AsSingleQuery()
                                                          .SingleOrDefault(
                                                              y => y.UserId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null,
                                    group =
                                        x.Type == "groups"
                                            ? _cache.GetOrCreate<UserGroup>(
                                                  "search-group-" + x.AtlasId,
                                                  cacheEntry =>
                                                  {
                                                      cacheEntry.AbsoluteExpirationRelativeToNow =
                                                          TimeSpan.FromMinutes(20);
                                                      return _context.UserGroups
                                                          .AsSingleQuery()
                                                          .SingleOrDefault(
                                                              y => y.GroupId == x.AtlasId
                                                          );
                                                  }
                                              )
                                            : null
                                }
                        )
                        .ToList(),
                    BuildFacetModels(results.FacetFields),
                    BuildHighlightModels(results.Highlights),
                    BuildFilterFields(Type),
                    (int)results.NumFound,
                    results.Header.QTime,
                    parameters,
                    advanced
                );
            }

            SearchString = Query;
            return Page();
        }

        public ActionResult OnPostReportSearch(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            ObjectSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/reports"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 10,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name = x.Name,
                            Description =
                                x.Description != null ? x.Description.FirstOrDefault() : ""
                        }
                )
                .ToList();
            var json = JsonConvert.SerializeObject(ObjectSearch);
            return Content(json);
        }

        public ActionResult OnPostTermSearch(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            ObjectSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/aterms"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 10,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name = x.Name,
                            Description =
                                x.Description != null ? x.Description.FirstOrDefault() : ""
                        }
                )
                .ToList();
            var json = JsonConvert.SerializeObject(ObjectSearch);

            return Content(json);
        }

        public ActionResult OnPostCollectionSearch(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            ObjectSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/collections"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 10,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name = x.Name,
                            Description =
                                x.Description != null ? x.Description.FirstOrDefault() : ""
                        }
                )
                .ToList();

            var json = JsonConvert.SerializeObject(ObjectSearch);

            return Content(json);
        }

        public ActionResult OnPostUserSearch(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            UserSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/users"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 10,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name =
                                x.Name
                                + (!string.IsNullOrEmpty(x.Email) ? " (" + x.Email + ")" : ""),
                            Type = "u"
                        }
                )
                .ToList();

            var json = JsonConvert.SerializeObject(UserSearch);

            return Content(json);
        }

        public ActionResult OnPostGroupSearch(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            UserSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/groups"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 10,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name =
                                x.Name
                                + (!string.IsNullOrEmpty(x.Email) ? " (" + x.Email + ")" : ""),
                            Type = "u"
                        }
                )
                .ToList();

            var json = JsonConvert.SerializeObject(UserSearch);

            return Content(json);
        }

        public ActionResult OnPostUserSearchMail(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            UserSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters("/users"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 20,
                    }
                )
                .Select(
                    x =>
                        new ObjectSearch
                        {
                            ObjectId = x.AtlasId,
                            Name = x.Name,
                            Type = "u"
                        }
                )
                .ToList();

            var json = JsonConvert.SerializeObject(UserSearch);

            return Content(json);
        }

        public ActionResult OnPostDirector(string s)
        {
            if (s != null)
            {
                SearchString = s;
            }

            var FilterQuery = new List<SolrQuery>();
            FilterQuery.Add(new SolrQuery("user_roles_text:(Director)"));

            UserSearch = _solr
                .Query(
                    new SolrQuery(BuildSearchString(SearchString, Request.Query)),
                    new QueryOptions
                    {
                        FilterQueries = FilterQuery.ToArray(),
                        RequestHandler = new RequestHandlerParameters("/users"),
                        StartOrCursor = new StartOrCursor.Start(0),
                        Rows = 20,
                    }
                )
                .Select(x => new ObjectSearch { Description = x.Email, Name = x.Name })
                .ToList();

            var json = JsonConvert.SerializeObject(UserSearch);

            return Content(json);
        }

        public ActionResult OnPostValueList(string s)
        {
            List<SmallData> myObject = new() { new SmallData("0", "error") };
            string index_type = "";
            switch (s)
            {
                default:
                    index_type = null;
                    break;

                case "org-value":
                    index_type = "organizational_value";
                    break;

                case "run-freq":
                    index_type = "run_frequency";
                    break;

                case "fragility":
                    index_type = "fragility";
                    break;

                case "maint-sched":
                    index_type = "maintenance_schedule";
                    break;

                case "ro-fragility":
                    index_type = "fragility_tag";
                    break;

                case "maint-log-stat":
                    index_type = "maintenance_log_status";
                    break;

                case "user-roles":
                    index_type = "user_roles";
                    break;

                case "financial-impact":
                    index_type = "financial_impact";
                    break;

                case "strategic-importance":
                    index_type = "strategic_importance";
                    break;
            }

            if (index_type != "")
            {
                myObject = _solrLookup
                    .Query(
                        new SolrQuery($"item_type:({index_type})"),
                        new QueryOptions
                        {
                            RequestHandler = new RequestHandlerParameters("/query"),
                            StartOrCursor = new StartOrCursor.Start(0),
                            Rows = 9999,
                        }
                    )
                    .Select(x => new SmallData(x.AtlasId.ToString(), x.Name))
                    .ToList();
            }

            var json = JsonConvert.SerializeObject(myObject);

            return Content(json);
        }
    }
}
