using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Initiatives
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public Initiative Initiative { get; set; }

        public IEnumerable<Initiative> Initiatives { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // if the id null then list all
            if (id != null)
            {
                Initiative = await _cache.GetOrCreateAsync<Initiative>(
                    "initiative-" + id,
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                        return _context.Initiatives
                            .Include(x => x.Collections)
                            .ThenInclude(x => x.CollectionReports)
                            .Include(x => x.OperationOwner)
                            .Include(x => x.ExecutiveOwner)
                            .Include(x => x.FinancialImpactNavigation)
                            .Include(x => x.StrategicImportanceNavigation)
                            .Include(x => x.LastUpdateUserNavigation)
                            .Include(x => x.StarredInitiatives)
                            .AsNoTracking()
                            .SingleAsync(x => x.InitiativeId == id);
                    }
                );

                if (Initiative != null)
                {
                    return Page();
                }
            }

            Initiatives = await _cache.GetOrCreateAsync<List<Initiative>>(
                "initiatives",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    return _context.Initiatives
                        .Include(x => x.Collections)
                        .Include(x => x.StarredInitiatives)
                        .ToListAsync();
                }
            );

            return Page();
        }

        public async Task<ActionResult> OnGetDeleteInitiative(int Id)
        {
            if (!User.HasPermission("Delete Initiative"))
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            // remove collection links, contacts and remove initiative.
            (await _context.Collections.Where(d => d.InitiativeId == Id).ToListAsync()).ForEach(
                x => x.InitiativeId = null
            );

            _context.Remove(
                await _context.Initiatives.Where(x => x.InitiativeId == Id).FirstOrDefaultAsync()
            );
            await _context.SaveChangesAsync();

            _cache.Remove("initiative-" + Id);
            _cache.Remove("initiatives");
            return RedirectToPage("/Initiatives/Index");
        }
    }
}
