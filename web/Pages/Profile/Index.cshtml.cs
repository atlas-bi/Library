using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Atlas_Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public class TopUsersData
        {
            public string Username { get; set; }
            public string UserUrl { get; set; }
            public int Hits { get; set; }
            public double RunTime { get; set; }
            public string LastRun { get; set; }
        }

        public class RunTimeData
        {
            public string Date { get; set; }
            public double Avg { get; set; }
            public int Cnt { get; set; }
        }

        public class FailedRunsData
        {
            public string Date { get; set; }
            public string RunUser { get; set; }
            public string UserUrl { get; set; }
            public string RunStatus { get; set; }
        }

        public class SubscriptionData
        {
            public string UserUrl { get; set; }
            public string User { get; set; }
            public string Subscription { get; set; }
            public string InactiveFlags { get; set; }
            public string EmailList { get; set; }
            public string Description { get; set; }
            public string LastStatus { get; set; }
            public string LastRun { get; set; }
        }

        public class FavoritesData
        {
            public string UserUrl { get; set; }
            public string User { get; set; }
        }

        public IEnumerable<TopUsersData> TopUsers { get; set; }

        //public IEnumerable<RunTimeData> RunTime { get; set; }
        public IEnumerable<FailedRunsData> FailedRuns { get; set; }
        public IEnumerable<SubscriptionData> Subscriptions { get; set; }
        public IEnumerable<FavoritesData> ProfileFavorites { get; set; }

        public int ProfileId { get; set; }
        public string ProfileType { get; set; }

        public class BarData
        {
            public string Key { get; set; }
            public string Href { get; set; }
            public string TitleOne { get; set; }
            public string TitleTwo { get; set; }

            public double Count { get; set; }
            public double? Percent { get; set; }
        }

        public class RunHistoryData
        {
            public string Date { get; set; }
            public int Runs { get; set; }
            public int Users { get; set; }
            public int FailedRuns { get; set; }
            public double RunTime { get; set; }
        }

        public class Filters
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string FriendlyValue { get; set; }
            public int Count { get; set; }
        }

        public int Runs { get; set; }
        public int Users { get; set; }
        public double RunTime { get; set; }

        public List<User> UserStars { get; set; }
        public List<User> UserSubscriptions { get; set; }

        public List<RunHistoryData> RunHistory { get; set; }

        public List<BarData> BarDataSet { get; set; }

        public List<Filters> Filter_Server { get; set; }
        public List<Filters> Filter_Database { get; set; }
        public List<Filters> Filter_MasterFile { get; set; }
        public List<Filters> Filter_Visible { get; set; }
        public List<Filters> Filter_Certification { get; set; }
        public List<Filters> Filter_Availabiltiy { get; set; }
        public List<Filters> Filter_ReportType { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<ActionResult> OnGetFiltersAsync(
            double start_at = -604800, // last 7 days
            double end_at = 0,
            string server = "all",
            string database = "all",
            string masterFile = "all",
            string visible = "all",
            string certification = "all",
            string availability = "all",
            int reportType = -1
        )
        {
            var reports = _context.ReportObjects.Where(
                x =>
                    _context.ReportObjectRunData.Any(
                        r =>
                            r.RunStartTime >= DateTime.Now.AddSeconds(start_at)
                            && r.RunStartTime <= DateTime.Now.AddSeconds(end_at)
                    )
            );

            if (server != "all")
            {
                reports = reports.Where(x => x.SourceServer == server);
            }

            if (database != "all")
            {
                reports = reports.Where(x => x.SourceDb == database);
            }

            if (masterFile != "all")
            {
                if (masterFile == "None")
                {
                    reports = reports.Where(x => string.IsNullOrEmpty(x.EpicMasterFile));
                }
                else
                {
                    reports = reports.Where(x => x.EpicMasterFile == masterFile);
                }
            }

            if (visible != "all")
            {
                reports = reports.Where(x => x.DefaultVisibilityYn == visible);
            }

            if (certification != "all")
            {
                reports = reports.Where(x => x.CertificationTag == certification);
            }

            if (availability != "all")
            {
                reports = reports.Where(x => x.Availability == availability);
            }

            if (reportType != -1)
            {
                reports = reports.Where(x => x.ReportObjectTypeId == reportType);
            }

            Filter_Server = await _context.ReportObjects
                .GroupBy(x => x.SourceServer)
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "server",
                            Value = x.Key,
                            FriendlyValue = x.Key,
                            Count = reports.Count(c => c.SourceServer == x.Key)
                        }
                )
                .ToListAsync();

            Filter_Database = await _context.ReportObjects
                .GroupBy(x => x.SourceDb)
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "database",
                            Value = x.Key,
                            FriendlyValue = x.Key,
                            Count = reports.Count(c => c.SourceDb == x.Key)
                        }
                )
                .ToListAsync();

            Filter_ReportType = await _context.ReportObjects
                .GroupBy(x => new { x.ReportObjectTypeId, x.ReportObjectType.Name })
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "reportType",
                            Value = x.Key.ReportObjectTypeId.ToString(),
                            FriendlyValue = x.Key.Name,
                            Count = reports.Count(
                                c => c.ReportObjectTypeId == x.Key.ReportObjectTypeId
                            )
                        }
                )
                .ToListAsync();

            Filter_MasterFile = await _context.ReportObjects
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "masterFile",
                            Value = string.IsNullOrEmpty(x.EpicMasterFile)
                              ? "None"
                              : x.EpicMasterFile,
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.Value,
                            Count = reports.Count(
                                c =>
                                    (
                                        string.IsNullOrEmpty(c.EpicMasterFile)
                                          ? "None"
                                          : c.EpicMasterFile
                                    ) == x.Key.Value
                            )
                        }
                )
                .ToListAsync();

            Filter_Visible = await _context.ReportObjects
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "visible",
                            Value = x.DefaultVisibilityYn == "N" ? "N" : "Y",
                            FriendlyValue = x.DefaultVisibilityYn == "N" ? "No" : "Yes"
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.FriendlyValue,
                            Count = reports.Count(
                                c =>
                                    (
                                        string.IsNullOrEmpty(c.DefaultVisibilityYn)
                                          ? "Y"
                                          : c.DefaultVisibilityYn
                                    ) == x.Key.Value
                            )
                        }
                )
                .ToListAsync();

            Filter_Certification = await _context.ReportObjects
                .GroupBy(x => x.CertificationTag)
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "certification",
                            Value = x.Key,
                            FriendlyValue = x.Key,
                            Count = reports.Count(c => c.CertificationTag == x.Key)
                        }
                )
                .ToListAsync();

            Filter_Availabiltiy = await _context.ReportObjects
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "availability",
                            Value = string.IsNullOrEmpty(x.Availability) ? "Public" : x.Availability
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.Value,
                            Count = reports.Count(
                                c =>
                                    (
                                        string.IsNullOrEmpty(c.Availability)
                                          ? "Public"
                                          : c.Availability
                                    ) == x.Key.Value
                            )
                        }
                )
                .ToListAsync();
            return new PartialViewResult { ViewName = "Partials/_Filters", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetChartAsync(
            int id,
            string type,
            double start_at = -604800, // last 7 days
            double end_at = 0,
            List<string> server = null,
            List<string> database = null,
            List<string> masterFile = null,
            List<string> visible = null,
            List<string> certification = null,
            List<string> availability = null,
            List<int> reportType = null
        )
        {
            /*
            when start - end < 2days, use 1 AM, 2 AM...
            when start - end < 8 days use  Sun 3/20, Mon 3/21...
            when start - end < 365 days use Mar 1, Mar 2 ...
            when start - end > 365 days use Jan, Feb ...

            when using all time, get first day and last day and use the above rules
            */
            DateTime MinDate = new DateTime(1900, 01, 01, 00, 00, 00);
            var subquery = _context.ReportObjectRunData.Where(
                x =>
                    x.RunStartTime >= DateTime.Now.AddSeconds(start_at)
                    && x.RunStartTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                subquery = subquery.Where(x => x.ReportObjectId == id);
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.ReportObjectDocTerms
                            .Where(t => t.TermId == id)
                            .Select(t => t.ReportObjectId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "collection" && _context.Collections.Any(x => x.DataProjectId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.CollectionReports
                            .Where(c => c.DataProjectId == id)
                            .Select(c => c.ReportId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "report" && id == -1)
            {
                if (server.Any())
                {
                    subquery = subquery.Where(x => server.Contains(x.ReportObject.SourceServer));
                }

                if (database.Any())
                {
                    subquery = subquery.Where(x => database.Contains(x.ReportObject.SourceDb));
                }

                if (masterFile.Any())
                {
                    subquery = subquery.Where(
                        x =>
                            masterFile.Contains(x.ReportObject.EpicMasterFile)
                            || masterFile.Contains("None")
                                && string.IsNullOrEmpty(x.ReportObject.EpicMasterFile)
                    );
                }

                if (visible.Any())
                {
                    subquery = subquery.Where(
                        x =>
                            visible.Contains(x.ReportObject.DefaultVisibilityYn)
                            || (
                                visible.Contains("Y")
                                && string.IsNullOrEmpty(x.ReportObject.DefaultVisibilityYn)
                            )
                    );
                }

                if (certification.Any())
                {
                    subquery = subquery.Where(
                        x => certification.Contains(x.ReportObject.CertificationTag)
                    );
                }

                if (availability.Any())
                {
                    subquery = subquery.Where(
                        x =>
                            availability.Contains(x.ReportObject.Availability)
                            || (
                                availability.Contains("Public")
                                && string.IsNullOrEmpty(x.ReportObject.Availability)
                            )
                    );
                }

                if (reportType.Any())
                {
                    subquery = subquery.Where(
                        x => reportType.Contains((int)x.ReportObject.ReportObjectTypeId)
                    );
                }
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            // if (groupId > 0 && _context.UserGroups.Any(x => x.GroupId == groupId))
            // {
            //     subquery = subquery.Where(
            //         x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
            //     );
            // }

            switch (end_at - start_at)
            {
                // for < 2 days
                // 1 AM, 2 AM etc..
                default:
                case < 172800:
                    RunHistory = await (
                        from a in subquery
                        // in linqpad, use SqlMethods.DateDiffHour instead of EF.Functions.DateDiffHour
                        group a by MinDate.AddHours(
                            EF.Functions.DateDiffHour(MinDate, (a.RunStartTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new RunHistoryData
                        {
                            Date = grp.Key.ToString("h tt"),
                            Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                            Runs = grp.Count(),
                            FailedRuns = grp.Count(x => x.RunStatus != "Success"),
                            RunTime = Math.Round(grp.Average(x => (int)x.RunDurationSeconds), 1)
                        }
                    ).ToListAsync();

                    break;
                // for < 8 days
                //  Sun 3/20, Mon 3/21...
                case < 691200:
                    RunHistory = await (
                        from a in subquery
                        group a by MinDate.AddDays(
                            EF.Functions.DateDiffDay(MinDate, (a.RunStartTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new RunHistoryData
                        {
                            Date = grp.Key.ToString("ddd M/d"),
                            Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                            Runs = grp.Count(),
                            FailedRuns = grp.Count(x => x.RunStatus != "Success"),
                            RunTime = Math.Round(grp.Average(x => (int)x.RunDurationSeconds), 1)
                        }
                    ).ToListAsync();
                    break;
                // for < 365 days
                // Mar 1, Mar 2
                case < 31536000:
                    RunHistory = await (
                        from a in subquery
                        group a by MinDate.AddDays(
                            EF.Functions.DateDiffDay(MinDate, (a.RunStartTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new RunHistoryData
                        {
                            Date = grp.Key.ToString("MMM d"),
                            Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                            Runs = grp.Count(),
                            FailedRuns = grp.Count(x => x.RunStatus != "Success"),
                            RunTime = Math.Round(grp.Average(x => (int)x.RunDurationSeconds), 1)
                        }
                    ).ToListAsync();
                    break;
                case >= 31536000:
                    RunHistory = await (
                        from a in subquery
                        group a by MinDate.AddMonths(
                            EF.Functions.DateDiffMonth(MinDate, (a.RunStartTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new RunHistoryData
                        {
                            Date = grp.Key.ToString("MMM yy"),
                            Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                            Runs = grp.Count(),
                            FailedRuns = grp.Count(x => x.RunStatus != "Success"),
                            RunTime = Math.Round(grp.Average(x => (int)x.RunDurationSeconds), 1)
                        }
                    ).ToListAsync();
                    break;
            }

            Runs = subquery.Count();

            Users = subquery.Select(x => x.RunUserId).Distinct().Count();

            if (Runs > 0)
            {
                RunTime = Math.Round(subquery.Average(x => (int)x.RunDurationSeconds), 2);
            }
            else
            {
                RunTime = 0;
            }

            return new PartialViewResult { ViewName = "Partials/_Chart", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetUsersAsync(
            int id,
            string type,
            double start_at = -604800, // last 7 days
            double end_at = 0
        )
        {
            var subquery = _context.ReportObjectRunData.Where(
                x =>
                    x.RunStartTime >= DateTime.Now.AddSeconds(start_at)
                    && x.RunStartTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                subquery = subquery.Where(x => x.ReportObjectId == id);
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.ReportObjectDocTerms
                            .Where(t => t.TermId == id)
                            .Select(t => t.ReportObjectId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "collection" && _context.Collections.Any(x => x.DataProjectId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.CollectionReports
                            .Where(c => c.DataProjectId == id)
                            .Select(c => c.ReportId)
                            .Contains(x.ReportObjectId)
                );
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            double total = subquery.Count();

            BarDataSet = await (
                from a in subquery
                group a by new { a.RunUserId, a.RunUser.FullnameCalc } into grp
                select new BarData
                {
                    Key = grp.Key.FullnameCalc,
                    Count = grp.Count(),
                    Percent = (double)grp.Count() / total,
                    TitleOne = "Top Users",
                    TitleTwo = "Runs",
                    Href =
                        (
                            _config["features:enable_user_profile"] == null
                            || _config["features:enable_user_profile"].ToString().ToLower()
                                == "true"
                        )
                            ? "/users?id=" + grp.Key.RunUserId
                            : null,
                }
            ).OrderByDescending(x => x.Count).Take(20).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetFailsAsync(
            int id,
            string type,
            double start_at = -604800, // last 7 days
            double end_at = 0
        )
        {
            var subquery = _context.ReportObjectRunData.Where(
                x =>
                    x.RunStartTime >= DateTime.Now.AddSeconds(start_at)
                    && x.RunStartTime <= DateTime.Now.AddSeconds(end_at)
                    && x.RunStatus != "Success"
            );

            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                subquery = subquery.Where(x => x.ReportObjectId == id);
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.ReportObjectDocTerms
                            .Where(t => t.TermId == id)
                            .Select(t => t.ReportObjectId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "collection" && _context.Collections.Any(x => x.DataProjectId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.CollectionReports
                            .Where(c => c.DataProjectId == id)
                            .Select(c => c.ReportId)
                            .Contains(x.ReportObjectId)
                );
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            double total = subquery.Count();

            BarDataSet = await (
                from a in subquery
                group a by a.RunStatus into grp
                select new BarData
                {
                    Key = Regex.Replace(
                        Regex.Replace(grp.Key, @"^rs", "", RegexOptions.Multiline),
                        @"(?<=[a-z])([A-Z])",
                        " $1"
                    ),
                    Count = grp.Count(),
                    Percent = (double)grp.Count() / total,
                    TitleOne = "Failed Runs",
                    TitleTwo = "Fails"
                }
            ).OrderByDescending(x => x.Count).Take(20).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetReportsAsync(
            int id,
            string type,
            double start_at = -604800, // last 7 days
            double end_at = 0
        )
        {
            var subquery = _context.ReportObjectRunData.Where(
                x =>
                    x.RunStartTime >= DateTime.Now.AddSeconds(start_at)
                    && x.RunStartTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.ReportObjectDocTerms
                            .Where(t => t.TermId == id)
                            .Select(t => t.ReportObjectId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "collection" && _context.Collections.Any(x => x.DataProjectId == id))
            {
                subquery = subquery.Where(
                    x =>
                        _context.CollectionReports
                            .Where(c => c.DataProjectId == id)
                            .Select(c => c.ReportId)
                            .Contains(x.ReportObjectId)
                );
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            double total = subquery.Count();

            BarDataSet = await (
                from a in subquery
                group a by new
                {
                    a.ReportObjectId,
                    name = string.IsNullOrEmpty(a.ReportObject.DisplayTitle)
                      ? a.ReportObject.Name
                      : a.ReportObject.DisplayTitle
                } into grp
                select new BarData
                {
                    Key = grp.Key.name,
                    Count = grp.Count(),
                    Percent = (double)grp.Count() / total,
                    TitleOne = "Top Reports",
                    TitleTwo = "Runs",
                    Href = "/reports?id=" + grp.Key.ReportObjectId
                }
            ).OrderByDescending(x => x.Count).Take(20).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetStarsAsync(int id, string type)
        {
            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredReports.Any(r => r.Reportid == id))
                    .ToListAsync();
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredTerms.Any(r => r.Termid == id))
                    .ToListAsync();
            }
            else if (type == "collection" && _context.Collections.Any(x => x.DataProjectId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredCollections.Any(r => r.Collectionid == id))
                    .ToListAsync();
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            return new PartialViewResult { ViewName = "Partials/_Stars", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetSubscriptionsAsync(int id, string type)
        {
            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                UserSubscriptions = await _context.Users
                    .Where(x => x.ReportObjectSubscriptions.Any(r => r.ReportObjectId == id))
                    .ToListAsync();
            }
            else
            {
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
            }

            return new PartialViewResult
            {
                ViewName = "Partials/_Subscriptions",
                ViewData = ViewData
            };
        }
    }
}
