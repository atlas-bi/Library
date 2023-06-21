using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UAParser;

namespace Atlas_Web.Pages.Analytics
{
    public class VisitsModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        private readonly IConfiguration _config;

        public VisitsModel(Atlas_WebContext context, IConfiguration config)
        {
            _context = context;

            _config = config;
        }

        public class BarData
        {
            public string Key { get; set; }
            public string Href { get; set; }
            public string TitleOne { get; set; }
            public string TitleTwo { get; set; }

            public double Count { get; set; }
            public double? Percent { get; set; }
        }

        public class AccessHistoryData
        {
            public string Date { get; set; }
            public int Pages { get; set; }
            public int Sessions { get; set; }
            public double LoadTime { get; set; }
        }

        public int Views { get; set; }
        public int Visitors { get; set; }
        public double LoadTime { get; set; }

        public List<AccessHistoryData> AccessHistory { get; set; }

        public List<BarData> BarDataSet { get; set; }

        public async Task<ActionResult> OnGetAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
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
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            switch (end_at - start_at)
            {
                // for < 2 days
                // 1 AM, 2 AM etc..
                case < 172800:
                    AccessHistory = await (
                        from a in subquery
                        // in linqpad, use SqlMethods.DateDiffHour instead of EF.Functions.DateDiffHour
                        group a by MinDate.AddHours(
                            EF.Functions.DateDiffHour(MinDate, (a.AccessDateTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new AccessHistoryData
                        {
                            Date = grp.Key.ToString("h tt"),
                            Sessions = grp.Select(x => x.SessionId).Distinct().Count(),
                            Pages = grp.Select(x => x.PageId).Distinct().Count(),
                            LoadTime = Math.Round(
                                (grp.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                                1
                            )
                        }
                    ).ToListAsync();

                    break;
                // for < 8 days
                //  Sun 3/20, Mon 3/21...
                case < 691200:
                    AccessHistory = await (
                        from a in subquery
                        group a by MinDate.AddDays(
                            EF.Functions.DateDiffDay(MinDate, (a.AccessDateTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new AccessHistoryData
                        {
                            Date = grp.Key.ToString("ddd M/d"),
                            Sessions = grp.Select(x => x.SessionId).Distinct().Count(),
                            Pages = grp.Select(x => x.PageId).Distinct().Count(),
                            LoadTime = Math.Round(
                                (grp.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                                1
                            )
                        }
                    ).ToListAsync();
                    break;
                // for < 365 days
                // Mar 1, Mar 2
                case < 31536000:
                    AccessHistory = await (
                        from a in subquery
                        group a by MinDate.AddDays(
                            EF.Functions.DateDiffDay(MinDate, (a.AccessDateTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new AccessHistoryData
                        {
                            Date = grp.Key.ToString("MMM d"),
                            Sessions = grp.Select(x => x.SessionId).Distinct().Count(),
                            Pages = grp.Select(x => x.PageId).Distinct().Count(),
                            LoadTime = Math.Round(
                                (grp.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                                1
                            )
                        }
                    ).ToListAsync();
                    break;
                case >= 31536000:
                    AccessHistory = await (
                        from a in subquery
                        group a by MinDate.AddMonths(
                            EF.Functions.DateDiffMonth(MinDate, (a.AccessDateTime ?? DateTime.Now))
                        ) into grp
                        orderby grp.Key
                        select new AccessHistoryData
                        {
                            Date = grp.Key.ToString("MMM yy"),
                            Sessions = grp.Select(x => x.SessionId).Distinct().Count(),
                            Pages = grp.Select(x => x.PageId).Distinct().Count(),
                            LoadTime = Math.Round(
                                (grp.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                                1
                            )
                        }
                    ).ToListAsync();
                    break;
            }

            Views = await subquery.CountAsync();

            Visitors = await subquery.Select(x => x.SessionId).Distinct().CountAsync();

            if (Views > 0)
            {
                LoadTime = Math.Round(
                    (await subquery.AverageAsync(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                    1
                );
            }
            else
            {
                LoadTime = 0;
            }

            return Page();
        }

        public async Task<ActionResult> OnGetBrowsersAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var uaParser = Parser.GetDefault();
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            double total = await subquery.CountAsync();
            var grouped = await subquery
                .GroupBy(x => x.UserAgent)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync();
            BarDataSet = (
                from a in grouped.Select(x => new { Key = uaParser.Parse(x.Key), x.Count })
                group a by new { a.Key.UA.Family, a.Key.UA.Major } into grp
                select new BarData
                {
                    Key = grp.Key.Family + " " + grp.Key.Major,
                    Count = grp.Sum(x => x.Count),
                    Percent = (double)grp.Sum(x => x.Count) / total,
                    TitleOne = "Browser",
                    TitleTwo = "Views"
                }
            ).OrderByDescending(x => x.Count).Take(10).ToList();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetOsAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var uaParser = Parser.GetDefault();
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            double total = await subquery.CountAsync();
            var grouped = await subquery
                .GroupBy(x => x.UserAgent)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToListAsync();
            BarDataSet = (
                from a in grouped.Select(x => new { Key = uaParser.Parse(x.Key), x.Count })
                group a by new { a.Key.OS.Family, a.Key.OS.Major } into grp
                select new BarData
                {
                    Key = grp.Key.Family + " " + grp.Key.Major,
                    Count = grp.Sum(x => x.Count),
                    Percent = (double)grp.Sum(x => x.Count) / total,
                    TitleOne = "Operating System",
                    TitleTwo = "Views"
                }
            ).OrderByDescending(x => x.Count).Take(10).ToList();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetResolutionAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            double total = await subquery.CountAsync();
            BarDataSet = await (
                from a in subquery
                group a by new { a.ScreenWidth, a.ScreenHeight } into grp
                select new BarData
                {
                    Key = grp.Key.ScreenWidth + "x" + grp.Key.ScreenHeight,
                    Count = grp.Count(),
                    Percent = (double)grp.Count() / total,
                    TitleOne = "Window Resolution",
                    TitleTwo = "Views"
                }
            ).OrderByDescending(x => x.Count).Take(10).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetUsersAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            double total = await subquery.CountAsync();
            BarDataSet = await (
                from a in subquery
                group a by new { a.UserId, a.User.FullnameCalc } into grp
                select new BarData
                {
                    Key = grp.Key.FullnameCalc,
                    Count = grp.Count(),
                    Percent = (double)grp.Count() / total,
                    Href =
                        (
                            _config["features:enable_user_profile"] == null
                            || _config["features:enable_user_profile"].ToString().ToLower()
                                == "true"
                        )
                            ? "/users?id=" + grp.Key.UserId
                            : null,
                    TitleOne = "Top Users",
                    TitleTwo = "Views"
                }
            ).OrderByDescending(x => x.Count).Take(10).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetLoadTimesAsync(
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var subquery = _context.Analytics.Where(
                x =>
                    x.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && x.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                subquery = subquery.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                subquery = subquery.Where(
                    x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId)
                );
            }

            BarDataSet = await (
                from a in subquery
                group a by a.Pathname into grp
                select new BarData
                {
                    Key = grp.Key,
                    Count = Math.Round(
                        (grp.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000),
                        1
                    ),
                    TitleOne = "Load Times",
                    TitleTwo = "Seconds"
                }
            ).OrderByDescending(x => x.Count).Take(10).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_BarData", ViewData = ViewData };
        }
    }
}
