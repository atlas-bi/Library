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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Data_Governance_WebApp.Models;
using System.Collections.Generic;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.AccessControl
{
    public class IndexModel : PageModel
    {
        private IMemoryCache _cache;
    	private readonly Data_GovernanceContext _context;
        public IndexModel(Data_GovernanceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public List<UserFavorites> Favorites { get; set; }
    	public List<int?> Permissions { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        public List<UserRolesData> UserRoles { get; set; }
        public List<RolePermissions> RolePermissions { get; set; }
        public List<PrivilegedUsersData> PrivilegedUsers { get; set; }

        [BindProperty] public UserRoles UserRole { get; set; }
        [BindProperty] public UserRoleLinks NewUserRole { get; set; }
        public User PublicUser { get; set; }

        public class UserRolesData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<RolePermissionsData> Permissions { get; set; }
        }

        public class RolePermissionsData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        public class PrivilegedUsersData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<PrivilegedUserRolesData> Roles { get; set; }
        }

        public class PrivilegedUserRolesData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            UserRoles = await (from u in _context.UserRoles
                                select new UserRolesData {
                                    Id = u.UserRolesId,
                                    Name = u.Name,
                                    Permissions = from p in u.RolePermissionLinks
                                                    select new RolePermissionsData {
                                                        Id = p.RolePermissionsId,
                                                        Name = p.RolePermissions.Name
                                                        }
                                }).ToListAsync();
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            RolePermissions = await _context.RolePermissions.OrderBy(x => x.Name).ToListAsync();
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);

            PrivilegedUsers = await (from u in _context.User
                                     where u.UserRoleLinks.Any(x => x.UserRoles.Name.ToLower() != "user")
                                     orderby u.Username
                                     select new PrivilegedUsersData
                                     {
                                         Id = u.UserId,
                                         Name = u.Fullname_Cust,
                                         Roles = from l in u.UserRoleLinks
                                                 select new PrivilegedUserRolesData
                                                 {
                                                     Id = l.UserRolesId,
                                                     Name = l.UserRoles.Name,
                                                 }
                                     }).ToListAsync();
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.

            return Page();
        }

        public ActionResult OnPostRemoveUserPermission()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 1);
            if (ModelState.IsValid && checkpoint)
            {
                var q = NewUserRole;
                _context.RemoveRange(_context.UserRoleLinks.Where(x => x.UserId.Equals(NewUserRole.UserId) && x.UserRolesId.Equals(NewUserRole.UserRolesId)));
                _context.SaveChanges();
            }

            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for(var x=0;x<oldPerm.Count();x++){
                _cache.Remove(oldPerm[x]);
            }

            return RedirectToPage("/AccessControl/Index");
        }

        public ActionResult OnPostAddUserPermission()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 1);
            if (ModelState.IsValid && checkpoint && !_context.UserRoleLinks.Any(x => x.UserId == NewUserRole.UserId && x.UserRolesId == NewUserRole.UserRolesId))
            {
                _context.Add(new UserRoleLinks { UserId = NewUserRole.UserId, UserRolesId = NewUserRole.UserRolesId });
                _context.SaveChanges();
            }
            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count(); x++)
            {
                _cache.Remove(oldPerm[x]);
            }

            return RedirectToPage("/AccessControl/Index"); 
        }

        public ActionResult OnPostUpdatePermissions(int RoleId, int PermissionId, int Type)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 18);
            if (checkpoint)
            {
                // type 1 = add
                // type 2 = remove

                _context.RemoveRange(_context.RolePermissionLinks.Where(x => x.RoleId.Equals(RoleId) && x.RolePermissionsId.Equals(PermissionId)));

                if (Type == 1)
                {
                    _context.Add(new RolePermissionLinks { RoleId = RoleId, RolePermissionsId = PermissionId });
                }
                // clear cache
                var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
                for (var x = 0; x < oldPerm.Count(); x++)
                {
                    try { 
                    _cache.Remove(oldPerm[x]);
                    }
                    catch { }
                }
                _context.SaveChanges();
            }

            return Content("success");
        }

        public ActionResult OnPostDeleteRole()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 18);
            // cannot delete admin or user role
            if (ModelState.IsValid && UserRole.UserRolesId != 1 && UserRole.UserRolesId != 5 && UserRole.Name != "Director" && checkpoint)
            {
                // remove links, then remove role
                _context.RemoveRange(_context.UserRoleLinks.Where(x => x.UserRolesId.Equals(UserRole.UserRolesId)));
                _context.RemoveRange(_context.RolePermissionLinks.Where(x => x.RoleId.Equals(UserRole.UserRolesId)));
                _context.Remove(UserRole);
                _context.SaveChanges();
            }
            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count(); x++)
            {
                _cache.Remove(oldPerm[x]);
            }
            return RedirectToPage("/AccessControl/Index");
        }

        public ActionResult OnPostCreateRole()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 18);
            if (ModelState.IsValid && checkpoint && UserRole.Name != "Administrator" && UserRole.Name !="Director")
            {
                _context.Add(UserRole);
                _context.SaveChanges();
            }

            return RedirectToPage("/AccessControl/Index");
        }
    }
}