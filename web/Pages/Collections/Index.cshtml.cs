using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.Extensions.Caching.Memory;

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
                            // for authentication
                            .Include(x => x.CollectionReports)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectHierarchyChildReportObjects)
                            .ThenInclude(x => x.ParentReportObject)
                            .ThenInclude(x => x.ReportGroupsMemberships)
                            .AsNoTracking()
                            .SingleAsync(x => x.CollectionId == id);
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
                    return _context.Collections.Include(x => x.StarredCollections).ToListAsync();
                }
            );

            return Page();
        }

        public async Task<ActionResult> OnGetDeleteCollection(int Id)
        {
            if (!User.HasPermission("Delete Collection"))
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new
                    {
                        id = Collection.CollectionId,
                        error = "You do not have permission to access that page."
                    }
                );
            }

            // delete report annotations and term annotations
            // then delete collection and save.
            _context.RemoveRange(_context.CollectionReports.Where(m => m.CollectionId == Id));
            await _context.SaveChangesAsync();
            _context.RemoveRange(_context.CollectionTerms.Where(m => m.CollectionId == Id));
            await _context.SaveChangesAsync();

            _context.Remove(_context.Collections.Where(m => m.CollectionId == Id).FirstOrDefault());
            await _context.SaveChangesAsync();

            _cache.Remove("collection-" + Id);
            _cache.Remove("collections");

            return RedirectToPage("/Collections/Index");
        }
    }
}
