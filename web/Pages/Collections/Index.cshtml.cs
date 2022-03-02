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
        public DpDataProject Collection { get; set; }

        [BindProperty]
        public DpReportAnnotation DpReportAnnotation { get; set; }

        public IEnumerable<DpDataProject> Collections { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // if the id null then list all
            if (id != null)
            {
                Collection = await _cache.GetOrCreateAsync<DpDataProject>(
                    "collection-" + id,
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                        return _context.DpDataProjects
                            .Include(x => x.LastUpdateUserNavigation)
                            .Include(x => x.DpTermAnnotations)
                            .ThenInclude(x => x.Term)
                            .Include(x => x.DpTermAnnotations)
                            .ThenInclude(x => x.Term)
                            .ThenInclude(x => x.StarredTerms)
                            .Include(x => x.DpReportAnnotations)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectDoc)
                            .Include(x => x.DpReportAnnotations)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectType)
                            .Include(x => x.DpReportAnnotations)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.ReportObjectAttachments)
                            .Include(x => x.DpReportAnnotations)
                            .ThenInclude(x => x.Report)
                            .ThenInclude(x => x.StarredReports)
                            .Include(x => x.StarredCollections)
                            .SingleAsync(x => x.DataProjectId == id);
                    }
                );

                if (Collection != null)
                {
                    return Page();
                }
            }

            Collections = await _cache.GetOrCreateAsync<List<DpDataProject>>(
                "collections",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.DpDataProjects.ToListAsync();
                }
            );

            return Page();
        }

        public ActionResult OnGetDeleteCollection(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                27
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
            _context.RemoveRange(
                _context.DpDataProjectConversationMessages.Where(
                    x => x.DataProjectConversation.DataProjectId == Id
                )
            );
            _context.SaveChanges();
            _context.RemoveRange(
                _context.DpDataProjectConversations.Where(x => x.DataProjectId == Id)
            );
            _context.SaveChanges();
            _context.RemoveRange(_context.DpReportAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpTermAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();

            _context.Remove(
                _context.DpDataProjects.Where(m => m.DataProjectId == Id).FirstOrDefault()
            );
            _context.SaveChanges();

            _cache.Remove("collection-" + Id);
            _cache.Remove("collections");

            return RedirectToPage("/Collections/Index");
        }


    }
}
