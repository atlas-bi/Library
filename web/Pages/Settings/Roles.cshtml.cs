using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Atlas_Web.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Settings
{
    public class RolesModel : PageModel
    {
        private readonly IMemoryCache _cache;
        private readonly Atlas_WebContext _context;

        public RolesModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<UserRolesData> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }

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

        public async Task<IActionResult> OnGetAsync()
        {
            UserRoles = await (
                from u in _context.UserRoles
                select new UserRolesData
                {
                    Id = u.UserRolesId,
                    Name = u.Name,
                    Permissions =
                        from p in u.RolePermissionLinks
                        select new RolePermissionsData
                        {
                            Id = p.RolePermissionsId,
                            Name = p.RolePermissions.Name
                        }
                }
            ).ToListAsync();

            RolePermissions = await _context.RolePermissions.OrderBy(x => x.Name).ToListAsync();
            return Page();
        }

        public ActionResult OnGetDeleteRole(int Id)
        {
            // cannot delete admin or user role
            if (
                Id != 1
                && Id != 5
                && _context.UserRoles.Where(x => x.UserRolesId == Id).First().Name != "Director"
                && User.HasPermission("Edit Role Permissions")
            )
            {
                // remove links, then remove role
                _context.RemoveRange(_context.UserRoleLinks.Where(x => x.UserRolesId == Id));
                _context.RemoveRange(_context.RolePermissionLinks.Where(x => x.RoleId == Id));
                _context.Remove(_context.UserRoles.Where(x => x.UserRolesId == Id).First());
                _context.SaveChanges();
            }
            // clear cache
            var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
            for (var x = 0; x < oldPerm.Count; x++)
            {
                _cache.Remove(oldPerm[x]);
            }
            return RedirectToPage("/AccessControl/Index");
        }

        public ActionResult OnPostCreateRole()
        {
            if (
                ModelState.IsValid
                && User.HasPermission("Edit Role Permissions")
                && UserRole.Name != "Administrator"
                && UserRole.Name != "Director"
            )
            {
                _context.Add(UserRole);
                _context.SaveChanges();
            }

            return RedirectToPage("/AccessControl/Index");
        }

        public ActionResult OnPostUpdatePermissions(int RoleId, int PermissionId, int Type)
        {
            if (User.HasPermission("Edit Role Permissions"))
            {
                // type 1 = add
                // type 2 = remove

                _context.RemoveRange(
                    _context.RolePermissionLinks.Where(
                        x => x.RoleId.Equals(RoleId) && x.RolePermissionsId.Equals(PermissionId)
                    )
                );

                if (Type == 1)
                {
                    _context.Add(
                        new RolePermissionLink { RoleId = RoleId, RolePermissionsId = PermissionId }
                    );
                }
                // clear cache
                var oldPerm = _cache.Get<List<string>>("MasterUserPermissions");
                for (var x = 0; x < oldPerm.Count; x++)
                {
                    try
                    {
                        _cache.Remove(oldPerm[x]);
                    }
                    catch
                    {
                        // if the perm failed to remove let it hang.
                    }
                }
                _context.SaveChanges();
            }

            return Content("success");
        }
    }
}
