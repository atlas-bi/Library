using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas_Web.Pages.Initiatives
{
    public class EditModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public EditModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public Initiative Initiative { get; set; }

        [BindProperty]
        public List<Collection> Collections { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
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
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            Initiative = await _context.Initiatives
                .Include(x => x.Collections)
                .ThenInclude(x => x.DpReportAnnotations)
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
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { id, error = "The data submitted was invalid." }
                );
            }

            // we get a copy of the initiative and then will only update several fields.
            Initiative NewInitiative = _context.Initiatives
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
            _context.Collections
                .Where(d => Collections.Select(x => x.DataProjectId).Contains(d.DataProjectId))
                .ToList()
                .ForEach(x => x.DataInitiativeId = Initiative.DataInitiativeId);

            _context.SaveChanges();

            _context.Collections
                .Where(d => d.DataInitiativeId == Initiative.DataInitiativeId)
                .Where(d => !Collections.Select(x => x.DataProjectId).Contains(d.DataProjectId))
                .ToList()
                .ForEach(x => x.DataInitiativeId = null);

            _context.SaveChanges();

            _cache.Remove("initiative-" + Initiative.DataInitiativeId);
            _cache.Remove("initatives");

            return RedirectToPage("/Initiatives/Index", new { id, success = "Changes saved." });
        }
    }
}
