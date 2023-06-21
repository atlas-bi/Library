using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;

namespace Atlas_Web.Pages.Settings
{
    public class GroupRolesModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public GroupRolesModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public List<PrivilegedGroupData> PrivilegedGroups { get; set; }

        [BindProperty]
        public GroupRoleLink NewGroupRole { get; set; }

        public class PrivilegedGroupData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<PrivilegedGroupRolesData> Roles { get; set; }
        }

        public class PrivilegedGroupRolesData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            PrivilegedGroups = await (
                from u in _context.UserGroups
                where u.GroupRoleLinks.Any(x => x.UserRoles.Name.ToLower() != "user")
                orderby u.GroupName
                select new PrivilegedGroupData
                {
                    Id = u.GroupId,
                    Name = u.GroupName,
                    Roles =
                        from l in u.GroupRoleLinks
                        select new PrivilegedGroupRolesData
                        {
                            Id = l.GroupRoleLinksId,
                            Name = l.UserRoles.Name,
                        }
                }
            ).ToListAsync();

            return Page();
        }

        public ActionResult OnGetRemoveGroupPermission(int Id, int GroupId)
        {
            if (User.HasPermission("Edit Group Permissions"))
            {
                _context.RemoveRange(
                    _context.GroupRoleLinks.Where(
                        x => x.GroupId.Equals(GroupId) && x.UserRolesId.Equals(Id)
                    )
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostAddGroupPermission()
        {
            if (
                ModelState.IsValid
                && User.HasPermission("Edit User Permissions")
                && !_context.GroupRoleLinks.Any(
                    x =>
                        x.GroupId == NewGroupRole.GroupId
                        && x.UserRolesId == NewGroupRole.UserRolesId
                )
            )
            {
                _context.Add(
                    new GroupRoleLink
                    {
                        GroupId = NewGroupRole.GroupId,
                        UserRolesId = NewGroupRole.UserRolesId
                    }
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }
    }
}
