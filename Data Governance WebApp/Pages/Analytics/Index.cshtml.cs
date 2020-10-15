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
using System.Linq;
using System.Threading.Tasks;
using Data_Governance_WebApp.Models;
using Data_Governance_WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.Analytics
{
    public class IndexModel : PageModel
    {
        private readonly Data_GovernanceContext _context;
        private IMemoryCache _cache;
        public IndexModel(Data_GovernanceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
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

        public List<UserFavorites> Favorites { get; set; }
        public List<int?> Permissions { get; set; }
        public List<MediumData> TopUsers {get; set;}
        public List<MediumData> TopPages { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        [BindProperty] public Models.Analytics NewAnalytic { get; set; }
        public User PublicUser { get; set; }
        public async Task<ActionResult> OnGetAsync()
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            TopUsers = await (from a in _context.Analytics
                        where a.AccessDateTime >= DateTime.Today.AddDays(-7)
                        group a by a.User.Fullname_Cust into grp
                        orderby grp.Count() descending
                        select new MediumData
                        {
                            Name = grp.Key,
                            Time = Math.Round(grp.Average(i => Convert.ToDouble(i.LoadTime ?? "0"))/1000,2),
                            Count = grp.Count()
                        }).Take(10).ToListAsync();

            TopPages = await (from a in _context.Analytics
                              where a.AccessDateTime >= DateTime.Today.AddDays(-7)
                              group a by a.Pathname.ToLower() into grp
                              orderby grp.Count() descending
                              select new MediumData
                              {
                                  Name = grp.Key,
                                  Time = Math.Round(grp.Average(i => Convert.ToDouble(i.LoadTime ?? "0"))/1000, 2),
                                  Count = grp.Count()
                              }).Take(10).ToListAsync();
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Page();
        }

        public async Task<ActionResult> OnGetLiveUsers()
        {
            var ActiveUserData = await (from b in _context.Analytics
                                     join sub in (from a in _context.Analytics
                                                  where a.UpdateTime >= DateTime.Now.AddSeconds(-60)
                                                  group a by new { a.UserId, a.SessionId } into grp
                                                  select new { grp.Key.UserId, grp.Key.SessionId, Time = grp.Max(x => x.UpdateTime), SessionTime = grp.Sum(x => x.PageTime ?? 0), Pages = grp.Count() })
                                      on new { b.UserId, b.SessionId, time = b.UpdateTime } equals new { sub.UserId, sub.SessionId, time = sub.Time }
                                     join u in _context.User on b.UserId equals u.UserId
                                     select new ActiveUserData
                                     {
                                         Fullname = u.UserNameData.Fullname,
                                         UserId = (int)b.UserId,
                                         SessionId = b.SessionId,
                                         SessionTime = TimeSpan.FromMilliseconds(sub.SessionTime).ToString(@"h\:mm\:ss"),
                                         PageTime = TimeSpan.FromMilliseconds(b.PageTime ?? 0).ToString(@"h\:mm\:ss"),
                                         Title = b.Title,
                                         Href = b.Href,
                                         AccessDateTime = (b.AccessDateTime ?? DateTime.Now).ToString(@"M/d/yy h\:mm\:ss tt"),
                                         UpdateTime = (b.UpdateTime ?? DateTime.Now).ToString(@"M/d/yy h\:mm\:ss tt"),
                                         Pages = sub.Pages
                                     }).ToListAsync();



           var ActiveUsers = (from a in ActiveUserData
                              group a by new { a.UserId, a.SessionId } into grp
                               from a in grp
                               select new
                               {
                                   grp.Key.UserId,
                                   grp.Key.SessionId
                               }).Count();

            ViewData["ActiveUserData"] = new List<ActiveUserData>();
            if (ActiveUserData.Count() > 0) ViewData["ActiveUserData"] = ActiveUserData;
            ViewData["ActiveUsers"] = ActiveUsers;

            return Partial("Partials/_ActiveUsers");
        }
        
        public async Task<ActionResult> OnPostBeacon()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
                var package = JObject.Parse(reader.ReadToEnd());

                /*
                 * check if session + page exists
                 * if yes > update time
                 * if no > create
                 *
                */
                var oldAna = await _context.Analytics.Where(x => x.UserId == MyUser.UserId
                                                            && x.SessionId == package.Value<string>("sessionId")
                                                            && x.PageId == package.Value<string>("pageId")).ToListAsync();
                if(oldAna.Count() > 0)
                {
                    oldAna.FirstOrDefault().PageTime = (int)package["pageTime"];
                    oldAna.FirstOrDefault().UpdateTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                    HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
                    HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
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
            }

            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Content("ok");
        }
    }
}