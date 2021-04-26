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
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Atlas_Web.Pages.Users
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

        public class FolderList
        {
            public string FolderName { get; set; }
            public int FolderId { get; set; }
            public int? FolderRank { get; set; }
            public int? FavCount { get; set; }
        }

        public class ReportRunData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public int Hits { get; set; }
            public decimal RunTime { get; set; }
            public string LastRun { get; set; }
        }

        public class ReportRunTimeData
        {
            public string Date { get; set; }
            public int Cnt { get; set; }
            public decimal Avg { get; set; }
        }

        public class FavData
        {
            public string Name { get; set; }
            public int ItemId { get; set; }
            public string AtlasUrl { get; set; }
            public int? FolderId { get; set; }
            public string Description { get; set; }
            public string EpicRecordId { get; set; }
            public string EpicMasterFile { get; set; }
            public string ReportServerPath { get; set; }
            public string SourceServer { get; set; }
            public string ReportUrl { get; set; }
            public string EpicReleased { get; set; }
            public string ManageReportUrl { get; set; }
            public string FolderName { get; set; }
            public int? ItemRank { get; set; }
            public int? FolderRank { get; set; }
            public int FavoriteId { get; set; }
            public string EpicReportTemplateId { get; set; }
            public string EditReportUrl { get; set; }
            public string ItemType { get; set; }
        }

        public class FailedRunsData
        {
            public string Date { get; set; }
            public string Url { get; set; }
            public string RunStatus { get; set; }
            public string Name { get; set; }
        }
        public class AtlasHistoryData
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Type { get; set; }
            public string Date { get; set; }
        }
        public class LastEdited
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Date { get; set; }
        }

        public class SubscribedReportsData
        {
            public string Name { get; set; }
            public int? Id { get; set; }
            public string EmailList { get; set; }
            public string Description { get; set; }
            public string LastStatus { get; set; }
            public string LastRun { get; set; }
            public string SentTo { get; set; }
        }

        public class SearchHistoryData
        {
            public string SearchUrl { get; set; }
            public string SearchString { get; set; }
            public string ReportType { get; set; }
            public string SearchField { get; set; }
            public string HiddenTypes { get; set; }
            public string Hidden { get; set; }
            public string Orphans { get; set; }
            public int Id { get; set; }
        }

        public class Group
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Source { get; set; }
        }

        public class SharedObjectsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShareDate { get; set; }
            public string SharedFrom { get; set; }
            public string Url { get; set; }
        }

        public List<AdList> AdLists { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public User UserDetails { get; set; }
        public List<int?> Permissions { get; set; }
        public IEnumerable<LastEdited> ReportObjectDocEdits { get; set; }
        public IEnumerable<LastEdited> InitiativeEdits { get; set; }
        public IEnumerable<LastEdited> ProjectEdits { get; set; }
        public IEnumerable<LastEdited> TermEdits { get; set; }
        public int UserId { get; set; }
        public int MyId { get; set; }
        public List<ReportObject> ReportObjectDocLastViewed { get; set; }
        public List<string> AnalyticsList { get; set; }
        public IEnumerable<FavData> FavoriteReports { get; set; }
        public IEnumerable<FolderList> FavoriteFolders { get; set; }
        public IEnumerable<ReportRunData> TopRunReports { get; set; }
        public IEnumerable<ReportRunTimeData> ReportRunTime { get; set; }
        public IEnumerable<ReportRunTimeData> LoadTime { get; set; }
        public IEnumerable<FailedRunsData> FailedRuns { get; set; }
        public IEnumerable<AtlasHistoryData> AtlasHistory { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public List<SubscribedReportsData> SubscribedReports { get; set; }
        public List<UserPreference> Preferences { get; set; }
        [BindProperty] public MyRole MyRole { get; set; }
        [BindProperty] public UserFavoriteFolder Folder { get; set; }
        [BindProperty] public MyRole AsAdmin { get; set; }
        public User PublicUser { get; set; }

        public ActionResult OnGet(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            UserDetails = PublicUser;
            // for the viewing user, not the viewed user
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            MyId = PublicUser.UserId;
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            UserId = MyId;
            if (checkpoint)
            {
                UserId = id ?? MyId;
                UserDetails = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            }








            return Page();
        }

        public ActionResult OnPostEditFavorites(int actionType, int objectId, string favoriteType, string objectName)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (actionType == 1)
            {
                _context.Add(new UserFavorite { UserId = MyUser.UserId, ItemRank = -1, ItemId = objectId, ItemType = favoriteType, ItemName = objectName == "null" ? null : objectName });
            }
            else if (actionType == 0)
            {
                if (favoriteType == "search")
                {
                    _context.RemoveRange(_context.UserFavorites.Where(x => x.UserId == MyUser.UserId && x.ItemId == objectId && x.ItemType == favoriteType));
                }
                else
                {
                    _context.RemoveRange(_context.UserFavorites.Where(x => x.ItemId == objectId && x.ItemType == favoriteType));
                }

            }

            _context.SaveChanges();
            // remove cache
            _cache.Remove("FavoriteReports-" + MyUser.UserId);
            _cache.Remove("FavoriteFolders-" + MyUser.UserId);
            _cache.Remove("Favorites-" + User.Identity.Name);
            return Content("ok");
        }

        public ActionResult OnPostCreateFolder()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (ModelState.IsValid)
            {
                _context.Add(Folder);
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + MyUser.UserId);
                _cache.Remove("FavoriteReports-" + MyUser.UserId);
                return Content(Folder.UserFavoriteFolderId.ToString());
            }
            return Content("error");
        }
        public ActionResult OnPostDeleteFolder()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (ModelState.IsValid)
            {
                // remove any report links to this folder
                List<UserFavorite> Favs = _context.UserFavorites.Where(x => x.FolderId == Folder.UserFavoriteFolderId && x.UserId == MyUser.UserId).ToList();
                foreach (UserFavorite Fav in Favs)
                {
                    Fav.FolderId = null;
                }
                _context.Remove(Folder);
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + MyUser.UserId);
                _cache.Remove("FavoriteReports-" + MyUser.UserId);
                return Content(Folder.UserFavoriteFolderId.ToString());
            }
            return Content("error");
        }

        public ActionResult OnPostReorderFavorites([FromBody] dynamic package)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            //try
            //{

            foreach (var l in package)
            {
                var id = (int)l.FavoriteId;
                var fav = (from u in _context.UserFavorites
                           where u.UserFavoritesId == id
                              && u.UserId == MyUser.UserId
                           select u
                              ).FirstOrDefault();
                var r = (int)l.FavoriteRank;
                fav.ItemRank = r;

            }
            _context.SaveChanges();
            _cache.Remove("FavoriteFolders-" + MyUser.UserId);
            _cache.Remove("FavoriteReports-" + MyUser.UserId);
            return Content("ok");
            //}
            //catch
            //{
            //  return Content("error");
            //}
        }

        public ActionResult OnPostUpdateFavoriteFolder([FromBody] dynamic package)
        {
            try
            {
                var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
                var FavoriteId = (int)package.FavoriteId;
                var FolderId = (int)package.FolderId;
                if (FolderId == 0)
                {
                    _context.UserFavorites.Where(x => x.UserFavoritesId == FavoriteId && x.UserId == MyUser.UserId).FirstOrDefault().FolderId = null;
                }
                else
                {
                    _context.UserFavorites.Where(x => x.UserFavoritesId == FavoriteId && x.UserId == MyUser.UserId).FirstOrDefault().FolderId = FolderId;
                }
                _cache.Remove("FavoriteFolders-" + MyUser.UserId);
                _cache.Remove("FavoriteReports-" + MyUser.UserId);
                _context.SaveChanges();
                return Content("ok");
            }
            catch
            {
                return Content("error");
            }
        }

        public ActionResult OnPostReorderFolders([FromBody] dynamic package)
        {
            try
            {
                var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
                foreach (var l in package)
                {
                    var id = (int)l.FolderId;
                    _context.UserFavoriteFolders.Where(x => x.UserFavoriteFolderId == id && x.UserId == MyUser.UserId).FirstOrDefault().FolderRank = l.FolderRank;
                }
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + MyUser.UserId);
                return Content("ok");
            }
            catch
            {
                return Content("error");
            }
        }

        public async Task<ActionResult> OnGetSearchHistory()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["SearchHistory"] = await (from a in _context.Analytics
                                               where a.Pathname.ToLower() == "/search"
                                                  && a.UserId == MyUser.UserId
                                               orderby a.AccessDateTime descending
                                               select new SearchHistoryData
                                               {
                                                   SearchUrl = a.Search.Replace("%25", "%"),
                                                   SearchString = Regex.Match(a.Search + " ", @"s=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " ").Replace("%2C", ","),
                                                   ReportType = Regex.Match(a.Search + " ", @"f=(.*?)[&|?|\s]").Groups[1].Value.Replace("%20", " ").Replace("%2C", ","),
                                                   HiddenTypes = Regex.Match(a.Search + " ", @"t=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%25", "%").Replace("%20", " "),
                                                   Hidden = Regex.Match(a.Search + " ", @"h=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " "),
                                                   Orphans = Regex.Match(a.Search + " ", @"o=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " "),
                                                   SearchField = Regex.Match(a.Search + " ", @"sf=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " ").Replace("%2C", " & ")
                                               }).Take(7).ToListAsync();

            ViewData["SearchFavorites"] = await _cache.GetOrCreateAsync<List<SearchHistoryData>>("SearchFavorites-" + MyUser.UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return (from f in _context.UserFavorites
                            where f.ItemType == "search"
                               && f.UserId == MyUser.UserId
                            select new SearchHistoryData
                            {
                                SearchUrl = f.ItemName.Replace("%25", "%"),
                                Id = f.UserFavoritesId,
                                SearchString = Regex.Match(f.ItemName + " ", @"s=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " ").Replace("%2C", ","),
                                ReportType = Regex.Match(f.ItemName + " ", @"f=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " ").Replace("%2C", ","),
                                HiddenTypes = Regex.Match(f.ItemName + " ", @"t=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " "),
                                Hidden = Regex.Match(f.ItemName + " ", @"h=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " "),
                                Orphans = Regex.Match(f.ItemName + " ", @"o=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " "),
                                SearchField = Regex.Match(f.ItemName + " ", @"sf=(.*?)[&|?|\s]").Groups[1].Value.Replace("%25", "%").Replace("%20", " ").Replace("%2C", " & ")
                            }).ToListAsync();
                });
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");
            HttpContext.Response.Headers.Remove("Expires");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
                                                              //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_SearchHistory",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetFavorites(int? id)
        {
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");
            HttpContext.Response.Headers.Remove("Expires");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            if (checkpoint)
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            FavoriteReports = await _cache.GetOrCreateAsync<List<FavData>>("FavoriteReports-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return (from q in _context.UserFavorites
                            join ro in _context.ReportObjects on new { Id = (int)q.ItemId, Type = q.ItemType } equals new { Id = (int)ro.ReportObjectId, Type = "report" } into t
                            from ro in t.DefaultIfEmpty()
                            where ((ro.DefaultVisibilityYn == "Y" && q.ItemType == "report") || q.ItemType != "report")
                            join tm in _context.Terms on new { Id = (int)q.ItemId, Type = q.ItemType } equals new { Id = (int)tm.TermId, Type = "term" } into tms
                            from tm in tms.DefaultIfEmpty()
                            join pj in _context.DpDataProjects on new { Id = (int)q.ItemId, Type = q.ItemType } equals new { Id = (int)pj.DataProjectId, Type = "project" } into pjs
                            from pj in pjs.DefaultIfEmpty()
                            join di in _context.DpDataInitiatives on new { Id = (int)q.ItemId, Type = q.ItemType } equals new { Id = (int)di.DataInitiativeId, Type = "initiative" } into dis
                            from di in dis.DefaultIfEmpty()
                            where q.UserId == UserId
                              && q.ItemId != 0
                            orderby q.ItemRank, q.ItemName
                            select new FavData
                            {
                                FavoriteId = q.UserFavoritesId,
                                Name = String.IsNullOrEmpty(ro.DisplayName) ? (tm.Name ?? pj.Name ?? di.Name ?? q.ItemName) : ro.DisplayName,
                                ItemId = (int)q.ItemId,
                                ItemType = q.ItemType_Proper,
                                AtlasUrl = q.ItemType + (q.ItemType == "search" ? "?" : "s") + (q.ItemName ?? "?id=" + q.ItemId.ToString() ?? ""),
                                EpicReportTemplateId = ro.EpicReportTemplateId.ToString(),
                                FolderId = q.FolderId,
                                FolderName = q.Folder.FolderName,
                                ItemRank = q.ItemRank,
                                EpicReleased = q.ItemType == "report" ? ReportHelpers.EpicReleased(_context, (int)q.ItemId) : "",
                                FolderRank = q.Folder.FolderRank,
                                Description = (ro.ReportObjectDoc.DeveloperDescription ?? ro.Description ?? ro.DetailedDescription ?? ro.ReportObjectDoc.KeyAssumptions
                                               ?? tm.Summary ?? tm.TechnicalDefinition
                                               ?? pj.Purpose ?? pj.Description
                                               ?? di.Description),
                                EpicRecordId = ro.EpicRecordId.ToString(),
                                EpicMasterFile = ro.EpicMasterFile,
                                ReportServerPath = ro.ReportServerPath,
                                SourceServer = ro.SourceServer,
                                ReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, ro.ReportObjectUrl,
                                                                      ro.Name,
                                                                      ro.ReportObjectType.Name,
                                                                      ro.EpicReportTemplateId.ToString(),
                                                                      ro.EpicRecordId.ToString(),
                                                                      ro.EpicMasterFile,
                                                                      ro.ReportObjectDoc.EnabledForHyperspace
                                                                      ),
                                ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(_config["AppSettings:org_domain"], HttpContext, ro.ReportObjectType.Name, ro.ReportServerPath, ro.SourceServer),
                                EditReportUrl = HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, ro.ReportServerPath, ro.SourceServer, ro.EpicMasterFile, ro.EpicReportTemplateId.ToString(), ro.EpicRecordId.ToString()),

                            }).ToListAsync();
                });

            //ViewData["FavoriteOther"] = 

            ViewData["FavoriteReports"] = FavoriteReports;

            ViewData["FavoriteFolders"] = await _cache.GetOrCreateAsync<List<FolderList>>("FavoriteFolders-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return (from l in _context.UserFavoriteFolders
                            join u in _context.Users on l.UserId equals u.UserId
                            where u.UserId == UserId
                            orderby l.FolderRank ?? -1
                            select new FolderList
                            {
                                FolderName = l.FolderName,
                                FolderId = l.UserFavoriteFolderId,
                                FolderRank = l.FolderRank,
                                FavCount = (from f in l.UserFavorites select f.UserFavoritesId).Count()
                            }).ToListAsync();
                });

            if (FavoriteReports.Count() < 1)
            {

                ViewData["TopRunReports"] = await _cache.GetOrCreateAsync<List<FavData>>("FavTopRunReports-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    return (from d in _context.ReportObjectTopRuns
                            where d.RunUserId == UserId
                            orderby d.LastRun descending
                            select new FavData
                            {
                                FavoriteId = 0,
                                Name = d.ReportObject.DisplayName,
                                ItemId = (int)d.ReportObjectId,
                                EpicReportTemplateId = d.ReportObject.EpicReportTemplateId.ToString(),
                                FolderId = 0,
                                FolderName = "",
                                ItemRank = 0,
                                FolderRank = 0,
                                Description = (
                                     d.ReportObject.ReportObjectDoc.DeveloperDescription ??
                                     d.ReportObject.Description ??
                                     d.ReportObject.DetailedDescription ??
                                     d.ReportObject.ReportObjectDoc.KeyAssumptions
                                     ),
                                EpicRecordId = d.ReportObject.EpicRecordId.ToString(),
                                EpicMasterFile = d.ReportObject.EpicMasterFile,
                                ReportServerPath = d.ReportObject.ReportServerPath,
                                SourceServer = d.ReportObject.SourceServer,
                                ReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, d.ReportObject.ReportObjectUrl,
                                                                       d.ReportObject.Name,
                                                                       d.ReportObject.ReportObjectType.Name,
                                                                       d.ReportObject.EpicReportTemplateId.ToString(),
                                                                       d.ReportObject.EpicRecordId.ToString(),
                                                                       d.ReportObject.EpicMasterFile,
                                                                       d.ReportObject.ReportObjectDoc.EnabledForHyperspace
                                                                       ),
                                EditReportUrl = HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, d.ReportObject.ReportServerPath, d.ReportObject.SourceServer, d.ReportObject.EpicMasterFile, d.ReportObject.EpicReportTemplateId.ToString(), d.ReportObject.EpicRecordId.ToString()),
                            }).Take(10).ToListAsync();
                });
            }
            else
            {
                // only show recomened reports if there are some favs
                var ReportId = string.Join(",", FavoriteReports.Select(x => x.ItemId.ToString()));
                AdLists = new List<AdList>
                {
                    new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                };
                ViewData["AdLists"] = AdLists;
            }

            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_Favorites",
                ViewData = ViewData
            };
        }

        public ActionResult OnPostWelcomeToAtlasState(int State)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyItemType = "WelcomeToAtlasVideo";
            _cache.Remove("Preferences-" + User.Identity.Name);
            var CurrentState = _context.UserPreferences.Where(x => x.UserId == MyUser.UserId && x.ItemType == MyItemType).FirstOrDefault();
            if (State == 1)
            {
                if (CurrentState != null)
                {
                    CurrentState.ItemValue = 1;
                    _context.Attach(CurrentState).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Content("ok");
                }
                else
                {
                    _context.Add(new UserPreference { UserId = MyUser.UserId, ItemType = MyItemType, ItemValue = 1 });
                    _context.SaveChanges();
                    return Content("ok");
                }
            }
            else if (State == 0)
            {
                if (CurrentState != null)
                {
                    CurrentState.ItemValue = 0;
                    _context.Attach(CurrentState).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Content("ok");
                }
                else
                {
                    _context.Add(new UserPreference { UserId = MyUser.UserId, ItemType = MyItemType, ItemValue = 0 });
                    _context.SaveChanges();
                    return Content("ok");
                }
            }

            return Content("ok");
        }

        public ActionResult OnGetChangeRole(int Id, string Url)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (!UserHelpers.IsAdmin(_cache, _context, User.Identity.Name))
            {
                return Redirect("/");
            }

            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count(); x++)
            {
                _cache.Remove(oldPerm[x]);
            }

            var MyItemType = "ActiveRole";
            var CurrentState = _context.UserPreferences.Where(x => x.UserId == MyUser.UserId && x.ItemType == MyItemType).FirstOrDefault();
            try
            {
                if (CurrentState != null)
                {
                    CurrentState.ItemValue = Id;
                    _context.Attach(CurrentState).State = EntityState.Modified;
                }
                else
                {
                    _context.Add(new UserPreference { UserId = MyUser.UserId, ItemType = MyItemType, ItemValue = Id });
                }
                _context.SaveChanges();

                return Redirect(Url ?? "/");
            }
            catch
            {
                return Redirect(Url ?? "/");
            }
        }

        public ActionResult OnGetSharedObjects()
        {
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");
            HttpContext.Response.Headers.Remove("Expires");
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["SharedToMe"] = (from o in _context.SharedItems
                                      where o.SharedToUserId == MyUser.UserId
                                      select new SharedObjectsData
                                      {
                                          Id = o.Id,
                                          Name = o.Name,
                                          ShareDate = o.ShareDate == null ? null : (o.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                                          SharedFrom = o.SharedFromUser.Fullname_Cust,
                                          Url = o.Url
                                      }).ToList();
            ViewData["SharedFromMe"] = (from o in _context.SharedItems
                                        where o.SharedFromUserId == MyUser.UserId
                                        select new SharedObjectsData
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            ShareDate = o.ShareDate == null ? null : (o.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                                            SharedFrom = o.SharedToUser.Fullname_Cust,
                                            Url = o.Url
                                        }).ToList();

            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Partials/_SharedObjects",
                ViewData = ViewData
            };
        }

        public ActionResult OnPostRemoveSharedObject(int id)
        {
            _context.RemoveRange(_context.SharedItems.Where(x => x.Id == id));
            _context.SaveChanges();

            return Content("ok");
        }


        public async Task<ActionResult> OnGetGroups(int? id)
        {

            MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            if (checkpoint)
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["Groups"] = await _cache.GetOrCreateAsync<List<Group>>("Group-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                    return (from a in _context.UserGroupsMemberships
                            where a.UserId == UserId
                        && a.Group.GroupType != "Email Distribution Groups"
                            select new Group
                            {
                                Id = a.GroupId,
                                Name = a.Group.GroupName,
                                Type = a.Group.GroupType,
                                Source = a.Group.GroupSource
                            }).ToListAsync();
                });
            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_Groups",
                ViewData = ViewData
            };
        }
        public async Task<ActionResult> OnGetSubscriptions(int? id)
        {

            MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            if (checkpoint)
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["SubscribedReports"] = await _cache.GetOrCreateAsync<List<SubscribedReportsData>>("SubscribedReports-" + UserId,
               cacheEntry =>
               {
                   cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                   return (from r in _context.ReportObjectSubscriptions.Where(x => x.UserId == UserId).Union(
                                                                             from m in _context.UserGroupsMemberships
                                                                             join s in _context.ReportObjectSubscriptions
                                                                             on m.Group.GroupEmail equals s.SubscriptionTo
                                                                             where m.UserId == UserId

                                                                             select s
                                        )
                           orderby r.InactiveFlags, r.LastRunTime descending
                           select new SubscribedReportsData
                           {
                               Name = r.ReportObject.DisplayName,
                               Description = r.Description,
                               LastStatus = r.LastStatus.Replace(";", "; "),
                               LastRun = r.LastRunDisplayString,
                               SentTo = r.SubscriptionTo.Replace(";", "; "),
                               Id = r.ReportObjectId,
                               EmailList = r.EmailList,
                           }).ToListAsync();
               });


            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_Subscriptions",
                ViewData = ViewData
            };
        }
        public async Task<ActionResult> OnGetHistory(int? id)
        {

            MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            if (checkpoint)
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["AtlasHistory"] = await _cache.GetOrCreateAsync<List<AtlasHistoryData>>("AtlasHistory-" + UserId,
               cacheEntry =>
               {

                   cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                   return (from a in _context.Analytics
                           where a.UserId == UserId
                              && a.AccessDateTime > DateTime.Today.AddDays(-7)
                              && a.Pathname != "/"
                           orderby a.AccessDateTime descending
                           select new AtlasHistoryData
                           {
                               Name = a.Title,
                               Type = (a.Pathname.ToLower() == "/reports" ? "Reports" :
                                       a.Pathname.ToLower() == "/terms" ? "Terms" :
                                       a.Pathname.ToLower() == "/projects" ? "Projects" :
                                       a.Pathname.ToLower() == "/initiatives" ? "Initiatives" :
                                       a.Pathname.ToLower() == "/users" ? "Users" :
                                       a.Pathname.ToLower() == "/contacts" ? "Reports" :
                                       a.Pathname.ToLower() == "/tasks" ? "Tasks" :
                                       a.Pathname.ToLower() == "/search" ? "Search" : "Other"),
                               Url = a.Href,
                               Date = a.AccessDateTimeDisplayString
                           }).ToListAsync();

               });

            ViewData["ReportObjectDocEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>("ReportObjectDocEdits-" + UserId,
              cacheEntry =>
              {
                  cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                  return (from r in _context.ReportObjectDocs
                          where r.UpdatedBy == UserId
                             && r.LastUpdateDateTime > DateTime.Today.AddDays(-30)
                          orderby r.LastUpdateDateTime descending
                          select new LastEdited
                          {
                              Date = r.LastUpdatedDateTimeDisplayString,
                              Name = r.ReportObject.DisplayName,
                              Url = "\\reports?id=" + r.ReportObjectId
                          }).Take(10).ToListAsync();
              });

            ViewData["InitiativeEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>("InitiativeEdits-" + UserId,
               cacheEntry =>
               {
                   cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                   return (from r in _context.DpDataInitiatives
                           where r.LastUpdateUser == UserId
                              && r.LastUpdateDate > DateTime.Today.AddDays(-30)
                           orderby r.LastUpdateDate descending
                           select new LastEdited
                           {
                               Date = r.LastUpdatedDateDisplayString,
                               Name = r.Name,
                               Url = "\\initiatives?id=" + r.DataInitiativeId
                           }).Take(10).ToListAsync();
               });

            ViewData["ProjectEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>("ProjectEdits-" + UserId,
               cacheEntry =>
               {
                   cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                   return (from r in _context.DpDataProjects
                           where r.LastUpdateUser == UserId
                              && r.LastUpdateDate > DateTime.Today.AddDays(-30)
                           orderby r.LastUpdateDate descending
                           select new LastEdited
                           {
                               Date = r.LastUpdatedDateDisplayString,
                               Name = r.Name,
                               Url = "\\projects?id=" + r.DataProjectId
                           }).Take(10).ToListAsync();
               });

            ViewData["TermEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>("TermEdits-" + UserId,
               cacheEntry =>
               {
                   cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);

                   return (from r in _context.Terms
                           where r.UpdatedByUserId == UserId
                              && r.LastUpdatedDateTime > DateTime.Today.AddDays(-30)
                           orderby r.LastUpdatedDateTime descending
                           select new LastEdited
                           {
                               Date = r.LastUpdatedDateTimeDisplayString,
                               Name = r.Name,
                               Url = "\\terms?id=" + r.TermId
                           }).Take(10).ToListAsync();
               });

            ViewData["LoadTime"] = await _cache.GetOrCreateAsync<List<ReportRunTimeData>>("LoadTime-" + UserId,
               cacheEntry =>
               {
                   cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                   return (from a in _context.Analytics
                           where a.AccessDateTime > DateTime.Now.AddDays(-7)
                              && a.UserId == UserId
                           group a by a.Pathname.ToLower() into grp
                           orderby grp.Count() descending
                           select new ReportRunTimeData
                           {
                               Date = grp.Key,
                               Cnt = grp.Count(),
                               Avg = Math.Round((decimal)grp.Average(x => Convert.ToInt32(x.LoadTime)) / 1000, 2)
                           }).ToListAsync();
               });


            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_Atlas",
                ViewData = ViewData
            };
        }
        public async Task<ActionResult> OnGetActivity(int? id)
        {

            MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 37);
            if (checkpoint)
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["ReportRunTime"] = await _cache.GetOrCreateAsync<List<ReportRunTimeData>>("ReportRunTime-" + UserId,
                     cacheEntry =>
                     {
                         cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                         return (from d in _context.ReportObjectRunTimes
                                 where d.RunUserId == UserId
                                 orderby d.RunWeek descending
                                 select new ReportRunTimeData
                                 {
                                     Date = d.RunWeekString,
                                     Cnt = d.Runs ?? 0,
                                     Avg = d.RunTime ?? 0
                                 }
                                        ).ToListAsync(); ;
                     });

            ViewData["TopRunReports"] = await _cache.GetOrCreateAsync<List<ReportRunData>>("TopRunReports-" + UserId,
                    cacheEntry =>
                    {
                        cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                        return (from d in _context.ReportObjectTopRuns
                                where d.RunUserId == UserId
                                   && d.ReportObjectTypeId != 21
                                orderby d.Runs descending
                                select new ReportRunData
                                {
                                    Name = d.Name,
                                    Type = d.ReportObject.ReportObjectType.Name,
                                    Url = "\\reports?id=" + d.ReportObjectId,
                                    Hits = d.Runs ?? 0,
                                    RunTime = d.RunTime ?? 0,
                                    LastRun = d.LastRun
                                }).ToListAsync();
                    });

            ViewData["FailedRuns"] = await _cache.GetOrCreateAsync<List<FailedRunsData>>("FailedRuns-" + UserId,
                   cacheEntry =>
                   {
                       cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);

                       return (from d in _context.ReportObjectRunData
                               where d.RunUserId == UserId
                              && d.RunStatus != "Success"
                               orderby d.RunStartTime descending
                               select new FailedRunsData
                               {
                                   Date = d.RunStartTimeDisplayString,
                                   Url = "\\reports?id=" + d.ReportObjectId,
                                   Name = d.ReportObject.DisplayName,
                                   RunStatus = d.RunStatus
                               }).ToListAsync();
                   });
            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Sections/_Activity",
                ViewData = ViewData
            };
        }
    }
}
