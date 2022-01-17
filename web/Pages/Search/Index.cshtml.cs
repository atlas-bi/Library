/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using SolrNet;
using SolrNet.Commands.Parameters;

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
        private IMemoryCache _cache;

        public IndexModel(
            Atlas_WebContext context,
            IConfiguration config,
            IMemoryCache cache,
            ISolrReadOnlyOperations<SolrAtlas> solr
        )
        {
            _context = context;
            _config = config;
            _cache = cache;
            _solr = solr;
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

        public async Task<IActionResult> OnGet(string Query)
        {
            int PageIndex = Int32.Parse(Request.Query["PageIndex"].FirstOrDefault() ?? "1");
            string Type = Request.Query["type"].FirstOrDefault() ?? "query";

            static string BuildSearchString(
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
                    "\"",
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

                static string BuildFuzzy(string substr)
                {
                    if (substr.Length > 2)
                    {
                        return substr + "~" + Math.Max(substr.Length / 3, 1).ToString();
                    }
                    else if (substr.Length == 2)
                    {
                        return substr + "*~";
                    }
                    return substr;
                }

                string Fuzzy = String.Join(
                    " ",
                    search_string
                        .Split(' ')
                        .Where(s => !String.IsNullOrEmpty(s))
                        .Select(x => BuildFuzzy(x))
                );

                if (query.Keys.Contains("field"))
                {
                    string field = query["field"];
                    return $"{field}:({search_string})^6 OR {field}:({Fuzzy})^3";
                }

                return $"name:({search_string})^6 OR name:({Fuzzy})^3 OR ({search_string})^5 OR ({Fuzzy})";
            }

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
                    "advanced"
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
                        StartOrCursor = new StartOrCursor.Start((PageIndex - 1) * 10),
                        Rows = 10,
                        FilterQueries = search_filter_built,
                        ExtraParams = new Dictionary<string, string>
                        {
                            { "rq", "{!rerank reRankQuery=$rqq reRankDocs=1000 reRankWeight=10}" },
                            {
                                "rqq",
                                "(type:collections^2 OR documented:Y OR executive_visibility_text:Y OR enabled_for_hyperspace_text:Y OR certification_text:\"Analytics Certified\"^1)"
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
                                                .First(),
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicReportSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            ObjectSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["ReportObjectId"],
                                    Name = datareader["ReportObjectName"].ToString(),
                                    Description = datareader["Description"].ToString(),
                                }
                            );
                        }
                    }
                }
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicTermSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            ObjectSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["TermId"],
                                    Name = datareader["Name"].ToString(),
                                    Description = datareader["Summary"].ToString(),
                                }
                            );
                        }
                    }
                }
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicProjectSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }

                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            ObjectSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["DataProjectId"],
                                    Name = datareader["Name"].ToString(),
                                    Description = datareader["Description"].ToString(),
                                }
                            );
                        }
                    }
                }
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicUserSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            UserSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["Userid"],
                                    Name = datareader["Username"].ToString(),
                                    Type = datareader["S"].ToString()
                                }
                            );
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostUserProfileSearch(string s, string e)
        {
            if (s != null)
                SearchString = s; //.Replace("'","''").Replace(";","_");
            {
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicUserSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        command.Parameters.Add(new SqlParameter("@type", "a"));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            UserSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["Userid"],
                                    Name = datareader["Username"].ToString(),
                                    Type = datareader["S"].ToString()
                                }
                            );
                        }
                    }
                }
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicUserSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        command.Parameters.Add(new SqlParameter("@type", "a"));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            UserSearch.Add(
                                new ObjectSearch
                                {
                                    ObjectId = (int)datareader["Userid"],
                                    Name = datareader["Username"].ToString(),
                                    Type = datareader["S"].ToString()
                                }
                            );
                        }
                    }
                }
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
                using (
                    var connection = new SqlConnection(_config.GetConnectionString("AtlasDatabase"))
                )
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "BasicDirectorSearch";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        command.Parameters.Add(new SqlParameter("@results", 20));
                        if (e != null)
                        {
                            command.Parameters.Add(new SqlParameter("@exclude", e));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            UserSearch.Add(
                                new ObjectSearch
                                {
                                    Description = (string)datareader["Email"],
                                    Name = datareader["FullName"].ToString()
                                }
                            );
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostValueList(string s)
        {
            List<SmallData> myObject = new List<SmallData>() { new SmallData("0", "error") };

            switch (s)
            {
                case "org-value":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "org-value",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.OrganizationalValues
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.OrganizationalValueId.ToString(),
                                            x.OrganizationalValueName
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "run-freq":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "run-freq",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.EstimatedRunFrequencies
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.EstimatedRunFrequencyId.ToString(),
                                            x.EstimatedRunFrequencyName
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "fragility":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "fragility",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.Fragilities
                                .Select(
                                    x => new SmallData(x.FragilityId.ToString(), x.FragilityName)
                                )
                                .ToList();
                        }
                    );
                    break;

                case "maint-sched":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "maint-sched",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.MaintenanceSchedules
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.MaintenanceScheduleId.ToString(),
                                            x.MaintenanceScheduleName
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "ro-fragility":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "ro-fragility",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.FragilityTags
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.FragilityTagId.ToString(),
                                            x.FragilityTagName
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "maint-log-stat":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "maint-log-stat",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.MaintenanceLogStatuses
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.MaintenanceLogStatusId.ToString(),
                                            x.MaintenanceLogStatusName
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "ext-cont":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "ext-cont",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.DpContacts
                                .Select(
                                    x =>
                                        new SmallData(
                                            x.ContactId.ToString(),
                                            x.Name + " - " + x.Company
                                        )
                                )
                                .ToList();
                        }
                    );
                    break;

                case "mile-temp":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "mile-temp",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.DpMilestoneFrequencies
                                .Select(x => new SmallData(x.MilestoneTypeId.ToString(), x.Name))
                                .ToList();
                        }
                    );
                    break;

                case "mile-type":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "mile-type",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.DpMilestoneTemplates
                                .Select(
                                    x => new SmallData(x.MilestoneTemplateId.ToString(), x.Name)
                                )
                                .ToList();
                        }
                    );
                    break;

                case "user-roles":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "user-roles",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.UserRoles
                                .Select(x => new SmallData(x.UserRolesId.ToString(), x.Name))
                                .ToList();
                        }
                    );
                    break;

                case "financial-impact":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "financial-impact",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.FinancialImpacts
                                .Select(x => new SmallData(x.FinancialImpactId.ToString(), x.Name))
                                .ToList();
                        }
                    );
                    break;

                case "strategic-importance":
                    myObject = _cache.GetOrCreate<List<SmallData>>(
                        "strategic-importance",
                        cacheEntry =>
                        {
                            cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                            return _context.StrategicImportances
                                .Select(
                                    x => new SmallData(x.StrategicImportanceId.ToString(), x.Name)
                                )
                                .ToList();
                        }
                    );
                    break;
            }

            var json = JsonConvert.SerializeObject(myObject);
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=0");
            return Content(json);
        }
    }
}
