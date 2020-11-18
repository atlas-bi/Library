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
using Data_Governance_WebApp.Models;
using Data_Governance_WebApp.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Data_Governance_WebApp.Pages.Groups
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

        public List<AdList> AdLists { get; set; }
        public List<UserFavorites> Favorites { get; set; }
        public User UserDetails { get; set; }
        public List<int?> Permissions { get; set; }
        public int UserId { get; set; }
        public int MyId { get; set; }
        public List<ReportObject> ReportObjectDocLastViewed { get; set; }
        public List<string> AnalyticsList { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        [BindProperty] public MyRole MyRole { get; set; }
        [BindProperty] public UserFavoriteFolders Folder { get; set; }
        [BindProperty] public MyRole AsAdmin { get; set; }
        public User PublicUser { get; set; }
        public IEnumerable<UserList> GroupUsers { get; set; }
        public IEnumerable<ReportList> GroupReports { get; set; }
        public GroupItem Group { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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
                UserDetails = _context.User.Where(x => x.UserId == UserId).FirstOrDefault() ;
            }

            Group = await _cache.GetOrCreateAsync<GroupItem>("Group-" + id,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                    return (from a in _context.UserGroups
                            where a.GroupId == id
                            select new GroupItem
                            {
                                Id = a.GroupId,
                                Email = a.GroupEmail,
                                Type = a.GroupType,
                                Name = a.GroupName,
                                Source = a.GroupSource
                            }).FirstOrDefaultAsync();
                });

            // users w/ group
            GroupUsers = await _cache.GetOrCreateAsync<List<UserList>>("GroupUsers-" + id,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                    return (from a in _context.UserGroupsMembership
                            where a.GroupId == id
                            select new UserList
                            {
                                Id = a.UserId,
                                Name = a.User.Fullname_Cust,
                                Email = a.User.Email,
                                EpicId = a.User.EpicId,
                                EmployeeId = a.User.EmployeeId,
                                Phone = a.User.Phone,
                            }).ToListAsync();
                });
            // reports w/ group
            GroupReports = await _cache.GetOrCreateAsync<List<ReportList>>("GroupReports-" + id,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(12);
                    return (from a in _context.ReportGroupsMemberships
                            join q in (from f in _context.UserFavorites
                                       where f.ItemType.ToLower() == "report"

                                       select new { f.ItemId })
                           on a.ReportId equals q.ItemId into tmp
                            from rfi in tmp.DefaultIfEmpty()
                            where a.GroupId == id
                            select new ReportList
                            {
                                Id = a.ReportId,
                                Name = a.Report.DisplayName,
                                LastUpdated = a.Report.LastUpdatedDateDisplayString,
                                Runs = a.Report.ReportObjectRunData.Count(),
                                Subscriptions= a.Report.ReportObjectSubscriptions.Count(),
                                Favs = tmp.Count()
                            }).ToListAsync();
                });

            

            return Page();
        }
    }
}