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
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;
        public IndexModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }
        public List<UserPreference> Preferences { get; set; }
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
        public IEnumerable<RunTimeData> RunTime { get; set; }
        public IEnumerable<FailedRunsData> FailedRuns { get; set; }
        public IEnumerable<SubscriptionData> Subscriptions { get; set; }
        public IEnumerable<FavoritesData> ProfileFavorites { get; set; }

        public List<int?> Permissions { get; set; }

        public List<UserFavorite> Favorites { get; set; }
        public User PublicUser { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            TopUsers = await _cache.GetOrCreateAsync<List<TopUsersData>>("TopUsers-Report" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectRunData
                          where d.ReportObjectId == id
                             && d.RunStatus == "Success"
                          group d by d.RunUserId into grp
                          select new { UserId = grp.Key, count = grp.Count(), avg = grp.Average(x => (int)x.RunDurationSeconds), lastRun = grp.Max(x => (DateTime)x.RunStartTime) } into tmp
                          join u in _context.Users on tmp.UserId equals u.UserId
                          orderby tmp.count descending
                          select new TopUsersData
                          {
                              Username = u.Fullname_Cust,
                              UserUrl = "\\users?id=" + u.UserId,
                              Hits = tmp.count,
                              RunTime = Math.Round(tmp.avg, 2),
                              LastRun = tmp.lastRun.ToString("MM/dd/yyyy")
                          }).ToListAsync();
              });

            RunTime = await _cache.GetOrCreateAsync<List<RunTimeData>>("RunTime-Report" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectReportRunTimes
                          where d.ReportObjectId == id
                          orderby d.RunWeek
                          select new RunTimeData
                          {
                              Date = d.RunWeekString,
                              Avg = (double)d.Duration,
                              Cnt = (int)d.Runs
                          }).ToListAsync();
              });

            FailedRuns = await _cache.GetOrCreateAsync<List<FailedRunsData>>("FailedRuns-Report" + id,
             cacheEntry =>
             {
                 cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                 return (from d in _context.ReportObjectRunData
                         where d.ReportObjectId == id
                            && d.RunStatus != "Success"
                         orderby d.RunStartTime descending
                         select new FailedRunsData
                         {
                             Date = d.RunStartTimeDisplayString,
                             RunUser = d.RunUser.Fullname_Cust,
                             UserUrl = "\\users?id=" + d.RunUserId,
                             RunStatus = d.RunStatus
                         }).ToListAsync();
             });

            Subscriptions = await _cache.GetOrCreateAsync<List<SubscriptionData>>("Subscriptions-Report" + id,
             cacheEntry =>
             {
                 cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                 return (from s in _context.ReportObjectSubscriptions
                         where s.ReportObjectId == id
                         orderby s.ReportObjectId
                         select new SubscriptionData
                         {
                             UserUrl = "\\users?id=" + s.UserId,
                             User = s.User.Fullname_Cust,
                             Subscription = s.SubscriptionTo.Replace(";", "; "),
                             InactiveFlags = s.InactiveFlags.ToString(),
                             EmailList = s.EmailList.Replace(";", "; "),
                             Description = s.Description.Replace(";", "; "),
                             LastStatus = s.LastStatus.Replace(";", "; "),
                             LastRun = s.LastRunDisplayString
                         }).ToListAsync();
             });

            ProfileFavorites = await _cache.GetOrCreateAsync<List<FavoritesData>>("ProfileFavorites-Report" + id,
            cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                return (from f in _context.UserFavorites
                        where f.ItemId == id
                           && f.ItemType.ToLower() == "report"
                        select new FavoritesData
                        {
                            UserUrl = "\\users?id=" + f.UserId,
                            User = f.User.Fullname_Cust,
                        }).ToListAsync();
            });

            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=360");
            return Page();
        }
        public async Task<IActionResult> OnGetCollectionsAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var ReportList = _context.DpReportAnnotations.Where(x => x.DataProjectId == id).Select(x => x.ReportId).ToList();

            TopUsers = await _cache.GetOrCreateAsync<List<TopUsersData>>("TopUsers-Collection" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectRunData
                          where ReportList.Contains(d.ReportObjectId)
                             && d.RunStatus == "Success"
                          group d by d.RunUserId into grp
                          select new { UserId = grp.Key, count = grp.Count(), avg = grp.Average(x => (int)x.RunDurationSeconds), lastRun = grp.Max(x => (DateTime)x.RunStartTime) } into tmp
                          join u in _context.Users on tmp.UserId equals u.UserId
                          orderby tmp.count descending
                          select new TopUsersData
                          {
                              Username = u.Fullname_Cust,
                              UserUrl = "\\users?id=" + u.UserId,
                              Hits = tmp.count,
                              RunTime = Math.Round(tmp.avg, 2),
                              LastRun = tmp.lastRun.ToString("MM/dd/yyyy")
                          }).ToListAsync();
              });

            RunTime = await _cache.GetOrCreateAsync<List<RunTimeData>>("RunTime-Collection" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectReportRunTimes
                          where ReportList.Contains(d.ReportObjectId)
                          group d by new { d.RunWeekString, d.RunWeek } into grp
                          orderby grp.Key.RunWeek
                          select new RunTimeData
                          {
                              Date = grp.Key.RunWeekString,
                              Avg = (double)Math.Round(grp.Average(x => x.Duration ?? 0), 2),
                              Cnt = (int)grp.Sum(x => x.Runs ?? 1)
                          }).ToListAsync();
              });


            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=360");
            return Page();
        }
        public async Task<IActionResult> OnGetTermsAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var ReportList = _context.ReportObjectDocTerms.Where(x => x.TermId == id).Select(x => x.ReportObjectId).ToList();

            TopUsers = await _cache.GetOrCreateAsync<List<TopUsersData>>("TopUsers-Term" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectRunData
                          where ReportList.Contains(d.ReportObjectId)
                             && d.RunStatus == "Success"
                          group d by d.RunUserId into grp
                          select new { UserId = grp.Key, count = grp.Count(), avg = grp.Average(x => (int)x.RunDurationSeconds), lastRun = grp.Max(x => (DateTime)x.RunStartTime) } into tmp
                          join u in _context.Users on tmp.UserId equals u.UserId
                          orderby tmp.count descending
                          select new TopUsersData
                          {
                              Username = u.Fullname_Cust,
                              UserUrl = "\\users?id=" + u.UserId,
                              Hits = tmp.count,
                              RunTime = Math.Round(tmp.avg, 2),
                              LastRun = tmp.lastRun.ToString("MM/dd/yyyy")
                          }).ToListAsync();
              });

            RunTime = await _cache.GetOrCreateAsync<List<RunTimeData>>("RunTime-Term" + id,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                  return (from d in _context.ReportObjectReportRunTimes
                          where ReportList.Contains((int)d.ReportObjectId)
                          group d by new { d.RunWeekString, d.RunWeek } into grp
                          orderby grp.Key.RunWeek
                          select new RunTimeData
                          {
                              Date = grp.Key.RunWeekString,
                              Avg = (double)Math.Round(grp.Average(x => x.Duration ?? 0), 2),
                              Cnt = (int)grp.Sum(x => x.Runs ?? 1)
                          }).ToListAsync();
              });


            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=360");
            return Page();
        }
    }
}
