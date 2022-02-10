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

        public DpDataInitiative Initiative { get; set; }

        public IEnumerable<DpDataInitiative> Initiatives { get; set; }

        public string Favorite { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // if the id null then list all
            if (id != null)
            {
                Initiative = await _cache.GetOrCreateAsync<DpDataInitiative>("initiative-" + id,
                    cacheEntry => {
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                        return _context.DpDataInitiatives
                                .Include(x => x.DpDataProjects)
                                .ThenInclude(x => x.DpReportAnnotations)
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

            Initiatives = await _cache.GetOrCreateAsync<List<DpDataInitiative>>("initiatives",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    return _context.DpDataInitiatives
                            .Include(x => x.DpDataProjects)
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
            _context.DpDataProjects
                .Where(d => d.DataInitiativeId == Id)
                .ToList()
                .ForEach(x => x.DataInitiativeId = null);

            _context.Remove(
                _context.DpDataInitiatives.Where(x => x.DataInitiativeId == Id).FirstOrDefault()
            );
            _context.SaveChanges();

            _cache.Remove("initiative-" + Id);
            _cache.Remove("initiatives");
            return RedirectToPage("/Initiatives/Index");
        }
    }
}
