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

using Data_Governance_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages
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
        public List<UserFavorites> Favorites { get; set; }
        public List<int?> Permissions { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        [BindProperty] public UserFavoriteFolders Folder { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        public class BasicFavoriteData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
        }

        public class BasicFavoriteReportData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
            public string ReportUrl { get; set; }
        }
        public List<AdList> AdLists { get; set; }
        public User PublicUser { get; set; }

        public ActionResult OnGetAsync()
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = PublicUser;
            UserId = MyUser.UserId;
            FirstName = MyUser.Firstname_Cust;
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);

            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                //new AdList { Url = "Reports/?handler=RelatedReports&id="+id, Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentProjects", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;
            
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Page();
        }

        public ActionResult OnGetScripts()
        {
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_Scripts");
        }

        public async Task<ActionResult> OnGetWelcomeVideo()
        {
            var Pref = await Task.Run(() => UserHelpers.GetPreferences(_cache, _context, User.Identity.Name).Where(x => x.ItemType == "WelcomeToAtlasVideo").FirstOrDefault());
            if(Pref != null)
            {
                ViewData["Open"] = Pref.ItemValue;
            } else
            {
                ViewData["Open"] = 1;
            }

          return Partial("Partials/_WelcomeVideo");
        }

        public async Task<ActionResult> OnGetRecentTerms()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var NewestApprovedTerms = await (from dp in _context.Term
                                                where dp.ApprovedYn == "Y"
                                                   && dp.ValidFromDateTime > DateTime.Now.AddDays(-30)
                                                join q in (from f in _context.UserFavorites
                                                     where f.ItemType.ToLower() == "term"
                                                        && f.UserId == user.UserId
                                                     select new { f.ItemId })
                                           on dp.TermId equals q.ItemId into tmp
                                           orderby dp.ValidFromDateTime descending
                                          from fi in tmp.DefaultIfEmpty()
                                          select new BasicFavoriteData
                                          {
                                              Name = dp.Name,
                                              Id = dp.TermId,
                                              Favorite = fi.ItemId == null ? "no" : "yes"
                                          }).Take(10).ToListAsync();
            ViewData["NewestApprovedTerms"] = NewestApprovedTerms;

            int myLength = 10 - NewestApprovedTerms.Count;
            if (myLength > 0)
            {
                ViewData["NewestTerms"] = await (from dp in _context.Term
                                           where dp.ApprovedYn == "N"
                                              && dp.ValidFromDateTime > DateTime.Now.AddDays(-30)
                                           join q in (from f in _context.UserFavorites
                                                      where f.ItemType.ToLower() == "term"
                                                         && f.UserId == user.UserId
                                                      select new { f.ItemId })
                                      on dp.TermId equals q.ItemId into tmp
                                           orderby dp.LastUpdatedDateTime descending
                                           from fi in tmp.DefaultIfEmpty()
                                           select new BasicFavoriteData
                                           {
                                               Name = dp.Name,
                                               Id = dp.TermId,
                                               Favorite = fi.ItemId == null ? "no" : "yes"
                                           }).Take(myLength).ToListAsync();
            } else
            {
                List<int> MyList = new List<int>();
                ViewData["NewestTerms"] = MyList;
            }
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Partial("Partials/_RecentTerms");

        }

        public async Task<ActionResult> OnGetRecentReports()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestReports"] = await (from dp in _context.ReportObjectDoc
                                          join q in (from f in _context.UserFavorites
                                                      where f.ItemType.ToLower() == "report"
                                                         && f.UserId == user.UserId
                                                      select new {f.ItemId})
                                           on dp.ReportObjectId equals q.ItemId into tmp
                                           from fi in tmp.DefaultIfEmpty()
                                           orderby dp.LastUpdateDateTime descending
                                           select new BasicFavoriteReportData
                                           {
                                               Name = dp.ReportObject.Name,
                                               Id = dp.ReportObjectId,
                                               Favorite = fi.ItemId == null ? "no" : "yes",
                                               ReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, dp.ReportObject.ReportObjectUrl,
                                                                              dp.ReportObject.Name,
                                                                              dp.ReportObject.ReportObjectType.Name,
                                                                              dp.ReportObject.EpicReportTemplateId.ToString(),
                                                                              dp.ReportObject.EpicRecordId.ToString(),
                                                                              dp.ReportObject.EpicMasterFile,
                                                                              dp.ReportObject.ReportObjectDoc.EnabledForHyperspace
                                                                              )
                                           }).Take(10).ToListAsync();
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Partial("Partials/_RecentReports");
        }

        public async Task<ActionResult> OnGetRecentProjects()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestProjects"] = await (from dp in _context.DpDataProject
                                            join q in (from f in _context.UserFavorites
                                                        where f.ItemType.ToLower() == "project"
                                                        && f.UserId == user.UserId
                                                       select new { f.ItemId })
                                            on dp.DataProjectId equals q.ItemId into tmp
                                            from fi in tmp.DefaultIfEmpty()
                                            orderby dp.LastUpdateDate descending
                                            select new BasicFavoriteData
                                            {
                                                Name = dp.Name,
                                                Id = dp.DataProjectId,
                                                Favorite = fi.ItemId == null ? "no" : "yes"
                                            }).Take(10).ToListAsync();

            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Partial("Partials/_RecentProjects");
        }

        public async Task<ActionResult> OnGetRecentInitiatives()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestInitiatives"] = await (from di in _context.DpDataInitiative
                                              join q in (from f in _context.UserFavorites
                                                         where f.ItemType.ToLower() == "initiative"
                                                         && f.UserId == user.UserId
                                                         select new {f.ItemId})
                                              on di.DataInitiativeId equals q.ItemId into tmp
                                              from fi in tmp.DefaultIfEmpty()
                                              orderby di.LastUpdateDate descending
                                              select new BasicFavoriteData
                                              {
                                                  Name = di.Name,
                                                  Id = di.DataInitiativeId,
                                                  Favorite = fi.ItemId == null ? "no" : "yes"
                                              }).Take(10).ToListAsync();
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Partial("Partials/_RecentInitiatives");
        }
    }
}