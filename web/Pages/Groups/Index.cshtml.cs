using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Groups
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

        public class UserList
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string EpicId { get; set; }
            public string EmployeeId { get; set; }
            public string Phone { get; set; }
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

        public class GroupItem
        {
            public int? Id { get; set; }
            public string Email { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public string Source { get; set; }
        }

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
                            Runs = a.Report.ReportObjectRunDataBridges.Sum(x => x.Runs),
                            Subscriptions = a.Report.ReportObjectSubscriptions.Count,
                            Favs = (
                                from f in _context.StarredReports
                                where f.Reportid == a.ReportId
                                select new { f.Reportid }
                            ).Count()
                        }
                    ).ToListAsync();
                }
            );

            ViewData["DefaultReportTypes"] = await _context.ReportObjectTypes
                .Where(v => v.Visible == "Y")
                .Select(x => x.ReportObjectTypeId)
                .ToListAsync();

            return Page();
        }
    }
}
