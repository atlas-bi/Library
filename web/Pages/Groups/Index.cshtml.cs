using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Pages.Groups
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

        public class UserList
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string EpicId { get; set; }
            public string EmployeeId { get; set; }
            public string Phone { get; set; }
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

        public class FailedRunsData
        {
            public string Date { get; set; }
            public string Url { get; set; }
            public string RunStatus { get; set; }
            public string Name { get; set; }
        }

        public class ReportList
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string LastUpdated { get; set; }
            public int Favs { get; set; }
            public int Subscriptions { get; set; }
            public int Runs { get; set; }
        }

        public class RunTimeData
        {
            public string Date { get; set; }
            public double Avg { get; set; }
            public int Cnt { get; set; }
        }

        public class GroupItem
        {
            public int? Id { get; set; }
            public string Email { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public string Source { get; set; }
        }

        public List<AdList> AdLists { get; set; }
        public User UserDetails { get; set; }
        public int UserId { get; set; }
        public int MyId { get; set; }
        public List<ReportObject> ReportObjectDocLastViewed { get; set; }
        public List<string> AnalyticsList { get; set; }

        [BindProperty]
        public MyRole MyRole { get; set; }

        [BindProperty]
        public UserFavoriteFolder Folder { get; set; }

        [BindProperty]
        public MyRole AsAdmin { get; set; }

        public IEnumerable<UserList> GroupUsers { get; set; }
        public IEnumerable<ReportList> GroupReports { get; set; }
        public GroupItem Group { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Group = await _cache.GetOrCreateAsync<GroupItem>(
                "Group-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from a in _context.UserGroups
                        where a.GroupId == id
                        select new GroupItem
                        {
                            Id = a.GroupId,
                            Email = a.GroupEmail,
                            Type = a.GroupType,
                            Name = a.GroupName,
                            Source = a.GroupSource
                        }
                    ).FirstOrDefaultAsync();
                }
            );

            // users w/ group
            GroupUsers = await _cache.GetOrCreateAsync<List<UserList>>(
                "GroupUsers-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from a in _context.UserGroupsMemberships
                        where a.GroupId == id
                        select new UserList
                        {
                            Id = a.UserId,
                            Name = a.User.FullnameCalc,
                            Email = a.User.Email,
                            EpicId = a.User.EpicId,
                            EmployeeId = a.User.EmployeeId,
                            Phone = a.User.Phone,
                        }
                    ).ToListAsync();
                }
            );
            // reports w/ group
            GroupReports = await _cache.GetOrCreateAsync<List<ReportList>>(
                "GroupReports-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from a in _context.ReportGroupsMemberships
                        where a.GroupId == id
                        select new ReportList
                        {
                            Id = a.ReportId,
                            Name = a.Report.DisplayName,
                            LastUpdated = a.Report.LastUpdatedDateDisplayString,
                            Runs = a.Report.ReportObjectRunData.Count,
                            Subscriptions = a.Report.ReportObjectSubscriptions.Count,
                            Favs = (
                                from f in _context.UserFavorites
                                where f.ItemType.ToLower() == "report" && f.ItemId == a.ReportId
                                select new { f.ItemId }
                            ).Count()
                        }
                    ).ToListAsync();
                }
            );

            return Page();
        }

        public async Task<ActionResult> OnGetActivity(int Id)
        {
            var PrivateMyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            var PrivateGroupUsers = _context.UserGroupsMemberships
                .Where(x => x.GroupId == Id)
                .Select(x => x.UserId)
                .ToList();

            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );
            ViewData["MyId"] = PrivateMyId;
            ViewData["UserId"] = PrivateMyId;

            ViewData["ReportRunTime"] = await _cache.GetOrCreateAsync<List<ReportRunTimeData>>(
                "ReportRunTime-" + Id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from d in _context.ReportObjectRunTimes
                        where PrivateGroupUsers.Contains(d.RunUserId)
                        orderby d.RunWeek descending
                        select new ReportRunTimeData
                        {
                            Date = d.RunWeekString,
                            Cnt = d.Runs ?? 0,
                            Avg = d.RunTime ?? 0
                        }
                    ).ToListAsync();
                }
            );

            ViewData["TopRunReports"] = await _cache.GetOrCreateAsync<List<ReportRunData>>(
                "TopRunReports-" + Id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from d in _context.ReportObjectTopRuns
                        where
                            PrivateGroupUsers.Contains(d.RunUserId)
                            && d.ReportObjectTypeId != 21
                            && d.ReportObjectTypeId != 39 // extensions
                            && d.ReportObjectTypeId != 40 // columns
                        orderby d.Runs descending
                        select new ReportRunData
                        {
                            Name = d.Name,
                            Type = d.ReportObject.ReportObjectType.Name,
                            Url = "\\reports?id=" + d.ReportObjectId,
                            Hits = d.Runs ?? 0,
                            RunTime = d.RunTime ?? 0,
                            LastRun = d.LastRun
                        }
                    ).ToListAsync();
                }
            );

            ViewData["FailedRuns"] = await _cache.GetOrCreateAsync<List<FailedRunsData>>(
                "FailedRuns-" + Id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    return (
                        from d in _context.ReportObjectRunData
                        where PrivateGroupUsers.Contains(d.RunUserId) && d.RunStatus != "Success"
                        orderby d.RunStartTime descending
                        select new FailedRunsData
                        {
                            Date = d.RunStartTimeDisplayString,
                            Url = "\\reports?id=" + d.ReportObjectId,
                            Name = d.ReportObject.DisplayName,
                            RunStatus = d.RunStatus
                        }
                    ).ToListAsync();
                }
            );

            return new PartialViewResult() { ViewName = "Sections/_Activity", ViewData = ViewData };
        }
    }
}
