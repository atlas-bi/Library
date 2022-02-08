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
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Atlas_Web.Pages.Groups
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            UserDetails = UserHelpers.GetUser(_cache, _context, User.Identity.Name); ;
            // for the viewing user, not the viewed user

            MyId = UserDetails.UserId;
           // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                37
            );
            UserId = MyId;
            if (checkpoint)
            {
                UserId = id ?? MyId;
                UserDetails = _context.Users.Where(x => x.UserId == UserId).FirstOrDefault();
            }

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
                            Name = a.User.Fullname_Cust,
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
                            Runs = a.Report.ReportObjectRunData.Count(),
                            Subscriptions = a.Report.ReportObjectSubscriptions.Count(),
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

        public async Task<ActionResult> OnGetActivity(int? Id)
        {
            var MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            var GroupUsers = _context.UserGroupsMemberships
                .Where(x => x.GroupId == Id)
                .Select(x => x.UserId)
                .ToList();

            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );
            ViewData["MyId"] = MyId;
            ViewData["UserId"] = MyId;

            ViewData["ReportRunTime"] = await _cache.GetOrCreateAsync<List<ReportRunTimeData>>(
                "ReportRunTime-" + Id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return (
                        from d in _context.ReportObjectRunTimes
                        where GroupUsers.Contains(d.RunUserId)
                        orderby d.RunWeek descending
                        select new ReportRunTimeData
                        {
                            Date = d.RunWeekString,
                            Cnt = d.Runs ?? 0,
                            Avg = d.RunTime ?? 0
                        }
                    ).ToListAsync();
                    ;
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
                            GroupUsers.Contains(d.RunUserId)
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
                        where GroupUsers.Contains(d.RunUserId) && d.RunStatus != "Success"
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
            //return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Sections/_Activity", ViewData = ViewData };
        }
    }
}
