using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Text.RegularExpressions;

namespace Atlas_Web.Pages.Search
{
    public class SmallData
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }

        public SmallData(string Id, string Value)
        {
            this.ObjectId = Id;
            this.Name = Value;
        }


    }

    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private readonly ISolrReadOnlyOperations<SolrAtlas> _solr;
        private readonly ISolrReadOnlyOperations<SolrAtlasLookups> _solrLookup;
        private IMemoryCache _cache;

        public IndexModel(
            Atlas_WebContext context,
            IConfiguration config,
            IMemoryCache cache,
            ISolrReadOnlyOperations<SolrAtlas> solr,
            ISolrReadOnlyOperations<SolrAtlasLookups> solrLookup
        )
        {
            _context = context;
            _config = config;
            _cache = cache;
            _solr = solr;
            _solrLookup = solrLookup;
        }

        public class SearchCollectionData
        {
            public SearchCollectionData() { }

            public int CollectionId { get; set; }
            public string Annotation { get; set; }
            public string Name { get; set; }
        }

        public List<UserPreference> Preferences { get; set; }
        public List<int?> Permissions { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public List<AdList> AdLists { get; set; }
        public List<SearchCollectionData> Collections { get; set; }

        [BindProperty]
        public List<ReportObjectType> AvailableFilters
        {
            get { return _context.ReportObjectTypes.ToList(); }
            set { }
        }
        public SolrAtlasResults SearchResults { get; set; }

        [BindProperty]
        public List<ObjectSearch> ObjectSearch { get; set; }

        [BindProperty]
        public List<ObjectSearch> UserSearch { get; set; }

        [BindProperty]
        public int ShowHidden { get; set; }

        [BindProperty]
        public int ShowAllTypes { get; set; }

        [BindProperty]
        public int ShowOrphans { get; set; }

        [BindProperty]
        public string SearchFilter { get; set; }

        [BindProperty]
        public string SearchField { get; set; }

        [BindProperty]
        public string Category { get; set; }

        [BindProperty]
        public int SearchPage { get; set; }

        [BindProperty]
        public int PageSize { get; set; }

        [BindProperty]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> AppliedFilters { get; set; }
        public User PublicUser { get; set; }

        public static string BuildSearchString(
            string search_string,
            Microsoft.AspNetCore.Http.IQueryCollection query
        )
        {
            string[] illegal_chars = new string[]
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
                // "\"",
                "~",
                "*",
                "?",
                ":",
                "/",
            };
            search_string = String.Join(
                "",
                search_string.Split(illegal_chars, StringSplitOptions.RemoveEmptyEntries)
            );

            // find exact match strings
            List<string> ExactMatches = new List<string>();

            var literals = Regex.Matches(search_string, @"("")(.+?)("")");
            if (literals.Count > 0)
            {
                foreach (Match literal in literals)
                {
                    if (query.Keys.Contains("field"))
                    {
                        ExactMatches.Add($"{query["field"]}:({literal.Groups[2].Value})");
                    }
                    else
                    {
                        ExactMatches.Add(
                            $"name:({literal.Groups[2].Value})^8 OR ({literal.Groups[2].Value})^5"
                        );
                    }
                }
                search_string = Regex.Replace(search_string, @"("".+?"")", "");
            }

            // clean double quote from search string
            search_string = String.Join(
                "",
                search_string.Split("\"", StringSplitOptions.RemoveEmptyEntries)
            );

            static string BuildFuzzy(string substr)
            {
                if (substr.Length > 2)
                {
                    return substr + "~" + Math.Max(substr.Length / 3, 1).ToString();
                }
                else if (substr.Length == 2)
                {
                    return substr;
                }
                return substr;
            }

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
            string Fuzzy = String.Join(
                " ",
                search_string
                    .Split(' ')
                    .Where(s => !String.IsNullOrEmpty(s))
                    .Select(x => BuildFuzzy(x))
            );

            string Wild = String.Join(
                " ",
                search_string.Split(' ').Where(s => !String.IsNullOrEmpty(s)).Select(x => x + "*")
            );

            if (search_string == "")
            {
                return BuildExact("", ExactMatches);
            }

            if (query.Keys.Contains("field"))
            {
                string field = query["field"];
                return BuildExact(
                    $"{field}:({search_string})^60 OR {field}:({Fuzzy})^3",
                    ExactMatches
                );
            }

            return BuildExact(
                $"name:({search_string})^12 OR name:({Fuzzy})^7 OR name:({Wild})^6 OR ({search_string})^5 OR ({Fuzzy}) OR ({Wild})",
                ExactMatches
            );
        }

        public async Task<IActionResult> OnGet(string Query)
        {
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
                List<FilterFields> filters = new List<FilterFields>();

                if (query == "reports")
                {
                    filters.Add(new FilterFields("name", "Name"));
                    filters.Add(new FilterFields("description", "Description"));
                    filters.Add(new FilterFields("query", "Query"));
                    filters.Add(new FilterFields("description", "Description"));
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
                IMemoryCache _cache,
                Atlas_WebContext _context,
                System.Security.Claims.ClaimsPrincipal User
            )
            {
                var FilterQuery = new List<SolrQuery>();

                var checkpoint = UserHelpers.CheckUserPermissions(
                    _cache,
                    _context,
                    User.Identity.Name,
                    46
                );
                if (
                    !checkpoint
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
                    FilterQuery.Add(key.ToLower(), query[key].First());
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
                var search_filter_built = BuildFilterQuery(Request.Query, _cache, _context, User);

                var results = await _solr.QueryAsync(
                    new SolrQuery(search_string_built),
                    new QueryOptions
                    {
                        RequestHandler = new RequestHandlerParameters(
                            "/" + Type.Replace("terms", "aterms")
                        ),
                        StartOrCursor = new StartOrCursor.Start((PageIndex - 1) * 20),
                        Rows = 20,
                        FilterQueries = search_filter_built,
                        ExtraParams = new Dictionary<string, string>
                        {
                            { "rq", "{!rerank reRankQuery=$rqq reRankDocs=1000 reRankWeight=5}" },
                            {
                                "rqq",
                                "(type:collections^3 OR type:reports^2 OR documented:Y^0.1 OR executive_visibility_text:Y^0.2  OR certification_text:\"Analytics Certified\"^0.4 OR certification_text:\"Analytics Reviewed\"^0.4)"
                            },
                            { "hl.fl", hl },
                            { "hl.requireFieldMatch", hl_match }
                        }
                    }
                );

                var checkpoint = UserHelpers.CheckUserPermissions(
                    _cache,
                    _context,
                    User.Identity.Name,
                    46
                );
                var advanced = "N";
                if (
                    checkpoint
                    && Request.Query.ContainsKey("advanced")
                    && Request.Query["advanced"] == "Y"
                )
                {
                    advanced = "Y";
                }

                SolrAtlasParameters parameters = new SolrAtlasParameters
                {
                    Query = Query,
                    PageIndex = PageIndex,
                    Filters = BuildFilterDict(Request.Query)
                };

                SearchResults = new SolrAtlasResults(
                    results
                        .OrderBy(x => x.Type.First() == "collections" ? 0 : 1)
                        .Select(
                            x =>
                                new SearchResult(
                                    x.ReportTypeId != null
                                        && (
                                            x.ReportTypeId.First() == 3
                                            || x.ReportTypeId.First() == 17
                                        )
                                        && Helpers.UserHelpers.CheckHrxPermissions(
                                            _context,
                                            x.AtlasId.First(),
                                            User.Identity.Name
                                        )
                                      ? _context.ReportObjectAttachments
                                        .Where(
                                            y =>
                                                y.ReportObjectId == x.AtlasId.First()
                                                && x.Type.First() == "reports"
                                        )
                                        .OrderByDescending(y => y.CreationDate)
                                        .ToList()
                                      : new List<ReportObjectAttachment>(),
                                    x,
                                    x.Type.First() == "reports"
                                      ? HtmlHelpers.ReportUrlFromParams(
                                            _config["AppSettings:org_domain"],
                                            HttpContext,
                                            _context.ReportObjects
                                                .Where(y => y.ReportObjectId == x.AtlasId.First())
                                                .FirstOrDefault(),
                                            _context,
                                            User.Identity.Name
                                        )
                                      : null
                                )
                        )
                        .ToList(),
                    BuildFacetModels(results.FacetFields),
                    BuildHighlightModels(results.Highlights),
                    BuildFilterFields(Type),
                    results.NumFound,
                    results.Header.QTime,
                    parameters,
                    advanced
                );
            }

            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            SearchString = Query;
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            ViewData["Fullname"] = PublicUser.Fullname_Cust;
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);

            //ReportId = string.Join(",", SearchResults.Results.Where(x => x.Type.First() == "reports").Select(x => x.AtlasId.First()).ToList());
            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentCollections", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;

            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);

            HttpContext.Response.Headers.Add(
                "Cache-Control",
                "no-cache, no-store, must-revalidate"
            );
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.

            return Page();
        }

        public ActionResult OnPostReportSearch(string s, string e)
        {
            if (s != null)
                SearchString = s;
            {
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
                                ObjectId = x.AtlasId.First(),
                                Name = x.Name,
                                Description =
                                    x.Description != null ? x.Description.FirstOrDefault() : ""
                            }
                    )
                    .ToList();
                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostTermSearch(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
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
                                ObjectId = x.AtlasId.First(),
                                Name = x.Name,
                                Description =
                                    x.Description != null ? x.Description.FirstOrDefault() : ""
                            }
                    )
                    .ToList();
                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostCollectionSearch(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
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
                                ObjectId = x.AtlasId.First(),
                                Name = x.Name,
                                Description =
                                    x.Description != null ? x.Description.FirstOrDefault() : ""
                            }
                    )
                    .ToList();

                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostUserSearch(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
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
                                ObjectId = x.AtlasId.First(),
                                Name = x.Name,
                                Type = "u"
                            }
                    )
                    .ToList();

                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostUserSearchMail(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
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
                                ObjectId = x.AtlasId.First(),
                                Name = x.Name,
                                Type = "u"
                            }
                    )
                    .ToList();

                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostDirector(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
                UserSearch = _solr
                    .Query(
                        new SolrQuery(
                            BuildSearchString(SearchString, Request.Query)
                                + " AND user_roles:(Director)"
                        ),
                        new QueryOptions
                        {
                            RequestHandler = new RequestHandlerParameters("/users"),
                            StartOrCursor = new StartOrCursor.Start(0),
                            Rows = 20,
                        }
                    )
                    .Select(x => new ObjectSearch { Description = x.Email, Name = x.Name })
                    .ToList();

                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostValueList(string s)
        {
            List<SmallData> myObject = new List<SmallData>() { new SmallData("0", "error") };
            string index_type = "";
            switch (s)
            {
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

                case "ext-cont":
                    index_type = "initiative_contacts";
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
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=0");
            return Content(json);
        }
    }
}
