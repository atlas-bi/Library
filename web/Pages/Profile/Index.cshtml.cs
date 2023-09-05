using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Text.RegularExpressions;

#pragma warning disable S1481
namespace Atlas_Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        private readonly IConfiguration _config;

        public IndexModel(Atlas_WebContext context, IConfiguration config)
        {
            _context = context;

            _config = config;
        }

        public class SubqueryData
        {
            public ReportObject r { get; set; }
            public ReportObjectRunData d { get; set; }
            public ReportObjectRunDataBridge b { get; set; }
        }

        public class BarData
        {
            public string Key { get; set; }
            public string Href { get; set; }
            public string TitleOne { get; set; }
            public string TitleTwo { get; set; }
            public string Date { get; set; }
            public string DateTitle { get; set; }
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
            public bool Checked { get; set; }
        }

        public int Runs { get; set; }
        public int Users { get; set; }
        public double RunTime { get; set; }

        public class RunListData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public int Runs { get; set; }
            public string LastRun { get; set; }
        }

        public List<User> UserStars { get; set; }
        public List<ReportObjectSubscription> UserSubscriptions { get; set; }

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
            ViewData["DefaultReportTypes"] = await _context.ReportObjectTypes
                .Where(v => v.Visible == "Y")
                .Select(x => x.ReportObjectTypeId)
                .ToListAsync();
            return Page();
        }

        public async Task<ActionResult> OnGetRunListAsync(
            int id = -1,
            string type = "user",
            List<int> reportType = null
        )
        {
            var run_data = _context.ReportObjectRunDatas.AsQueryable();

            if (type == "report")
            {
                ViewData["ReportRuns"] = await (
                    from b in run_data
                    from d in b.ReportObjectRunDataBridges
                    where d.ReportObjectId == id
                    group new { b, d } by new { b.RunUserId, b.RunUser.FullnameCalc } into grp
                    orderby grp.Max(x => x.b.RunStartTime) descending
                    select new RunListData
                    {
                        Name = grp.Key.FullnameCalc,
                        Url =
                            (
                                _config["features:enable_user_profile"] == null
                                || _config["features:enable_user_profile"].ToString().ToLower()
                                    == "true"
                            )
                                ? $"\\users?id={grp.Key.RunUserId}"
                                : null,
                        Runs = grp.Sum(x => x.d.Runs),
                        LastRun = grp.Max(x => x.b.RunStartTime).ToShortDateString(),
                    }
                ).AsNoTracking().ToListAsync();

                return new PartialViewResult()
                {
                    ViewName = "Partials/_RunList",
                    ViewData = ViewData
                };
            }

            if (type == "user")
            {
                run_data = run_data.Where(x => x.RunUserId == id);
            }
            else if (type == "group")
            {
                run_data = run_data.Where(
                    x => x.RunUser.UserGroupsMemberships.Any(g => g.GroupId == id)
                );
            }

            var reports = _context.ReportObjects.AsQueryable();

            if (reportType.Count > 0)
            {
                reports = reports.Where(x => reportType.Contains((int)x.ReportObjectTypeId));
            }

            ViewData["ReportRuns"] = await (
                from r in reports
                join b in _context.ReportObjectRunDataBridges
                    on r.ReportObjectId equals b.ReportObjectId
                join d in run_data on b.RunId equals d.RunDataId
                where b.Inherited == 0
                group new SubqueryData { d = d, b = b } by new
                {
                    r.ReportObjectId,
                    Name = string.IsNullOrEmpty(r.DisplayTitle) ? r.Name : r.DisplayTitle,
                    r.ReportObjectType.ShortName,
                    ReportTypeName = r.ReportObjectType.Name
                } into grp
                orderby grp.Max(x => x.d.RunStartTime) descending
                select new RunListData
                {
                    Name = grp.Key.Name,
                    Type = string.IsNullOrEmpty(grp.Key.ShortName)
                        ? grp.Key.ReportTypeName
                        : grp.Key.ShortName,
                    Url = $"\\reports?id={grp.Key.ReportObjectId}",
                    Runs = grp.Sum(x => x.b.Runs),
                    LastRun = grp.Max(x => x.d.RunStartTime).ToShortDateString(),
                }
            ).AsNoTracking().ToListAsync();

            return new PartialViewResult() { ViewName = "Partials/_RunList", ViewData = ViewData };
        }

        public Tuple<
            IQueryable<SubqueryData>,
            IQueryable<IGrouping<DateTime, SubqueryData>>,
            string
        > BuildSubqueries(
            int id = -1,
            string type = "report",
            double start_at = -31536000, // last 12 months
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
            var start = DateTime.Now.AddSeconds(start_at);
            var end = DateTime.Now.AddSeconds(end_at);

            // start building two subqueries
            var subquery_rundata = _context.ReportObjectRunDatas.AsQueryable();
            var subquery_reports = _context.ReportObjects.AsQueryable();

            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                subquery_reports = subquery_reports.Where(x => x.ReportObjectId == id);
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                subquery_reports = subquery_reports.Where(
                    x =>
                        _context.ReportObjectDocTerms
                            .Where(t => t.TermId == id)
                            .Select(t => t.ReportObjectId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (type == "collection" && _context.Collections.Any(x => x.CollectionId == id))
            {
                subquery_reports = subquery_reports.Where(
                    x =>
                        _context.CollectionReports
                            .Where(c => c.CollectionId == id)
                            .Select(c => c.ReportId)
                            .Contains(x.ReportObjectId)
                );
            }
            else if (
                (type == "report" && id == -1)
                || (type == "user" && _context.Users.Any(x => x.UserId == id))
                || (type == "group" && _context.UserGroups.Any(x => x.GroupId == id))
            )
            {
                if (type == "user")
                {
                    subquery_rundata = subquery_rundata.Where(x => x.RunUserId == id);
                }
                else if (type == "group")
                {
                    subquery_rundata = subquery_rundata.Where(
                        x => x.RunUser.UserGroupsMemberships.Any(g => g.GroupId == id)
                    );
                }

                if (server.Any())
                {
                    subquery_reports = subquery_reports.Where(x => server.Contains(x.SourceServer));
                }

                if (database.Any())
                {
                    subquery_reports = subquery_reports.Where(x => database.Contains(x.SourceDb));
                }

                if (masterFile.Any())
                {
                    subquery_reports = subquery_reports.Where(
                        x =>
                            masterFile.Contains(x.EpicMasterFile)
                            || masterFile.Contains("None") && string.IsNullOrEmpty(x.EpicMasterFile)
                    );
                }

                if (visible.Any())
                {
                    subquery_reports = subquery_reports.Where(
                        x =>
                            visible.Contains(x.DefaultVisibilityYn)
                            || (
                                visible.Contains("Y") && string.IsNullOrEmpty(x.DefaultVisibilityYn)
                            )
                    );
                }

                if (certification.Any())
                {
                    subquery_reports = subquery_reports.Where(
                        x =>
                            certification
                                .Intersect(x.ReportTagLinks.Select(x => x.Tag.Name).ToList())
                                .Any()
                    );
                }

                if (availability.Any())
                {
                    subquery_reports = subquery_reports.Where(
                        x =>
                            availability.Contains(x.Availability)
                            || (
                                availability.Contains("Public")
                                && string.IsNullOrEmpty(x.Availability)
                            )
                    );
                }

                if (reportType.Any())
                {
                    subquery_reports = subquery_reports.Where(
                        x => reportType.Contains((int)x.ReportObjectTypeId)
                    );
                }
            }
            else
            {
#pragma warning disable S112
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
#pragma warning restore S112
            }

            string date_format = "h tt";
            var subquery =
                from d in subquery_rundata.Where(
                    x => x.RunStartTime_Hour >= start && x.RunStartTime_Hour <= end
                )
                join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
                join r in subquery_reports on b.ReportObjectId equals r.ReportObjectId
                select new SubqueryData
                {
                    d = d,
                    b = b,
                    r = r
                };

            var subquery_group = (
                from x in subquery
                group new SubqueryData
                {
                    d = x.d,
                    b = x.b,
                    r = x.r
                } by x.d.RunStartTime_Hour
            );

            switch (end_at - start_at)
            {
                // for < 2 days
                // 1 AM, 2 AM etc..
                case < 172800:
                    date_format = "h tt";
                    subquery =
                        from d in subquery_rundata.Where(
                            x => x.RunStartTime_Hour >= start && x.RunStartTime_Hour <= end
                        )
                        join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
                        join r in subquery_reports on b.ReportObjectId equals r.ReportObjectId
                        select new SubqueryData
                        {
                            d = d,
                            b = b,
                            r = r
                        };

                    subquery_group = (
                        from x in subquery
                        group new SubqueryData
                        {
                            d = x.d,
                            b = x.b,
                            r = x.r
                        } by x.d.RunStartTime_Hour
                    );

                    break;
                // for < 8 days
                //  Sun 3/20, Mon 3/21...
                case < 691200:
                    date_format = "ddd M/d";
                    subquery =
                        from d in subquery_rundata.Where(
                            x => x.RunStartTime_Day >= start && x.RunStartTime_Day <= end
                        )
                        join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
                        join r in subquery_reports on b.ReportObjectId equals r.ReportObjectId
                        select new SubqueryData
                        {
                            d = d,
                            b = b,
                            r = r
                        };

                    subquery_group = (
                        from x in subquery
                        group new SubqueryData
                        {
                            d = x.d,
                            b = x.b,
                            r = x.r
                        } by x.d.RunStartTime_Day
                    );
                    break;
                // for < 365 days
                // Mar 1, Mar 2
                case < 31536000:
                    date_format = "MMM d";
                    subquery =
                        from d in subquery_rundata.Where(
                            x => x.RunStartTime_Day >= start && x.RunStartTime_Day <= end
                        )
                        join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
                        join r in subquery_reports on b.ReportObjectId equals r.ReportObjectId
                        select new SubqueryData
                        {
                            d = d,
                            b = b,
                            r = r
                        };

                    subquery_group = (
                        from x in subquery
                        group new SubqueryData
                        {
                            d = x.d,
                            b = x.b,
                            r = x.r
                        } by x.d.RunStartTime_Day
                    );
                    break;
                case >= 31536000:
                    date_format = "MMM yy";
                    subquery =
                        from d in subquery_rundata.Where(
                            x => x.RunStartTime_Month >= start && x.RunStartTime_Month <= end
                        )
                        join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
                        join r in subquery_reports on b.ReportObjectId equals r.ReportObjectId
                        select new SubqueryData
                        {
                            d = d,
                            b = b,
                            r = r
                        };

                    subquery_group = (
                        from x in subquery
                        group new SubqueryData
                        {
                            d = x.d,
                            b = x.b,
                            r = x.r
                        } by x.d.RunStartTime_Month
                    );

                    break;
            }
            return new Tuple<
                IQueryable<SubqueryData>,
                IQueryable<IGrouping<DateTime, SubqueryData>>,
                string
            >(subquery, subquery_group, date_format);
        }

        public async Task<ActionResult> OnGetFiltersAsync(
            int id = -1,
            string type = "report",
            double start_at = -31536000, // last 12 months
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
            (var subquery, var subquery_group, string date_format) = BuildSubqueries(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );

            Filter_Server = await subquery
                .GroupBy(x => x.r.SourceServer)
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "server",
                            Value = x.Key,
                            FriendlyValue = x.Key,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = server.Contains(x.Key)
                        }
                )
                .AsNoTracking()
                .ToListAsync();

            Filter_Database = await subquery
                .GroupBy(x => x.r.SourceDb)
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "database",
                            Value = x.Key,
                            FriendlyValue = x.Key,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = database.Contains(x.Key)
                        }
                )
                .AsNoTracking()
                .ToListAsync();

            Filter_ReportType = await subquery
                .GroupBy(x => new { x.r.ReportObjectTypeId, x.r.ReportObjectType.Name })
                .Select(
                    x =>
                        new Filters
                        {
                            Key = "reportType",
                            Value = x.Key.ReportObjectTypeId.ToString(),
                            FriendlyValue = x.Key.Name,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = reportType.Contains((int)x.Key.ReportObjectTypeId)
                        }
                )
                .AsNoTracking()
                .ToListAsync();

            Filter_MasterFile = await subquery
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "masterFile",
                            Value = string.IsNullOrEmpty(x.r.EpicMasterFile)
                                ? "None"
                                : x.r.EpicMasterFile,
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.Value,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = masterFile.Contains(x.Key.Value)
                        }
                )
                .AsNoTracking()
                .ToListAsync();

            Filter_Visible = await subquery
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "visible",
                            Value = x.r.DefaultVisibilityYn == "N" ? "N" : "Y",
                            FriendlyValue = x.r.DefaultVisibilityYn == "N" ? "No" : "Yes"
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.FriendlyValue,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = visible.Contains(x.Key.Value)
                        }
                )
                .AsNoTracking()
                .ToListAsync();

            Filter_Certification = await (
                from l in _context.ReportTagLinks
                join y in subquery on l.ReportId equals y.r.ReportObjectId
                group y by l.Tag.Name into x
                select new Filters
                {
                    Key = "certification",
                    Value = x.Key,
                    FriendlyValue = x.Key,
                    Count = x.Sum(y => y.b.Runs),
                    Checked = certification.Contains(x.Key)
                }
            ).AsNoTracking().ToListAsync();

            Filter_Availabiltiy = await subquery
                .GroupBy(
                    x =>
                        new
                        {
                            Key = "availability",
                            Value = string.IsNullOrEmpty(x.r.Availability)
                                ? "Public"
                                : x.r.Availability
                        }
                )
                .Select(
                    x =>
                        new Filters
                        {
                            Key = x.Key.Key,
                            Value = x.Key.Value,
                            FriendlyValue = x.Key.Value,
                            Count = x.Sum(y => y.b.Runs),
                            Checked = availability.Contains(x.Key.Value)
                        }
                )
                .AsNoTracking()
                .ToListAsync();
            return new PartialViewResult { ViewName = "Partials/_Filters", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetChartAsync(
            int id,
            string type,
            double start_at = -31536000, // last 12 months
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
            (var subquery, var subquery_group, string date_format) = BuildSubqueries(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );

            RunHistory = await (
                from grp in subquery_group
                orderby grp.Key
                select new RunHistoryData
                {
                    Date = grp.Key.ToString(date_format),
                    Users = grp.Select(x => x.d.RunUserId).Distinct().Count(),
                    Runs = grp.Sum(x => x.b.Runs),
                    RunTime = Math.Round(grp.Average(x => (int)x.d.RunDurationSeconds), 1)
                }
            ).AsNoTracking().ToListAsync();

            Runs = RunHistory.Sum(x => x.Runs);

            Users = await subquery.Select(x => x.d.RunUserId).Distinct().CountAsync();

            if (Runs > 0)
            {
                RunTime = Math.Round(
                    await subquery.AverageAsync(x => (int)x.d.RunDurationSeconds),
                    2
                );
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
            double start_at = -31536000, // last 12 months
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
            (var subquery, var subquery_group, string date_format) = BuildSubqueries(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );

            double total = subquery.Sum(x => x.b.Runs);

            BarDataSet = await (
                from a in subquery
                group a by new { a.d.RunUserId, a.d.RunUser.FullnameCalc } into grp
                select new BarData
                {
                    Key = grp.Key.FullnameCalc,
                    Count = grp.Sum(x => x.b.Runs),
                    Percent = (double)grp.Sum(x => x.b.Runs) / total,
                    TitleOne = "Top Users",
                    Date = grp.Max(x => x.d.RunStartTime).ToShortDateString(),
                    DateTitle = "Last Run",
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
            ).OrderByDescending(x => x.Count).Take(20).AsNoTracking().ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetFailsAsync(
            int id,
            string type,
            double start_at = -31536000, // last 12 months
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
            (var subquery, var subquery_group, string date_format) = BuildSubqueries(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );

            double total = subquery.Sum(x => x.b.Runs);

            BarDataSet = await (
                from a in subquery
                where a.d.RunStatus != "Success"
                group a by a.d.RunStatus into grp
                select new BarData
                {
                    Key = Regex.Replace(
                        Regex.Replace(grp.Key, @"^rs", "", RegexOptions.Multiline),
                        @"(?<=[a-z])([A-Z])",
                        " $1"
                    ),
                    Count = grp.Sum(x => x.b.Runs),
                    Percent = (double)grp.Sum(x => x.b.Runs) / total,
                    TitleOne = "Failed Runs",
                    TitleTwo = "Fails"
                }
            ).OrderByDescending(x => x.Count).Take(20).AsNoTracking().ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetReportsAsync(
            int id,
            string type,
            double start_at = -31536000, // last 12 months
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
            (var subquery, var subquery_group, string date_format) = BuildSubqueries(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );

            double total = subquery.Sum(x => x.b.Runs);

            BarDataSet = await (
                from a in subquery
                group a by new
                {
                    a.r.ReportObjectId,
                    name = string.IsNullOrEmpty(a.r.DisplayTitle) ? a.r.Name : a.r.DisplayTitle
                } into grp
                select new BarData
                {
                    Key = grp.Key.name,
                    Count = grp.Sum(x => x.b.Runs),
                    Percent = (double)grp.Sum(x => x.b.Runs) / total,
                    TitleOne = "Top Reports",
                    Date = grp.Max(x => x.d.RunStartTime).ToShortDateString(),
                    DateTitle = "Last Run",
                    TitleTwo = "Runs",
                    Href = "/reports?id=" + grp.Key.ReportObjectId
                }
            ).OrderByDescending(x => x.Count).Take(20).AsNoTracking().ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetStarsAsync(int id, string type)
        {
            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredReports.Any(r => r.Reportid == id))
                    .AsNoTracking()
                    .ToListAsync();
            }
            else if (type == "term" && _context.Terms.Any(x => x.TermId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredTerms.Any(r => r.Termid == id))
                    .AsNoTracking()
                    .ToListAsync();
            }
            else if (type == "collection" && _context.Collections.Any(x => x.CollectionId == id))
            {
                UserStars = await _context.Users
                    .Where(x => x.StarredCollections.Any(r => r.Collectionid == id))
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
#pragma warning disable S112
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
#pragma warning restore S112
            }

            return new PartialViewResult { ViewName = "Partials/_Stars", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetSubscriptionsAsync(int id, string type)
        {
            if (type == "report" && _context.ReportObjects.Any(x => x.ReportObjectId == id))
            {
                UserSubscriptions = await _context.ReportObjectSubscriptions
                    .Where(r => r.ReportObjectId == id)
                    .Include(x => x.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
#pragma warning disable S112
                throw new Exception(
                    "Wrong parameter value supplied. Type: " + type + " with Id: " + id
                );
#pragma warning restore S112
            }

            return new PartialViewResult
            {
                ViewName = "Partials/_Subscriptions",
                ViewData = ViewData
            };
        }
    }
}
