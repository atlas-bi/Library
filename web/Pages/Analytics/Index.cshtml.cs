using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atlas_Web.Pages.Analytics
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public IndexModel(Atlas_WebContext context)
        {
            _context = context;
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

        [BindProperty]
        public Models.Analytic NewAnalytic { get; set; }

        public ActionResult OnGet()
        {
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
                    on new
                    {
                        b.UserId,
                        b.SessionId,
                        time = b.UpdateTime
                    } equals new
                    {
                        sub.UserId,
                        sub.SessionId,
                        time = sub.Time
                    }
                join u in _context.Users on b.UserId equals u.UserId
                select new ActiveUserData
                {
                    Fullname = u.FullnameCalc,
                    UserId = b.UserId,
                    SessionId = b.SessionId,
                    SessionTime = TimeSpan.FromMilliseconds(sub.SessionTime).ToString(@"h\:mm\:ss"),
                    PageTime = TimeSpan.FromMilliseconds(b.PageTime ?? 0).ToString(@"h\:mm\:ss"),
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
            {
                ViewData["ActiveUserData"] = ActiveUserData;
            }

            ViewData["ActiveUsers"] = ActiveUsers;

            return new PartialViewResult
            {
                ViewName = "Partials/_ActiveUsers",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnPostBeacon()
        {
            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            /*
                * check if session + page exists
                * if yes > update time
                * if no > create
                *
            */
            var oldAna = await _context.Analytics
                .Where(
                    x =>
                        x.UserId == User.GetUserId()
                        && x.SessionId == package.Value<string>("sessionId")
                        && x.PageId == package.Value<string>("pageId")
                )
                .ToListAsync();
            if (oldAna.Count > 0)
            {
                oldAna[0].PageTime = (int)package["pageTime"];
                oldAna[0].UpdateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return Content("ok");
            }

            /*
                session: hostname, browser, os, device, screen, lang
                view: session, time, url, referer
                event: session, time, url, type, value
            */
            NewAnalytic.UserId = User.GetUserId();
            NewAnalytic.Language = package.Value<string>("language") ?? "";
            NewAnalytic.UserAgent = package.Value<string>("userAgent") ?? "";
            NewAnalytic.Hostname = package.Value<string>("hostname") ?? ""; // keep
            NewAnalytic.Href = package.Value<string>("href") ?? "";
            NewAnalytic.Protocol = package.Value<string>("protocol") ?? "";
            NewAnalytic.Search = package.Value<string>("search") ?? "";
            NewAnalytic.Pathname = package.Value<string>("pathname") ?? "";
            NewAnalytic.ScreenHeight = package.Value<string>("screenHeight") ?? "";
            NewAnalytic.ScreenWidth = package.Value<string>("screenWidth") ?? "";
            NewAnalytic.Origin = package.Value<string>("origin") ?? "";
            NewAnalytic.LoadTime = package.Value<string>("loadTime") ?? "";
            NewAnalytic.AccessDateTime = DateTime.Now;
            NewAnalytic.UpdateTime = DateTime.Now;
            NewAnalytic.Referrer = package.Value<string>("referrer") ?? "";
            NewAnalytic.Zoom = (double)package["zoom"];
            NewAnalytic.Epic = HttpContext.IsHyperspace() ? 1 : 0;
            NewAnalytic.SessionId = package.Value<string>("sessionId") ?? "";
            NewAnalytic.PageId = package.Value<string>("pageId") ?? "";
            NewAnalytic.PageTime = (int)package["pageTime"];

            await _context.Analytics.AddAsync(NewAnalytic);
            await _context.SaveChangesAsync();

            return Content("ok");
        }
    }
}
