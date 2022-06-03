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
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            Collection = await _context.Collections
                .Include(x => x.CollectionReports)
                .ThenInclude(x => x.Report)
                .Include(x => x.CollectionTerms)
                .ThenInclude(x => x.Term)
                .SingleAsync(x => x.DataProjectId == id);

            return Page();
        }

        public IActionResult OnPostAsync(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );

            if (!checkpoint)
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
            Collection NewCollection = _context.Collections.Find(Collection.DataProjectId);

            // update last update values & values that were posted
            NewCollection.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewCollection.LastUpdateDate = DateTime.Now;
            NewCollection.Name = Collection.Name;
            NewCollection.Description = Collection.Description;
            NewCollection.Purpose = Collection.Purpose;

            _context.Attach(NewCollection).State = EntityState.Modified;
            _context.SaveChanges();

            // updated any linked terms that were added and remove any that were delinked.
            _cache.Remove("terms");
            foreach (var term in Terms.Distinct())
            {
                term.DataProjectId = NewCollection.DataProjectId;

                if (
                    !_context.CollectionTerms.Any(
                        x => x.TermId == term.TermId && x.DataProjectId == term.DataProjectId
                    )
                )
                {
                    // clear term cache
                    _cache.Remove("term-" + term.TermId);
                    _context.Add(term);
                }
            }
            _context.SaveChanges();

            var RemovedTerms = _context.CollectionTerms
                .Where(d => d.DataProjectId == NewCollection.DataProjectId)
                .Where(d => !Terms.Select(x => x.TermId).Contains((int)d.TermId));

            foreach (var term in RemovedTerms)
            {
                _cache.Remove("term-" + term.TermId);
            }

            _context.RemoveRange(RemovedTerms);
            _context.SaveChanges();

            // update linked reports
            for (int i = 0; i < Reports.Count; i++)
            {
                CollectionReport report = Reports[i];

                // clear report cache
                _cache.Remove("report-" + report.ReportId);

                report.DataProjectId = NewCollection.DataProjectId;
                report.Rank = i;

                // if annotation exists, update rank and text
                CollectionReport oldReport = _context.CollectionReports
                    .Where(
                        x =>
                            x.ReportId == report.ReportId && x.DataProjectId == report.DataProjectId
                    )
                    .FirstOrDefault();
                if (oldReport != null)
                {
                    oldReport.Rank = i;
                }
                else
                {
                    _context.Add(report);
                }

                _context.SaveChanges();
            }

            var RemovedReports = _context.CollectionReports
                .Where(d => d.DataProjectId == NewCollection.DataProjectId)
                .Where(d => !Reports.Select(x => x.ReportId).Contains((int)d.ReportId));

            foreach (var report in RemovedReports)
            {
                _cache.Remove("report-" + report.ReportId);
            }

            _context.RemoveRange(RemovedReports);
            _context.SaveChanges();

            _cache.Remove("collection-" + NewCollection.DataProjectId);
            _cache.Remove("search-collection-" + NewCollection.DataProjectId);
            _cache.Remove("collections");

            return RedirectToPage("/Collections/Index", new { id, success = "Changes saved." });
        }
    }
}
