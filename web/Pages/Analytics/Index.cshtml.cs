using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Pages.Analytics
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

        public class SmallData
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public class MediumData
        {
            public string Name { get; set; }
            public double Time { get; set; }
            public int Count { get; set; }
        }

        public class ActiveUserData
        {
            public string Fullname { get; set; }
            public int UserId { get; set; }
            public string SessionTime { get; set; }
            public string PageTime { get; set; }
            public string Title { get; set; }
            public string Href { get; set; }
            public string AccessDateTime { get; set; }
            public string UpdateTime { get; set; }
            public int Pages { get; set; }
            public string SessionId { get; set; }
        }

        public class AccessHistoryData
        {
            public string Month { get; set; }
            public int Hits { get; set; }
        }

        public List<MediumData> TopUsers { get; set; }
        public List<AccessHistoryData> AccessHistory { get; set; }
        public List<AccessHistoryData> SearchHistory { get; set; }
        public List<AccessHistoryData> ReportHistory { get; set; }
        public List<AccessHistoryData> TermHistory { get; set; }
        public List<MediumData> TopPages { get; set; }

        [BindProperty]
        public Models.Analytic NewAnalytic { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {

            TopPages = await (
                from a in _context.Analytics
                where a.AccessDateTime >= DateTime.Today.AddDays(-7)
                group a by a.Pathname.ToLower() into grp
                orderby grp.Count() descending
                select new MediumData
                {
                    Name = grp.Key,
                    Time = Math.Round(
                        grp.Average(i => Convert.ToDouble(i.LoadTime ?? "0")) / 1000,
                        2
                    ),
                    Count = grp.Count()
                }
            ).Take(10).ToListAsync();

            DateTime MyNow = DateTime.Today;

            AccessHistory = (
                from a in _context.Analytics
                where
                    a.AccessDateTime.HasValue
                    && a.AccessDateTime < new DateTime(MyNow.Year, MyNow.Month, 1)
                group a by new
                {
                    year = a.AccessDateTime.Value.Year,
                    month = a.AccessDateTime.Value.Month
                } into tmp
                orderby tmp.Key.year ,tmp.Key.month
                select new AccessHistoryData
                {
                    Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
                    Hits = tmp.Count()
                }
            ).ToList();

            SearchHistory = (
                from a in _context.Analytics
                where
                    a.AccessDateTime.HasValue
                    && a.AccessDateTime < new DateTime(MyNow.Year, MyNow.Month, 1)
                    && a.Pathname.ToLower() == "/search"
                group a by new
                {
                    year = a.AccessDateTime.Value.Year,
                    month = a.AccessDateTime.Value.Month
                } into tmp
                orderby tmp.Key.year ,tmp.Key.month
                select new AccessHistoryData
                {
                    Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
                    Hits = tmp.Count()
                }
            ).ToList();

            ReportHistory = (
                from a in _context.Analytics
                where
                    a.AccessDateTime.HasValue
                    && a.AccessDateTime < new DateTime(MyNow.Year, MyNow.Month, 1)
                    && a.Pathname.ToLower() == "/reports"
                group a by new
                {
                    year = a.AccessDateTime.Value.Year,
                    month = a.AccessDateTime.Value.Month
                } into tmp
                orderby tmp.Key.year ,tmp.Key.month
                select new AccessHistoryData
                {
                    Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
                    Hits = tmp.Count()
                }
            ).ToList();

            TermHistory = (
                from a in _context.Analytics
                where
                    a.AccessDateTime.HasValue
                    && a.AccessDateTime < new DateTime(MyNow.Year, MyNow.Month, 1)
                    && a.Pathname.ToLower() == "/terms"
                group a by new
                {
                    year = a.AccessDateTime.Value.Year,
                    month = a.AccessDateTime.Value.Month
                } into tmp
                orderby tmp.Key.year ,tmp.Key.month
                select new AccessHistoryData
                {
                    Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
                    Hits = tmp.Count()
                }
            ).ToList();

            return Page();
        }

        public async Task<ActionResult> OnGetLiveUsers()
        {
            var ActiveUserData = await (
                from b in _context.Analytics
                join sub in (
                    from a in _context.Analytics
                    where a.UpdateTime >= DateTime.Now.AddSeconds(-60)
                    group a by new { a.UserId, a.SessionId } into grp
                    select new
                    {
                        grp.Key.UserId,
                        grp.Key.SessionId,
                        Time = grp.Max(x => x.UpdateTime),
                        SessionTime = grp.Sum(x => x.PageTime ?? 0),
                        Pages = grp.Count()
                    }
                )
                    on new { b.UserId, b.SessionId, time = b.UpdateTime } equals new
                    {
                        sub.UserId,
                        sub.SessionId,
                        time = sub.Time
                    }
                join u in _context.Users on b.UserId equals u.UserId
                select new ActiveUserData
                {
                    Fullname = u.UserNameDatum.Fullname,
                    UserId = (int)b.UserId,
                    SessionId = b.SessionId,
                    SessionTime = TimeSpan.FromMilliseconds(sub.SessionTime).ToString(@"h\:mm\:ss"),
                    PageTime = TimeSpan.FromMilliseconds(b.PageTime ?? 0).ToString(@"h\:mm\:ss"),
                    Title = b.Title,
                    Href = b.Href,
                    AccessDateTime = (b.AccessDateTime ?? DateTime.Now).ToString(
                        @"M/d/yy h\:mm\:ss tt"
                    ),
                    UpdateTime = (b.UpdateTime ?? DateTime.Now).ToString(@"M/d/yy h\:mm\:ss tt"),
                    Pages = sub.Pages
                }
            ).ToListAsync();

            var ActiveUsers = (
                from a in ActiveUserData
                group a by new { a.UserId, a.SessionId } into grp
                from a in grp
                select new { grp.Key.UserId, grp.Key.SessionId }
            ).Count();

            ViewData["ActiveUserData"] = new List<ActiveUserData>();
            if (ActiveUserData.Count > 0)
                ViewData["ActiveUserData"] = ActiveUserData;
            ViewData["ActiveUsers"] = ActiveUsers;

            return new PartialViewResult()
            {
                ViewName = "Partials/_ActiveUsers",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnPostBeacon()
        {
            var body = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var package = JObject.Parse(body);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            /*
                * check if session + page exists
                * if yes > update time
                * if no > create
                *
            */
            var oldAna = await _context.Analytics
                .Where(
                    x =>
                        x.UserId == MyUser.UserId
                        && x.SessionId == package.Value<string>("sessionId")
                        && x.PageId == package.Value<string>("pageId")
                )
                .ToListAsync();
            if (oldAna.Count > 0)
            {
                oldAna.FirstOrDefault().PageTime = (int)package["pageTime"];
                oldAna.FirstOrDefault().UpdateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return Content("ok");
            }

            NewAnalytic.Username = User.Identity.Name;
            NewAnalytic.UserId = MyUser.UserId;
            NewAnalytic.AppCodeName = package.Value<string>("appCodeName") ?? "";
            NewAnalytic.AppName = package.Value<string>("appName") ?? "";
            NewAnalytic.AppVersion = package.Value<string>("appVersion") ?? "";
            NewAnalytic.CookieEnabled = package.Value<string>("cookieEnabled") ?? "";
            NewAnalytic.Language = package.Value<string>("language") ?? "";
            NewAnalytic.Oscpu = package.Value<string>("oscpu") ?? "";
            NewAnalytic.Platform = package.Value<string>("platform") ?? "";
            NewAnalytic.UserAgent = package.Value<string>("userAgent") ?? "";
            NewAnalytic.Host = package.Value<string>("host") ?? "";
            NewAnalytic.Hostname = package.Value<string>("hostname") ?? "";
            NewAnalytic.Href = package.Value<string>("href") ?? "";
            NewAnalytic.Protocol = package.Value<string>("protocol") ?? "";
            NewAnalytic.Search = package.Value<string>("search") ?? "";
            NewAnalytic.Pathname = package.Value<string>("pathname") ?? "";
            NewAnalytic.Hash = package.Value<string>("hash") ?? "";
            NewAnalytic.ScreenHeight = package.Value<string>("screenHeight") ?? "";
            NewAnalytic.ScreenWidth = package.Value<string>("screenWidth") ?? "";
            NewAnalytic.Origin = package.Value<string>("origin") ?? "";
            NewAnalytic.Title = package.Value<string>("title") ?? "";
            NewAnalytic.LoadTime = package.Value<string>("loadTime") ?? "";
            NewAnalytic.AccessDateTime = DateTime.Now;
            NewAnalytic.UpdateTime = DateTime.Now;
            NewAnalytic.Referrer = package.Value<string>("referrer") ?? "";
            NewAnalytic.Zoom = (double)package["zoom"];
            NewAnalytic.Epic = HtmlHelpers.IsEpic(HttpContext) ? 1 : 0;
            NewAnalytic.SessionId = package.Value<string>("sessionId") ?? "";
            NewAnalytic.PageId = package.Value<string>("pageId") ?? "";
            NewAnalytic.PageTime = (int)package["pageTime"];

            await _context.Analytics.AddAsync(NewAnalytic);
            await _context.SaveChangesAsync();

            return Content("ok");
        }
    }
}
