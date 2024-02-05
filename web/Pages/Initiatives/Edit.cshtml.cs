using Atlas_Web.Authorization;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
            if (!User.HasPermission("Edit Initiative"))
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            Initiative = await _context
                .Initiatives.Include(x => x.Collections)
                .ThenInclude(x => x.CollectionReports)
                .Include(x => x.OperationOwner)
                .Include(x => x.ExecutiveOwner)
                .Include(x => x.FinancialImpactNavigation)
                .Include(x => x.StrategicImportanceNavigation)
                .SingleAsync(x => x.InitiativeId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!User.HasPermission("Edit Initiative"))
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
            Initiative NewInitiative = await _context
                .Initiatives.Where(m => m.InitiativeId == Initiative.InitiativeId)
                .FirstOrDefaultAsync();

            // update last update values & values that were posted
            NewInitiative.LastUpdateUser = User.GetUserId();
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

            // updated any linked collections that were added and remove any that were delinked.
            var AddedCollections = await _context
                .Collections.Where(d =>
                    Collections.Select(x => x.CollectionId).Contains(d.CollectionId)
                )
                .ToListAsync();

            _cache.Remove("collections");
            foreach (var collection in AddedCollections)
            {
                _cache.Remove("collection-" + collection.CollectionId);

                collection.InitiativeId = Initiative.InitiativeId;
                await _context.SaveChangesAsync();
            }

            var RemovedCollections = await _context
                .Collections.Where(d => d.InitiativeId == Initiative.InitiativeId)
                .Where(d => !Collections.Select(x => x.CollectionId).Contains(d.CollectionId))
                .ToListAsync();

            foreach (var collection in RemovedCollections)
            {
                _cache.Remove("collection-" + collection.CollectionId);
                collection.InitiativeId = null;
                await _context.SaveChangesAsync();
            }

            _cache.Remove("initiative-" + Initiative.InitiativeId);
            _cache.Remove("initiatives");

            return RedirectToPage("/Initiatives/Index", new { id, success = "Changes saved." });
        }
    }
}
