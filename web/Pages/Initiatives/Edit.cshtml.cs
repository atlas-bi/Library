using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas_Web.Pages.Initiatives
{
    public class EditModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public EditModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public List<int?> Permissions { get; set; }
        public User PublicUser { get; set; }

        [BindProperty]
        public DpDataInitiative Initiative { get; set; }

        [BindProperty]
        public int[] Projects { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                22
            );

            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            ViewData["Fullname"] = MyUser.Fullname_Cust;

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { id = id, error = "You do not have permission to access that page." }
                );
            }

            Initiative = await _context.DpDataInitiatives
                .Include(x => x.DpDataProjects)
                .ThenInclude(x => x.DpReportAnnotations)
                .Include(x => x.DpContactLinks)
                .Include(x => x.OperationOwner)
                .Include(x => x.ExecutiveOwner)
                .Include(x => x.FinancialImpactNavigation)
                .Include(x => x.StrategicImportanceNavigation)
                .SingleAsync(x => x.DataInitiativeId == id);

            return Page();
        }

        public IActionResult OnPostAsync(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                22
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { id = id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { id = id, error = "The data submitted was invalid." }
                );
            }

            // we get a copy of the initiative and then will only update several fields.
            DpDataInitiative NewInitiative = _context.DpDataInitiatives
                .Where(m => m.DataInitiativeId == Initiative.DataInitiativeId)
                .FirstOrDefault();

            // update last update values & values that were posted
            NewInitiative.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewInitiative.LastUpdateDate = DateTime.Now;
            NewInitiative.Name = Initiative.Name;
            NewInitiative.Description = Initiative.Description;
            NewInitiative.OperationOwnerId = Initiative.OperationOwnerId;
            NewInitiative.ExecutiveOwnerId = Initiative.ExecutiveOwnerId;
            NewInitiative.FinancialImpact = Initiative.FinancialImpact;
            NewInitiative.StrategicImportance = Initiative.StrategicImportance;

            _context.Attach(NewInitiative).State = EntityState.Modified;
            _context.SaveChanges();

            // updated any linked data projects that were added and remove any that were delinked.
            _context.DpDataProjects
                .Where(d => Projects.Contains(d.DataProjectId))
                .ToList()
                .ForEach(x => x.DataInitiativeId = Initiative.DataInitiativeId);

            _context.SaveChanges();

            _context.DpDataProjects
                .Where(d => d.DataInitiativeId == Initiative.DataInitiativeId)
                .Where(d => !Projects.Contains(d.DataProjectId))
                .ToList()
                .ForEach(x => x.DataInitiativeId = null);

            _context.SaveChanges();

            return RedirectToPage(
                "/Initiatives/Index",
                new { id = id, success = "Changes saved." }
            );
        }
    }
}
