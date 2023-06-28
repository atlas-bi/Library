using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Atlas_Web.Authorization;

namespace Atlas_Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;

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
            public double Avg { get; set; }
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
            public string Image { get; set; }
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

        public User UserDetails { get; set; }
        public IEnumerable<LastEdited> ReportObjectDocEdits { get; set; }
        public IEnumerable<LastEdited> InitiativeEdits { get; set; }
        public IEnumerable<LastEdited> CollectionEdits { get; set; }
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

        [BindProperty]
        public UserFavoriteFolder Folder { get; set; }

        public async Task<ActionResult> OnGetAsync(int? id)
        {
            MyId = User.GetUserId();

            // can user view others?
            UserId = MyId;
            if (User.HasPermission("View Other User"))
            {
                UserId = id ?? MyId;
            }

            UserDetails = await _context.Users.Where(x => x.UserId == UserId).FirstOrDefaultAsync();

            ViewData["DefaultReportTypes"] = await _context.ReportObjectTypes
                .Where(v => v.Visible == "Y")
                .Select(x => x.ReportObjectTypeId)
                .ToListAsync();

            return Page();
        }

        public ActionResult OnPostCreateFolder()
        {
            if (ModelState.IsValid)
            {
                _context.Add(Folder);
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + User.GetUserId());
                _cache.Remove("FavoriteReports-" + User.GetUserId());
                return Content(Folder.UserFavoriteFolderId.ToString());
            }
            return Content("error");
        }

        public ActionResult OnPostDeleteFolder()
        {
            if (ModelState.IsValid)
            {
                // remove any report links to this folder

                _context.Remove(Folder);
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + User.GetUserId());
                _cache.Remove("FavoriteReports-" + User.GetUserId());
                return Content(Folder.UserFavoriteFolderId.ToString());
            }
            return Content("error");
        }

        public ActionResult OnPostReorderFolders([FromBody] dynamic package)
        {
            try
            {
                foreach (var l in package)
                {
                    var id = (int)l.FolderId;
                    _context.UserFavoriteFolders
                        .Where(x => x.UserFavoriteFolderId == id && x.UserId == User.GetUserId())
                        .First()
                        .FolderRank = l.FolderRank;
                }
                _context.SaveChanges();
                _cache.Remove("FavoriteFolders-" + User.GetUserId());
                return Content("ok");
            }
            catch
            {
                return Content("error");
            }
        }

        public async Task<ActionResult> OnGetSearchHistory()
        {
            ViewData["SearchHistory"] = await (
                from a in _context.Analytics
                where a.Pathname.ToLower() == "/search" && a.UserId == User.GetUserId()
                orderby a.AccessDateTime descending
                select new SearchHistoryData
                {
                    SearchUrl = a.Search.Replace("%25", "%"),
                    SearchString = Regex.Match(a.Search + " ", @"Query=(.*?)[&|?|\s]").Groups[
                        1
                    ].Value
                        .Replace("%25", "%")
                        .Replace("%20", " ")
                        .Replace("%2C", ","),
                }
            )
                .Take(7)
                .ToListAsync();

            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");
            HttpContext.Response.Headers.Remove("Expires");
            HttpContext.Response.Headers.Add(
                "Cache-Control",
                "no-cache, no-store, must-revalidate"
            );
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.

            return new PartialViewResult
            {
                ViewName = "Sections/_SearchHistory",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetChangeRole(string Id, string Url)
        {
            if (!User.IsInRole("Administrator"))
            {
                return Redirect(Url);
            }

            var adminDisabled = await _context.UserPreferences.SingleOrDefaultAsync(
                x => x.UserId == User.GetUserId() && x.ItemType == "AdminDisabled"
            );

            if (adminDisabled == null)
            {
                await _context.AddAsync(
                    new UserPreference { UserId = User.GetUserId(), ItemType = "AdminDisabled" }
                );
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.RemoveRange(
                    _context.UserPreferences
                        .Where(x => x.UserId == User.GetUserId() && x.ItemType == "AdminDisabled")
                        .ToList()
                );
                await _context.SaveChangesAsync();
            }

            return Redirect(Url);
        }

        public ActionResult OnGetSharedObjects()
        {
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Remove("Pragma");
            HttpContext.Response.Headers.Remove("Expires");
            HttpContext.Response.Headers.Add(
                "Cache-Control",
                "no-cache, no-store, must-revalidate"
            );
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.

            ViewData["SharedToMe"] = (
                from o in _context.SharedItems
                where o.SharedToUserId == User.GetUserId()
                orderby o.ShareDate descending
                select new SharedObjectsData
                {
                    Id = o.Id,
                    Name = o.Name,
                    ShareDate =
                        o.ShareDate == null
                            ? null
                            : (o.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                    SharedFrom = o.SharedFromUser.FullnameCalc,
                    Url = o.Url
                }
            ).ToList();
            ViewData["SharedFromMe"] = (
                from o in _context.SharedItems
                where o.SharedFromUserId == User.GetUserId()
                select new SharedObjectsData
                {
                    Id = o.Id,
                    Name = o.Name,
                    ShareDate =
                        o.ShareDate == null
                            ? null
                            : (o.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                    SharedFrom = o.SharedToUser.FullnameCalc,
                    Url = o.Url
                }
            ).ToList();

            return new PartialViewResult
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
            MyId = User.GetUserId();

            // can user view others?
            if (User.HasPermission("View Other User"))
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }

            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["Groups"] = await _cache.GetOrCreateAsync<List<Group>>(
                "Group-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from a in _context.UserGroupsMemberships
                        where a.UserId == UserId
                        // && a.Group.GroupType != "Email Distribution Groups"
                        select new Group
                        {
                            Id = a.GroupId,
                            Name = a.Group.GroupName,
                            Type = a.Group.GroupType,
                            Source = a.Group.GroupSource
                        }
                    ).ToListAsync();
                }
            );

            return new PartialViewResult { ViewName = "Sections/_Groups", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetSubscriptions(int? id)
        {
            MyId = User.GetUserId();

            // can user view others?
            if (User.HasPermission("View Other User"))
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }

            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["SubscribedReports"] = await _cache.GetOrCreateAsync<
                List<SubscribedReportsData>
            >(
                "SubscribedReports-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from r in _context.ReportObjectSubscriptions
                            .Where(x => x.UserId == UserId)
                            .Union(
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
                        }
                    ).ToListAsync();
                }
            );

            return new PartialViewResult
            {
                ViewName = "Sections/_Subscriptions",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetHistory(int? id)
        {
            MyId = User.GetUserId();

            // can user view others?
            if (User.HasPermission("View Other User"))
            {
                UserId = id ?? MyId;
            }
            else
            {
                UserId = MyId;
            }

            ViewData["MyId"] = MyId;
            ViewData["UserId"] = UserId;

            ViewData["AtlasHistory"] = await _cache.GetOrCreateAsync<List<AtlasHistoryData>>(
                "AtlasHistory-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return (
                        from a in _context.Analytics
                        where
                            a.UserId == UserId
                            && a.AccessDateTime > DateTime.Today.AddDays(-7)
                            && a.Pathname != "/"
                        orderby a.AccessDateTime descending
                        select new AtlasHistoryData
                        {
                            Name = a.Pathname,
                            Type = (
                                a.Pathname.ToLower() == "/reports"
                                    ? "Reports"
                                    : a.Pathname.ToLower() == "/terms"
                                        ? "Terms"
                                        : a.Pathname.ToLower() == "/projects"
                                            ? "Collections"
                                            : a.Pathname.ToLower() == "/collections"
                                                ? "Collections"
                                                : a.Pathname.ToLower() == "/initiatives"
                                                    ? "Initiatives"
                                                    : a.Pathname.ToLower() == "/users"
                                                        ? "Users"
                                                        : a.Pathname.ToLower() == "/contacts"
                                                            ? "Reports"
                                                            : a.Pathname.ToLower() == "/tasks"
                                                                ? "Tasks"
                                                                : a.Pathname.ToLower() == "/search"
                                                                    ? "Search"
                                                                    : "Other"
                            ),
                            Url = a.Href,
                            Date = a.AccessDateTimeDisplayString
                        }
                    ).ToListAsync();
                }
            );

            ViewData["ReportObjectDocEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>(
                "ReportObjectDocEdits-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return (
                        from r in _context.ReportObjectDocs
                        where
                            r.UpdatedBy == UserId
                            && r.LastUpdateDateTime > DateTime.Today.AddDays(-30)
                        orderby r.LastUpdateDateTime descending
                        select new LastEdited
                        {
                            Date = r.LastUpdatedDateTimeDisplayString,
                            Name = r.ReportObject.DisplayName,
                            Url = "\\reports?id=" + r.ReportObjectId
                        }
                    ).Take(10).ToListAsync();
                }
            );

            ViewData["InitiativeEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>(
                "InitiativeEdits-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return (
                        from r in _context.Initiatives
                        where
                            r.LastUpdateUser == UserId
                            && r.LastUpdateDate > DateTime.Today.AddDays(-30)
                        orderby r.LastUpdateDate descending
                        select new LastEdited
                        {
                            Date = r.LastUpdatedDateDisplayString,
                            Name = r.Name,
                            Url = "\\initiatives?id=" + r.InitiativeId
                        }
                    ).Take(10).ToListAsync();
                }
            );

            ViewData["CollectionEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>(
                "CollectionEdits-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    return (
                        from r in _context.Collections
                        where
                            r.LastUpdateUser == UserId
                            && r.LastUpdateDate > DateTime.Today.AddDays(-30)
                        orderby r.LastUpdateDate descending
                        select new LastEdited
                        {
                            Date = r.LastUpdatedDateDisplayString,
                            Name = r.Name,
                            Url = "\\collections?id=" + r.CollectionId
                        }
                    ).Take(10).ToListAsync();
                }
            );

            ViewData["TermEdits"] = await _cache.GetOrCreateAsync<List<LastEdited>>(
                "TermEdits-" + UserId,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);

                    return (
                        from r in _context.Terms
                        where
                            r.UpdatedByUserId == UserId
                            && r.LastUpdatedDateTime > DateTime.Today.AddDays(-30)
                        orderby r.LastUpdatedDateTime descending
                        select new LastEdited
                        {
                            Date = r.LastUpdatedDateTimeDisplayString,
                            Name = r.Name,
                            Url = "\\terms?id=" + r.TermId
                        }
                    ).Take(10).ToListAsync();
                }
            );

            return new PartialViewResult { ViewName = "Sections/_Atlas", ViewData = ViewData };
        }
    }
}
