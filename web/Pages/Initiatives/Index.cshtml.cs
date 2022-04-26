using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System;

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

        public string Favorite { get; set; }

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
                            .SingleAsync(x => x.DataInitiativeId == id);
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

        public ActionResult OnGetDeleteInitiative(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                21
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            // remove project links, contacts and remove initiative.
            _context.Collections
                .Where(d => d.DataInitiativeId == Id)
                .ToList()
                .ForEach(x => x.DataInitiativeId = null);

            _context.Remove(
                _context.Initiatives.Where(x => x.DataInitiativeId == Id).FirstOrDefault()
            );
            _context.SaveChanges();

            _cache.Remove("initiative-" + Id);
            _cache.Remove("initiatives");
            return RedirectToPage("/Initiatives/Index");
        }
    }
}
