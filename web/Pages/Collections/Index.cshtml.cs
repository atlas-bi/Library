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

namespace Atlas_Web.Pages.Collections
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

        [BindProperty]
        public Collection Collection { get; set; }

        public IEnumerable<Collection> Collections { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id = null)
        {
            // if the id null then list all
            if (id != null)
            {
                Collection = await _cache.GetOrCreateAsync<Collection>(
                    "collection-" + id,
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                        return _context.Collections
                            .Include(x => x.LastUpdateUserNavigation)
                            .Include(x => x.CollectionTerms)
                            .ThenInclude(x => x.Term)
                            .Include(x => x.CollectionTerms)
                            .ThenInclude(x => x.Term)
                            .ThenInclude(x => x.StarredTerms)
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectDoc)
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectType)
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectAttachments)
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.StarredReports)
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportTagLinks)
                            .ThenInclude(x => x.Tag)
                            .Include(x => x.StarredCollections)
                            .Include(x => x.Initiative)
                            .AsNoTracking()
                            .SingleAsync(x => x.DataProjectId == id);
                    }
                );

                if (Collection != null)
                {
                    return Page();
                }
            }

            Collections = await _cache.GetOrCreateAsync<List<Collection>>(
                "collections",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.Collections.ToListAsync();
                }
            );

            return Page();
        }

        public async Task<ActionResult> OnGetDeleteCollection(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                "Delete Project"
            );
            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new
                    {
                        id = Collection.DataProjectId,
                        error = "You do not have permission to access that page."
                    }
                );
            }

            // delete report annotations and term annotations
            // then delete project and save.
            _context.RemoveRange(_context.CollectionReports.Where(m => m.DataProjectId == Id));
            await _context.SaveChangesAsync();
            _context.RemoveRange(_context.CollectionTerms.Where(m => m.DataProjectId == Id));
            await _context.SaveChangesAsync();

            _context.Remove(
                _context.Collections.Where(m => m.DataProjectId == Id).FirstOrDefault()
            );
            await _context.SaveChangesAsync();

            _cache.Remove("collection-" + Id);
            _cache.Remove("collections");

            return RedirectToPage("/Collections/Index");
        }


    }
}
