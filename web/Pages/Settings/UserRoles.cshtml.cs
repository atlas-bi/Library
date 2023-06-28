using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;

namespace Atlas_Web.Pages.Settings
{
    public class UserRolesModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public UserRolesModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public List<PrivilegedUsersData> PrivilegedUsers { get; set; }

        [BindProperty]
        public UserRoleLink NewUserRole { get; set; }

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
                    Name = u.FullnameCalc,
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
            if (User.HasPermission("Edit User Permissions"))
            {
                _context.RemoveRange(
                    _context.UserRoleLinks.Where(
                        x => x.UserId.Equals(UserId) && x.UserRolesId.Equals(Id)
                    )
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostAddUserPermission()
        {
            if (
                ModelState.IsValid
                && User.HasPermission("Edit User Permissions")
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
            return RedirectToPage("/Settings/Index");
        }
    }
}
