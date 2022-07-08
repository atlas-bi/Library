using System;
using Atlas_Web.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Helpers
{
    public static class UserHelpers
    {
        public static Boolean IsAdmin(Atlas_WebContext _context, string username)
        {
            username ??= "default";
            var is_Admin = (
                from r in _context.UserRoles
                where r.UserRoleLinks.Any(x => x.User.Username == username && x.UserRolesId == 1)
                select new { r.Name }
            ).Count();
            if (is_Admin > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<UserPreference> GetPreferences(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username ??= "default";
            return cache.GetOrCreate<List<UserPreference>>(
                "Preferences-" + username,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.UserPreferences
                        .Where(x => x.User.Username == username)
                        .ToList();
                }
            );
        }

        public static MyRole GetMyRole(Atlas_WebContext _context, string username)
        {
            username ??= "default";
            return (
                from p in _context.UserPreferences
                where p.User.Username == username && p.ItemType == "ActiveRole"
                join u in _context.UserRoles on p.ItemValue equals u.UserRolesId
                select new MyRole { Id = (int)p.ItemValue, Name = u.Name }
            ).FirstOrDefault();
        }

        public static User GetUser(IMemoryCache cache, Atlas_WebContext _context, string username)
        {
            username ??= "default";
            return cache.GetOrCreate<User>(
                "User-" + username,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    if (_context.Users.Any(u => u.Username.Equals(username)))
                    {
                        return _context.Users.SingleOrDefault(u => u.Username == username);
                    }
                    else
                    {
                        return _context.Users.SingleOrDefault(u => u.Username == "default");
                    }
                }
            );
        }

        public static List<int?> GetUserPermissions(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username ??= "default";
            // master permission cache - so we can clear all when a perm changes.
            var master = cache.GetOrCreate<List<string>>(
                "MasterUserPermissions",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return new List<String> { "UserPermissions-" + username };
                }
            );

            if (master.Count > 1)
            {
                // add in new item
                master.Add("UserPermissions-" + username);
                cache.Set<List<string>>("MasterUserPermissions", master, TimeSpan.FromMinutes(20));
            }

            return cache.GetOrCreate<List<int?>>(
                "UserPermissions-" + username,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    // permission table links
                    // user (username) > user role links > user roles > role permissions links > role permissions (activity)

                    // if an admin, return all privaledges.
                    // ps, never check on the name, always the ID... someone could create a duplicate fake "admin" role and steal privaledges
                    var is_Admin = (
                        from r in _context.UserRoles
                        where
                            r.UserRoleLinks.Any(
                                x => x.User.Username == username && x.UserRolesId == 1
                            )
                        select new { r.Name }
                    ).Count();

                    if (is_Admin > 0)
                    {
                        var MyRole = GetMyRole(_context, username);
                        if (
                            MyRole != null
                            && _context.UserRoles.Any(x => x.UserRolesId == MyRole.Id)
                            && MyRole.Id != 1
                        )
                        {
                            List<int?> roles = _context.RolePermissionLinks
                                .Where(k => k.RoleId == MyRole.Id)
                                .Select(x => x.RolePermissionsId)
                                .ToList();
                            roles.Add(44);
                            return roles;
                        }
                        return _context.RolePermissions
                            .Select(x => (int?)x.RolePermissionsId)
                            .ToList();
                    }

                    // default role is 5 (user)
                    var RoleList = (
                        from r in _context.UserRoles
                        where r.UserRoleLinks.Any(x => x.User.Username == username)
                        select r.UserRolesId
                    ).ToList();

                    if (RoleList.Count == 0)
                    {
                        RoleList = new List<int> { 5 };
                    }

                    var myPermissions = _context.RolePermissionLinks
                        .Where(k => RoleList.Contains(k.RoleId ?? 5))
                        .Select(x => x.RolePermissionsId)
                        .ToList();

                    return myPermissions;
                }
            );
        }

        public static Boolean CheckHrxPermissions(
            Atlas_WebContext _context,
            int ReportObjectId,
            string username
        )
        {
            username ??= "default";

            var report = _context.ReportObjects
                .Where(x => x.ReportObjectId == ReportObjectId)
                .FirstOrDefault();

            //Epic - Crystal Report = 3
            // Reporting Workbench Report = 17

            if (report != null && report.ReportObjectTypeId != 3 && report.ReportObjectTypeId != 17)
            {
                return true;
            }

            // check hrx

            var hrx =
                (
                    from g in _context.ReportGroupsMemberships
                    where
                        g.ReportId == ReportObjectId
                        && (
                            from ug in _context.UserGroupsMemberships
                            where ug.User.Username == username
                            select ug.GroupId
                        ).Contains(g.GroupId)
                    select g.GroupId
                ).ToList().Count;
            if (hrx >= 1)
            {
                return true;
            }

            var hrg =
                (
                    from g in _context.ReportGroupsMemberships
                    join h in _context.ReportObjectHierarchies.Where(
                        x => x.ChildReportObjectId == ReportObjectId
                    )
                        on g.ReportId equals h.ParentReportObjectId
                    where
                        (
                            from ug in _context.UserGroupsMemberships
                            where ug.User.Username == username
                            select ug.GroupId
                        ).Contains(g.GroupId)
                    select g.GroupId
                ).ToList().Count;
            if (hrg >= 1)
            {
                return true;
            }

            return false;
        }

        public static Boolean CheckUserPermissions(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username,
            int Access
        )
        {
            username ??= "default";
            return GetUserPermissions(cache, _context, username).Any(x => x.Value == Access);
        }
    }
}
