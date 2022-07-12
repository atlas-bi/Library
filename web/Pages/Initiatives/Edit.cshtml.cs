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
                .ThenInclude(x => x.CollectionReports)
                .Include(x => x.OperationOwner)
                .Include(x => x.ExecutiveOwner)
                .Include(x => x.FinancialImpactNavigation)
                .Include(x => x.StrategicImportanceNavigation)
                .SingleAsync(x => x.DataInitiativeId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
            Initiative NewInitiative = await _context.Initiatives
                .Where(m => m.DataInitiativeId == Initiative.DataInitiativeId)
                .FirstOrDefaultAsync();

            // update last update values & values that were posted
            NewInitiative.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewInitiative.LastUpdateDate = DateTime.Now;
            NewInitiative.Name = Initiative.Name;
            NewInitiative.Description = Initiative.Description;
            NewInitiative.OperationOwnerId = Initiative.OperationOwnerId;
            NewInitiative.ExecutiveOwnerId = Initiative.ExecutiveOwnerId;
            NewInitiative.FinancialImpact = Initiative.FinancialImpact;
            NewInitiative.Hidden = Initiative.Hidden;
            NewInitiative.StrategicImportance = Initiative.StrategicImportance;

            _context.Attach(NewInitiative).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // updated any linked data projects that were added and remove any that were delinked.
            var AddedCollections = await _context.Collections
                .Where(d => Collections.Select(x => x.DataProjectId).Contains(d.DataProjectId))
                .ToListAsync();

            _cache.Remove("collections");
            foreach (var collection in AddedCollections)
            {
                _cache.Remove("collection-" + collection.DataProjectId);

                collection.DataInitiativeId = Initiative.DataInitiativeId;
                await _context.SaveChangesAsync();
            }

            var RemovedCollections = await _context.Collections
                .Where(d => d.DataInitiativeId == Initiative.DataInitiativeId)
                .Where(d => !Collections.Select(x => x.DataProjectId).Contains(d.DataProjectId))
                .ToListAsync();

            foreach (var collection in RemovedCollections)
            {
                _cache.Remove("collection-" + collection.DataProjectId);
                collection.DataInitiativeId = null;
                await _context.SaveChangesAsync();
            }

            _cache.Remove("initiative-" + Initiative.DataInitiativeId);
            _cache.Remove("initatives");

            return RedirectToPage("/Initiatives/Index", new { id, success = "Changes saved." });
        }
    }
}
