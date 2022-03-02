using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Settings
{
    public class UserRolesModel : PageModel
    {
        private readonly IMemoryCache _cache;
        private readonly Atlas_WebContext _context;

        public UserRolesModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<UserRolesData> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public List<PrivilegedUsersData> PrivilegedUsers { get; set; }

        [BindProperty]
        public UserRole UserRole { get; set; }

        [BindProperty]
        public UserRoleLink NewUserRole { get; set; }

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
            PrivilegedUsers = await (
                from u in _context.Users
                where u.UserRoleLinks.Any(x => x.UserRoles.Name.ToLower() != "user")
                orderby u.Username
                select new PrivilegedUsersData
                {
                    Id = u.UserId,
                    Name = u.Fullname_Cust,
                    Roles =
                        from l in u.UserRoleLinks
                        select new PrivilegedUserRolesData
                        {
                            Id = l.UserRolesId,
                            Name = l.UserRoles.Name,
                        }
                }
            ).ToListAsync();

            return Page();
        }

        public ActionResult OnGetRemoveUserPermission(int Id, int UserId)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                1
            );
            if (checkpoint)
            {
                _context.RemoveRange(
                    _context.UserRoleLinks.Where(
                        x => x.UserId.Equals(UserId) && x.UserRolesId.Equals(Id)
                    )
                );
                _context.SaveChanges();
            }

            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count; x++)
            {
                _cache.Remove(oldPerm[x]);
            }

            return RedirectToPage("/UserRoles/Index");
        }

        public ActionResult OnPostAddUserPermission()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                1
            );
            if (
                ModelState.IsValid
                && checkpoint
                && !_context.UserRoleLinks.Any(
                    x => x.UserId == NewUserRole.UserId && x.UserRolesId == NewUserRole.UserRolesId
                )
            )
            {
                _context.Add(
                    new UserRoleLink
                    {
                        UserId = NewUserRole.UserId,
                        UserRolesId = NewUserRole.UserRolesId
                    }
                );
                _context.SaveChanges();
            }
            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count; x++)
            {
                _cache.Remove(oldPerm[x]);
            }

            return RedirectToPage("/UserRoles/Index");
        }
    }
}
