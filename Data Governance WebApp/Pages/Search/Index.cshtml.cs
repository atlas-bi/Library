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
using Data_Governance_WebApp.Models;
using Data_Governance_WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;


namespace Data_Governance_WebApp.Pages.Search
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
        private readonly Data_GovernanceContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public IndexModel(Data_GovernanceContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
             _cache = cache;
        }

        public List<UserPreferences> Preferences { get; set; }
        public List<int?> Permissions { get; set; }
        public List<UserFavorites> Favorites { get; set; }
        [BindProperty] public List<ReportObjectType> AvailableFilters { get { return _context.ReportObjectType.ToList(); } set { } }
        [BindProperty(SupportsGet = true)] public List<SearchResult> SearchResults { get; set; }
        [BindProperty] public List<ObjectSearch> ObjectSearch { get; set; }
        [BindProperty] public List<ObjectSearch> UserSearch { get; set; }
        [BindProperty] public int ShowHidden { get; set; }
        [BindProperty] public int ShowAllTypes { get; set; }
        [BindProperty] public int ShowOrphans { get; set; }
        [BindProperty] public int SearchFilter { get; set; }
        [BindProperty] public string SearchField { get; set; }
        [BindProperty] public int SearchPage { get; set; }
        [BindProperty] public int PageSize { get; set; }
        [BindProperty] public string SearchString { get; set; }
        [BindProperty(SupportsGet = true)] public List<string> AppliedFilters { get; set; }
        public User PublicUser { get; set; }

        public ActionResult OnGet(string s, int f, int p, int h, int t, int o, string sf)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            // var is copied to SearchString so it can be rendered in html view.
            SearchString = s.Replace("'","").Replace("–", "-");
            SearchFilter = f;
            SearchField = "";
            if (sf != null) {
                SearchField = sf.Replace("%2C", ",").Replace("%20", " ").Replace("–", "-");
            }
            SearchPage = p;
            ShowHidden = h;
            ShowAllTypes = t;
            ShowOrphans = o;
            PageSize = 20;

            // if string is empty no results will be added.
            if (SearchString != null && SearchString.Length > 0)
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "Search";
                        command.Parameters.Add(new SqlParameter("@searchTerm", SearchString));
                        // 10 results per page
                        command.Parameters.Add(new SqlParameter("@pageSize", PageSize));

                        command.Parameters.Add(new SqlParameter("@showHidden", ShowHidden));
                        command.Parameters.Add(new SqlParameter("@showAllTypes", ShowAllTypes));
                        command.Parameters.Add(new SqlParameter("@showOrphans", ShowOrphans));

                        if(SearchField != null && SearchField.Length > 0)
                        {
                            command.Parameters.Add(new SqlParameter("@searchField", SearchField));
                        }
                        // if page is specified, get the specified page
                        if (SearchPage > 0)
                        {
                            command.Parameters.Add(new SqlParameter("@currentPage", p));
                        }

                        if (SearchFilter > 0)
                        {
                            command.Parameters.Add(new SqlParameter("@reportObjectTypes", String.Join(",", SearchFilter)));
                        }
                        connection.Open();
                        var datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            SearchResults.Add(new SearchResult
                            {
                                Id = (int)datareader["ItemId"],
                                Name = datareader["Name"].ToString(),
                                ItemType = datareader["ItemType"].ToString(),
                                SearchField = datareader["SearchField"].ToString(),
                                TotalRecords = (int)datareader["TotalRecords"],
                                EpicRecordId = datareader["EpicRecordId"].ToString(),
                                EpicMasterFile = datareader["EpicMasterFile"].ToString(),
                                SourceServer = datareader["SourceServer"].ToString(),
                                ReportServerPath = datareader["ReportServerPath"].ToString(),
                                Description = datareader["Description"].ToString(),
                                ReportType = datareader["ReportType"].ToString(),
                                Documented = (int)datareader["Documented"],
                                EpicReportTemplateId = datareader["EpicReportTemplateId"].ToString(),
                                ReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, datareader["ReportObjectURL"].ToString(),
                                                                        datareader["Name"].ToString(),
                                                                        datareader["ReportType"].ToString(),
                                                                        datareader["EpicReportTemplateId"].ToString(),
                                                                        datareader["EpicRecordId"].ToString(),
                                                                        datareader["EpicMasterFile"].ToString(),
                                                                        datareader["EnabledForHyperspace"].ToString()
                                                                        ),
                                ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(_config["AppSettings:org_domain"], HttpContext, datareader["ReportType"].ToString(), datareader["ReportServerPath"].ToString(), datareader["SourceServer"].ToString()),
                                EditReportUrl = HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, datareader["ReportServerPath"].ToString(), datareader["SourceServer"].ToString(), datareader["EpicMasterFile"].ToString(), datareader["EpicReportTemplateId"].ToString(), datareader["EpicRecordId"].ToString()),
                                Hidden = (int)datareader["Hidden"],
                                VisibleType = (int)datareader["VisibleType"],
                                Orphaned = (int)datareader["Orphaned"],
                            });
                        }
                    }
                }
            }
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            ViewData["Fullname"] = PublicUser.Fullname_Cust;
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Page();
        }

        public ActionResult OnPostReportSearch(string s, string e)
        {
            if (s != null)
                SearchString = s;
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            ObjectSearch.Add(new ObjectSearch
                            {
                                ObjectId = (int)datareader["ReportObjectId"],
                                Name = datareader["ReportObjectName"].ToString(),
                                Description = datareader["Description"].ToString(),
                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostTermSearch(string s, string e)
        {
            if (s != null)
                SearchString = s;//.Replace("'","''").Replace(";","_");
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            ObjectSearch.Add(new ObjectSearch
                            {
                                ObjectId = (int)datareader["TermId"],
                                Name = datareader["Name"].ToString(),
                                Description = datareader["Summary"].ToString(),

                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostProjectSearch(string s, string e)
        {
            if (s != null)
                SearchString = s;//.Replace("'","''").Replace(";","_");
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            ObjectSearch.Add(new ObjectSearch
                            {
                                ObjectId = (int)datareader["DataProjectId"],
                                Name = datareader["Name"].ToString(),
                                Description = datareader["Description"].ToString(),

                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(ObjectSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostUserSearch(string s, string e)
        {
            if (s != null)
                SearchString = s;//.Replace("'","''").Replace(";","_");
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            UserSearch.Add(new ObjectSearch
                            {
                                ObjectId = (int)datareader["Userid"],
                                Name = datareader["Username"].ToString(),
                                Type = datareader["S"].ToString()

                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostUserSearchMail(string s, string e)
        {
            if (s != null)
                SearchString = s;//.Replace("'","''").Replace(";","_");
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            UserSearch.Add(new ObjectSearch
                            {
                                ObjectId = (int)datareader["Userid"],
                                Name = datareader["Username"].ToString(),
                                Type = datareader["S"].ToString()

                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostDirector(string s, string e)
        {
            if (s != null)
                SearchString = s;//.Replace("'","''").Replace(";","_");
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DataGovernanceDatabase")))
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
                            UserSearch.Add(new ObjectSearch
                            {
                                Description = (string)datareader["Email"],
                                Name = datareader["FullName"].ToString()

                            });
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(UserSearch);
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
                return Content(json);
            }
        }

        public ActionResult OnPostValueList(string s)
        {

            List<SmallData> myObject = new List<SmallData>(){ new SmallData("0", "error")};

            switch (s)
            {
                case "org-value":
                    myObject = _cache.GetOrCreate<List<SmallData>>("org-value",
                       cacheEntry => {
                           cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                           return _context.OrganizationalValue.Select(x => new SmallData(x.OrganizationalValueId.ToString(), x.OrganizationalValueName)).ToList();
                       });
                    break;

                case "run-freq":
                    myObject = _cache.GetOrCreate<List<SmallData>>("run-freq",
                      cacheEntry => {
                          cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                          return _context.EstimatedRunFrequency.Select(x => new SmallData(x.EstimatedRunFrequencyId.ToString(), x.EstimatedRunFrequencyName)).ToList();
                      });
                    break;

                case "fragility":
                    myObject = _cache.GetOrCreate<List<SmallData>>("fragility",
                      cacheEntry => {
                          cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                          return _context.Fragility.Select(x => new SmallData(x.FragilityId.ToString(), x.FragilityName)).ToList();
                      });
                    break;

                case "maint-sched":
                    myObject = _cache.GetOrCreate<List<SmallData>>("maint-sched",
                      cacheEntry => {
                          cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                          return _context.MaintenanceSchedule.Select(x => new SmallData(x.MaintenanceScheduleId.ToString(), x.MaintenanceScheduleName)).ToList();
                      });
                    break;

                case "ro-fragility":
                    myObject = _cache.GetOrCreate<List<SmallData>>("ro-fragility",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.FragilityTag.Select(x => new SmallData(x.FragilityTagId.ToString(), x.FragilityTagName)).ToList();
                     });
                    break;

                case "maint-log-stat":
                    myObject = _cache.GetOrCreate<List<SmallData>>("maint-log-stat",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.MaintenanceLogStatus.Select(x => new SmallData(x.MaintenanceLogStatusId.ToString(), x.MaintenanceLogStatusName)).ToList();
                     });
                    break;

                case "ext-cont":
                    myObject = _cache.GetOrCreate<List<SmallData>>("ext-cont",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.DpContact.Select(x => new SmallData(x.ContactId.ToString(), x.Name + " - " + x.Company)).ToList();
                     });
                    break;

                case "mile-temp":
                    myObject = _cache.GetOrCreate<List<SmallData>>("mile-temp",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.DpMilestoneFrequency.Select(x => new SmallData(x.MilestoneTypeId.ToString(), x.Name)).ToList();
                     });
                    break;

                case "mile-type":
                    myObject = _cache.GetOrCreate<List<SmallData>>("mile-type",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.DpMilestoneTemplates.Select(x => new SmallData(x.MilestoneTemplateId.ToString(), x.Name)).ToList();
                     });
                    break;

                case "user-roles":
                    myObject = _cache.GetOrCreate<List<SmallData>>("user-roles",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.UserRoles.Select(x => new SmallData(x.UserRolesId.ToString(), x.Name)).ToList();
                     });
                    break;

                case "financial-impact":
                    myObject = _cache.GetOrCreate<List<SmallData>>("financial-impact",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.FinancialImpact.Select(x => new SmallData(x.FinancialImpactId.ToString(), x.Name)).ToList();
                     });
                    break;

                case "strategic-importance":
                    myObject = _cache.GetOrCreate<List<SmallData>>("strategic-importance",
                     cacheEntry => {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                         return _context.StrategicImportance.Select(x => new SmallData(x.StrategicImportanceId.ToString(), x.Name)).ToList();
                     });
                    break;
            }

            var json = JsonConvert.SerializeObject(myObject);
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Content(json);
        }
    }
}
