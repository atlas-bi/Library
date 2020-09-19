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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data_Governance_WebApp.Models;
using Data_Governance_WebApp.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.Tasks
{
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
        public class UndocumentedReports
        {
            public int ReportObjectId { get; set; }
            public string ModifiedBy { get; set; }
            public string Name { get; set; }
            public string ReportType { get; set; }
            public int Runs { get; set; }
            public string LastMaintained { get; set; }
            public string LastRun { get; set; }
            public string Favorite { get; set; }
        }

        public class NextMaint
        {
            public int ReportId { get; set; }
            public string Date { get; set; }
            public string Name { get; set; }
            public string User { get; set; }
        }

        public class RecommendRetireReports
        {
            public string Name { get; set; }
            public DateTime? MaintenanceDate { get; set; }
            public string MaintenanceDateString { get; set; }
            public int ReportId { get; set; }
            public string Comment { get; set; }
            public string FullName { get; set; }
        }

        public class EditedOutsideAnalyticsData
        {
            public string ReportUrl { get; set; }
            public string LastMod { get; set; }
            public string Author { get; set; }
            public string ModifiedBy { get; set; }
            public string Name { get; set; }
            public string ReportType { get; set; }
            public string Epic { get; set; }
            public string RunReportUrl { get; set; }
            public string EditReportUrl { get; set; }
            public string RecordViewerUrl { get; set; }
            public int Runs { get; set; }
        }

        public class DeadData
        {
            public string ReportUrl { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string ModifiedBy { get; set; }
            public string LastMod { get; set; }
            public string Server { get; set; }
            public string MasterFile { get; set; }
            public string EpicId { get; set; }
        }

        public List<UserFavorites> Favorites { get; set; }
        public List<int?> Permissions { get; set; }
        public IEnumerable<RecommendRetireReports> RecommendRetire { get; set; }
        public IEnumerable<NextMaint> NextMaintenance { get; set; }
        public List<NextMaint> AuditOnly { get; set; }
        public List<NextMaint> MissingSchedule { get; set; }
        public IEnumerable<UndocumentedReports> Undocumented { get; set; }
        public IEnumerable<UndocumentedReports> NewUndocumented { get; set; }
        public IEnumerable<EditedOutsideAnalyticsData> EditedOutsideAnalytics { get; set; }
        public IEnumerable<DeadData> Dead { get; set; }
        public User PublicUser { get; set; }
        public List<AdList> AdLists { get; set; }
        public IActionResult OnGet(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                new AdList { Url = "Reports/?handler=RelatedReports&id="+id, Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentProjects", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Page();
        }
        public async Task<IActionResult> OnGetRecommendRetire()
        {
            ViewData["RecommendRetire"] = await (from l in _context.ReportObjectDocMaintenanceLogs
                                     join m in _context.MaintenanceLog on l.MaintenanceLogId equals m.MaintenanceLogId
                                     where m.MaintenanceLogStatus.MaintenanceLogStatusName == "Recommend Retire"
                                        && l.ReportObject.ExecutiveVisibilityYn == "Y"
                                     select new RecommendRetireReports
                                     {
                                         FullName = m.Maintainer.Fullname_Cust,
                                         Name = l.ReportObject.ReportObject.Name,
                                         MaintenanceDate = m.MaintenanceDate,
                                         MaintenanceDateString = m.MaintenanceDateDisplayString,
                                         ReportId = l.ReportObjectId,
                                         Comment = m.Comment
                                     }).ToListAsync();

            return Partial("Partials/_RecommendRetire");
        }
        public async Task<IActionResult> OnGetUnused()
        {
            ViewData["Dead"] = await (from r in _context.ReportObject.Where(x => (x.ReportObjectTypeId == 3 || x.ReportObjectTypeId == 17 || x.ReportObjectTypeId == 20 || x.ReportObjectTypeId == 28)
                                                                && x.DefaultVisibilityYn == "Y"
                                                                && x.OrphanedReportObjectYn.ToString() == "N")
                          join d in _context.ReportObjectRunData on r.ReportObjectId equals d.ReportObjectId into dta
                          from l in dta.DefaultIfEmpty()
                          where l.ReportObjectId == null
                          join doc in _context.ReportObjectDoc on r.ReportObjectId equals doc.ReportObjectId into doc_dta
                          from bla in doc_dta.DefaultIfEmpty()
                          where bla.Hidden.ToString() == "N" || bla.Hidden == null
                          where r.LastModifiedDate < DateTime.Now.AddMonths(-2) || r.LastModifiedDate == null
                          orderby r.LastModifiedDate ascending
                          select new DeadData
                          {
                              ReportUrl = "\\Reports?id=" + r.ReportObjectId,
                              Name = r.Name,
                              Type = r.ReportObjectType.Name,
                              ModifiedBy = r.LastModifiedByUser.Fullname_Cust,
                              LastMod = r.LastUpdatedDateDisplayString,
                              Server = r.SourceServer,
                              MasterFile = r.EpicMasterFile,
                              EpicId = r.EpicRecordId.ToString()
                                }).Take(30).ToListAsync();

            return Partial("Partials/_Unused");
        }
        public async Task<IActionResult> OnGetMaintRequired()
        {
            DateTime Today = DateTime.Now;
            ViewData["NextMaintenance"] = await (from n in (
                                    from d in _context.ReportObjectDoc
                                    where d.MaintenanceScheduleId != 5
                                       && d.MaintenanceScheduleId != null
                                       && d.ReportObject.DefaultVisibilityYn == "Y"
                                       && d.ReportObject.OrphanedReportObjectYn.ToString() == "N"
                                    join l in (
                                        from l in _context.MaintenanceLog
                                        join m in _context.ReportObjectDocMaintenanceLogs on l.MaintenanceLogId equals m.MaintenanceLogId
                                        group m by m.ReportObjectId into grp
                                        select new
                                        {
                                            ReportObjectId = grp.Key,
                                            MaintenanceLogId = grp.Max(x => x.MaintenanceLogId)
                                        }
                                    ) on d.ReportObjectId equals l.ReportObjectId into tmp
                                    from t in tmp.DefaultIfEmpty()

                                    join m in _context.MaintenanceLog on t.MaintenanceLogId equals m.MaintenanceLogId into tmptwo
                                    from ttwo in tmptwo.DefaultIfEmpty()
                                    select new
                                    {
                                        d.ReportObjectId,
                                        NextDate = d.MaintenanceScheduleId == 1 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddMonths(3) : // quarterly
                                                            d.MaintenanceScheduleId == 2 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddMonths(6) : // twice a year
                                                            d.MaintenanceScheduleId == 3 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddYears(1) : // yearly
                                                            d.MaintenanceScheduleId == 4 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddYears(2) :// every two years
                                                            (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? d.CreatedDateTime ?? Today),
                                        d.ReportObject.Name,
                                        LastUser = (
                                              ttwo.Maintainer.Fullname_Cust != "user not found" ? ttwo.Maintainer.Fullname_Cust :
                                              d.UpdatedByNavigation.Fullname_Cust)
                                    }
)
                                     where n.NextDate < Today.AddMonths(2)
                                     orderby n.NextDate
                                     select new NextMaint
                                     {
                                         ReportId = n.ReportObjectId,
                                         Date = n.NextDate.ToString("MM/dd/yyyy"),
                                         Name = n.Name,
                                         User = n.LastUser
                                     }
                                     ).ToListAsync();

            return Partial("Partials/_MaintRequired");
        }
        public async Task<IActionResult> OnGetAudit()
        {
            DateTime Today = DateTime.Now;
            ViewData["AuditOnly"] = await (from n in (
                                    from d in _context.ReportObjectDoc
                                    where d.MaintenanceScheduleId == 5
                                       && d.ReportObject.DefaultVisibilityYn == "Y"
                                       && d.ReportObject.OrphanedReportObjectYn.ToString() == "N"
                                    join l in (
                                        from l in _context.MaintenanceLog
                                        join m in _context.ReportObjectDocMaintenanceLogs on l.MaintenanceLogId equals m.MaintenanceLogId
                                        group m by m.ReportObjectId into grp
                                        select new
                                        {
                                            ReportObjectId = grp.Key,
                                            MaintenanceLogId = grp.Max(x => x.MaintenanceLogId)
                                        }
                                    ) on d.ReportObjectId equals l.ReportObjectId into tmp
                                    from t in tmp.DefaultIfEmpty()

                                    join m in _context.MaintenanceLog on t.MaintenanceLogId equals m.MaintenanceLogId into tmptwo
                                    from ttwo in tmptwo.DefaultIfEmpty()
                                    select new
                                    {
                                        d.ReportObjectId,
                                        NextDate = d.MaintenanceScheduleId == 1 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddMonths(3) : // quarterly
                                                            d.MaintenanceScheduleId == 2 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddMonths(6) : // twice a year
                                                            d.MaintenanceScheduleId == 3 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddYears(1) : // yearly
                                                            d.MaintenanceScheduleId == 4 ? (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today).AddYears(2) :// every two years
                                                            (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? d.CreatedDateTime ?? Today),
                                        d.ReportObject.Name,
                                        LastUser = (
                                              ttwo.Maintainer.Fullname_Cust != "user not found" ? ttwo.Maintainer.Fullname_Cust :
                                              d.UpdatedByNavigation.Fullname_Cust)
                                    }
)
                               where n.NextDate < Today.AddMonths(2)
                               orderby n.NextDate
                               select new NextMaint
                               {
                                   ReportId = n.ReportObjectId,
                                   Date = n.NextDate.ToString("MM/dd/yyyy"),
                                   Name = n.Name,
                                   User = n.LastUser
                               }
                               ).ToListAsync();

            return Partial("Partials/_Audit");
        }
        public async Task<IActionResult> OnGetNoSchedule()
        {
            DateTime Today = DateTime.Now;
            ViewData["MissingSchedule"] = await (from n in (
                                    from d in _context.ReportObjectDoc
                                    where d.MaintenanceScheduleId == null
                                       && d.ReportObject.DefaultVisibilityYn == "Y"
                                       && d.ReportObject.OrphanedReportObjectYn.ToString() == "N"
                                    join l in (
                                        from l in _context.MaintenanceLog
                                        join m in _context.ReportObjectDocMaintenanceLogs on l.MaintenanceLogId equals m.MaintenanceLogId
                                        group m by m.ReportObjectId into grp
                                        select new
                                        {
                                            ReportObjectId = grp.Key,
                                            MaintenanceLogId = grp.Max(x => x.MaintenanceLogId)
                                        }
                                    ) on d.ReportObjectId equals l.ReportObjectId into tmp
                                    from t in tmp.DefaultIfEmpty()

                                    join m in _context.MaintenanceLog on t.MaintenanceLogId equals m.MaintenanceLogId into tmptwo
                                    from ttwo in tmptwo.DefaultIfEmpty()
                                    select new
                                    {
                                        d.ReportObjectId,
                                        NextDate = (ttwo.MaintenanceDate ?? d.LastUpdateDateTime ?? Today),
                                        d.ReportObject.Name,
                                        LastUser = (
                                              ttwo.Maintainer.Fullname_Cust != "user not found" ? ttwo.Maintainer.Fullname_Cust :
                                              d.UpdatedByNavigation.Fullname_Cust)
                                    }
)
                                     where n.NextDate < Today.AddMonths(2)
                                     orderby n.NextDate
                                     select new NextMaint
                                     {
                                         ReportId = n.ReportObjectId,
                                         Date = n.NextDate.ToString("MM/dd/yyyy"),
                                         Name = n.Name,
                                         User = n.LastUser
                                     }
                               ).ToListAsync();

            return Partial("Partials/_NoSchedule");
        }
        public async Task<IActionResult> OnGetNotAnalytics()
        {
            ViewData["EditedOutsideAnalytics"] = await (from r in _context.ReportObject
                                            where r.LastModifiedDate > DateTime.Today.AddMonths(-6)
                                                  && r.DefaultVisibilityYn == "Y"
                                               && r.OrphanedReportObjectYn.ToString() == "N"
                                               && (r.ReportObjectTypeId == 17 || r.ReportObjectTypeId == 3)
                                            join l in _context.UserRoleLinks on r.LastModifiedByUserId equals l.UserId into tmp
                                            from t in tmp.DefaultIfEmpty()
                                            where ((int?)t.UserRolesId ?? 2) != 1
                                            join l in _context.UserRoleLinks on r.AuthorUserId equals l.UserId into tmptwo
                                            from ttwp in tmptwo.DefaultIfEmpty()
                                            where ((int?)ttwp.UserRolesId ?? 2) != 1
                                            join f in (from tr in _context.ReportObjectTopRuns
                                                       group tr by tr.ReportObjectId into g
                                                       select new { ReportObjectId = g.Key, Cnt = g.Count() }) on r.ReportObjectId equals f.ReportObjectId
                                            orderby r.ReportObjectId
                                            select new EditedOutsideAnalyticsData
                                            {
                                                ReportUrl = "\\Reports?id=" + r.ReportObjectId,
                                                LastMod = r.LastUpdatedDateDisplayString,
                                                Author = r.AuthorUser.Fullname_Cust,
                                                ModifiedBy = r.LastModifiedByUser.Fullname_Cust,
                                                Name = r.Name,
                                                ReportType = r.ReportObjectType.Name,
                                                Epic = r.EpicMasterFile + " " + r.EpicRecordId,
                                                RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, r.ReportObjectUrl, r.Name, r.ReportObjectType.Name, r.EpicReportTemplateId.ToString(), r.EpicRecordId.ToString(), r.EpicMasterFile, r.ReportObjectDoc.EnabledForHyperspace),
                                                EditReportUrl = Helpers.HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, r.ReportServerPath, r.SourceServer, r.EpicMasterFile, r.EpicReportTemplateId.ToString(), r.EpicRecordId.ToString()),
                                                RecordViewerUrl = Helpers.HtmlHelpers.RecordViewerLink(_config["AppSettings:org_domain"], HttpContext, r.EpicMasterFile, r.EpicRecordId.ToString()),
                                                Runs = ((int?)f.Cnt ?? 0)
                                            }).ToListAsync();

            return Partial("Partials/_NotAnalytics");
        }
        public async Task<IActionResult> OnGetTopUndocumented()
        {
            var rpts = new int[] { 17, 28, 3, 20 };
            ViewData["Undocumented"] = await (from r in _context.ReportObject
                                 join t in _context.ReportObjectType on r.ReportObjectTypeId equals t.ReportObjectTypeId
                                 where rpts.Contains(r.ReportObjectTypeId ?? 0)
                                    && r.DefaultVisibilityYn == "Y"
                                 select new
                                 {
                                     r.ReportObjectId,
                                     ModifiedBy = (
                                              r.LastModifiedByUser.Fullname_Cust != "user not found" ? r.LastModifiedByUser.Fullname_Cust:
                                              r.AuthorUser.Fullname_Cust),
                                     r.Name,
                                     ReportType = (t.Name == "Reporting Workbench Report" ? "Workbench" :
                                                     t.Name == "Source Radar Dashboard" ? "Dashboard" :
                                                     t.Name == "Epic-Crystal Report" ? "Crystal" : "SSRS"),
                                     Runs = r.ReportObjectRunData.Count(),
                                     LastMaintained = (r.LastModifiedDate ?? DateTime.Today.AddYears(-1)),
                                     LastRun = (r.ReportObjectRunData.Max(x => x.RunStartTime) ?? DateTime.Now),
                                     Favs = (from f in _context.UserFavorites
                                             where f.ItemType.ToLower() == "report"
                                                && f.ItemId == r.ReportObjectId
                                             select new { f.ItemId }).FirstOrDefault()

                                 }
                                into tmp
                                 join o in _context.ReportObjectDoc on tmp.ReportObjectId equals o.ReportObjectId into rs
                                 from p in rs.DefaultIfEmpty()

                                 where p.DeveloperDescription == null
                                  orderby tmp.Runs descending
                                  select new UndocumentedReports
                                 {
                                     ReportObjectId =tmp.ReportObjectId,
                                     ModifiedBy = tmp.ModifiedBy,
                                     Name = tmp.Name,
                                     ReportType = tmp.ReportType,
                                     Runs = tmp.Runs,
                                     Favorite = tmp.Favs == null ? "" : "Yes",
                                     LastMaintained = tmp.LastMaintained.ToString("MM/dd/yyyy"),
                                     LastRun = tmp.LastRun.ToString("MM/dd/yyyy")
                                 }).Take(60).ToListAsync();


            return Partial("Partials/_TopUndocumented");
        }
        public async Task<IActionResult> OnGetNewUndocumented()
        {
            var rpts = new int[] { 17, 28, 3, 20 };
            ViewData["NewUndocumented"] = await (from r in _context.ReportObject
                                  join t in _context.ReportObjectType on r.ReportObjectTypeId equals t.ReportObjectTypeId
                                  where rpts.Contains(r.ReportObjectTypeId ?? 0)
                                     && r.DefaultVisibilityYn == "Y"
                                     && r.LastModifiedDate > DateTime.Today.AddMonths(-1)
                                  select new
                                  {
                                      r.ReportObjectId,
                                      ModifiedBy = (
                                               r.LastModifiedByUser.Fullname_Cust != "user not found" ? r.LastModifiedByUser.Fullname_Cust :
                                               r.AuthorUser.Fullname_Cust),
                                      r.Name,
                                      ReportType = (t.Name == "Reporting Workbench Report" ? "Workbench" :
                                                      t.Name == "Source Radar Dashboard" ? "Dashboard" :
                                                      t.Name == "Epic-Crystal Report" ? "Crystal" : "SSRS"),
                                      Runs = r.ReportObjectRunData.Count(),
                                      LastMaintained = (r.LastModifiedDate ?? DateTime.Today.AddYears(-1)),
                                      LastRun = (r.ReportObjectRunData.Max(x => x.RunStartTime) ?? DateTime.Now),
                                      Favs = (from f in _context.UserFavorites
                                              where f.ItemType.ToLower() == "report"
                                                 && f.ItemId == r.ReportObjectId
                                              select new { f.ItemId }).FirstOrDefault()

                                  }
                                into tmp
                                  join o in _context.ReportObjectDoc on tmp.ReportObjectId equals o.ReportObjectId into rs
                                  from p in rs.DefaultIfEmpty()

                                  where p.DeveloperDescription == null
                                  orderby tmp.Runs descending
                                  select new UndocumentedReports
                                  {
                                      ReportObjectId = tmp.ReportObjectId,
                                      ModifiedBy = tmp.ModifiedBy,
                                      Name = tmp.Name,
                                      ReportType = tmp.ReportType,
                                      Runs = tmp.Runs,
                                      Favorite = tmp.Favs == null ? "" : "Yes",
                                      LastMaintained = tmp.LastMaintained.ToString("MM/dd/yyyy"),
                                      LastRun = tmp.LastRun.ToString("MM/dd/yyyy")
                                  }).Take(60).ToListAsync();



            return Partial("Partials/_NewUndocumented");
        }
    }
}