using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas_Web.Pages.Collections
{
    public class EditModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public EditModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        [BindProperty]
        public DpDataProject Collection { get; set; }

        [BindProperty]
        public List<DpTermAnnotation> Terms { get; set; }

        [BindProperty]
        public List<DpReportAnnotation> Reports { get; set; }

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
                    new { id = id, error = "You do not have permission to access that page." }
                );
            }

            Collection = await _context.DpDataProjects
                .Include(x => x.DpReportAnnotations)
                .ThenInclude(x => x.Report)
                .Include(x => x.DpTermAnnotations)
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
                    new { id = id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id = id, error = "The data submitted was invalid." }
                );
            }

            // we get a copy of the initiative and then will only update several fields.
            DpDataProject NewCollection = _context.DpDataProjects.Find(Collection.DataProjectId);

            // update last update values & values that were posted
            NewCollection.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewCollection.LastUpdateDate = DateTime.Now;
            NewCollection.Name = Collection.Name;
            NewCollection.Description = Collection.Description;

            _context.Attach(NewCollection).State = EntityState.Modified;
            _context.SaveChanges();

            // updated any linked terms that were added and remove any that were delinked.
            foreach (var term in Terms.Distinct())
            {
                term.DataProjectId = NewCollection.DataProjectId;

                if (
                    !_context.DpTermAnnotations.Any(
                        x => x.TermId == term.TermId && x.DataProjectId == term.DataProjectId
                    )
                )
                {
                    _context.Add(term);
                }
            }
            _context.SaveChanges();

            _context.RemoveRange(
                _context.DpTermAnnotations
                    .Where(d => d.DataProjectId == NewCollection.DataProjectId)
                    .Where(d => !Terms.Select(x => x.TermId).Contains((int)d.TermId))
            );
            _context.SaveChanges();

            // update linked reports
            for (int i = 0; i < Reports.Count; i++)
            {
                DpReportAnnotation report = Reports[i];

                report.DataProjectId = NewCollection.DataProjectId;
                report.Rank = i;

                // if annotation exists, update rank and text
                DpReportAnnotation oldReport = _context.DpReportAnnotations
                    .Where(
                        x =>
                            x.ReportId == report.ReportId && x.DataProjectId == report.DataProjectId
                    )
                    .FirstOrDefault();
                if (oldReport != null)
                {
                    oldReport.Rank = i;
                    oldReport.Annotation = report.Annotation;
                }
                else
                {
                    _context.Add(report);
                }

                _context.SaveChanges();
            }

            _context.RemoveRange(
                _context.DpReportAnnotations
                    .Where(d => d.DataProjectId == NewCollection.DataProjectId)
                    .Where(d => !Reports.Select(x => x.ReportId).Contains((int)d.ReportId))
            );
            _context.SaveChanges();

            _cache.Remove("collection-" + NewCollection.DataProjectId);
            _cache.Remove("collections");

            return RedirectToPage(
                "/Collections/Index",
                new { id = id, success = "Changes saved." }
            );
        }
    }
}
