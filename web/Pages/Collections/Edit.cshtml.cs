using Atlas_Web.Authorization;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Collections
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
        public Collection Collection { get; set; }

        [BindProperty]
        public List<CollectionTerm> Terms { get; set; }

        [BindProperty]
        public List<CollectionReport> Reports { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.HasPermission("Edit Collection"))
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            Collection = await _context
                .Collections.Include(x => x.CollectionReports)
                .ThenInclude(x => x.Report)
                .Include(x => x.CollectionTerms)
                .ThenInclude(x => x.Term)
                .SingleAsync(x => x.CollectionId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!User.HasPermission("Edit Collection"))
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id, error = "The data submitted was invalid." }
                );
            }

            // we get a copy of the initiative and then will only update several fields.
            Collection NewCollection = await _context.Collections.FindAsync(
                Collection.CollectionId
            );

            // update last update values & values that were posted
            NewCollection.LastUpdateUser = User.GetUserId();
            NewCollection.LastUpdateDate = DateTime.Now;
            NewCollection.Name = Collection.Name;
            NewCollection.Description = Collection.Description;
            NewCollection.Purpose = Collection.Purpose;
            NewCollection.Hidden = Collection.Hidden;
            _context.Attach(NewCollection).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // updated any linked terms that were added and remove any that were delinked.
            _cache.Remove("terms");
            foreach (var term in Terms.Distinct())
            {
                term.CollectionId = NewCollection.CollectionId;

                if (
                    !await _context.CollectionTerms.AnyAsync(x =>
                        x.TermId == term.TermId && x.CollectionId == term.CollectionId
                    )
                )
                {
                    // clear term cache
                    _cache.Remove("term-" + term.TermId);
                    await _context.AddAsync(term);
                }
            }
            await _context.SaveChangesAsync();

            var RemovedTerms = _context
                .CollectionTerms.Where(d => d.CollectionId == NewCollection.CollectionId)
                .Where(d => !Terms.Select(x => x.TermId).Contains(d.TermId));

            foreach (var term in await RemovedTerms.ToListAsync())
            {
                _cache.Remove("term-" + term.TermId);
            }

            _context.RemoveRange(RemovedTerms);
            await _context.SaveChangesAsync();

            // update linked reports
            for (int i = 0; i < Reports.Count; i++)
            {
                CollectionReport report = Reports[i];

                // clear report cache
                _cache.Remove("report-" + report.ReportId);

                report.CollectionId = NewCollection.CollectionId;
                report.Rank = i;

                // if annotation exists, update rank and text
                CollectionReport oldReport = await _context
                    .CollectionReports.Where(x =>
                        x.ReportId == report.ReportId && x.CollectionId == report.CollectionId
                    )
                    .FirstOrDefaultAsync();
                if (oldReport != null)
                {
                    oldReport.Rank = i;
                }
                else
                {
                    await _context.AddAsync(report);
                }

                await _context.SaveChangesAsync();
            }

            var RemovedReports = _context
                .CollectionReports.Where(d => d.CollectionId == NewCollection.CollectionId)
                .Where(d => !Reports.Select(x => x.ReportId).Contains(d.ReportId));

            foreach (var report in await RemovedReports.ToListAsync())
            {
                _cache.Remove("report-" + report.ReportId);
            }

            _context.RemoveRange(RemovedReports);
            await _context.SaveChangesAsync();

            _cache.Remove("collection-" + NewCollection.CollectionId);
            _cache.Remove("search-collection-" + NewCollection.CollectionId);
            _cache.Remove("collections");

            return RedirectToPage("/Collections/Index", new { id, success = "Changes saved." });
        }
    }
}
