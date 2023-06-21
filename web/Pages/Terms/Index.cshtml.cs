using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Terms
{
    public class IndexModel : PageModel
    {
        // import model
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<Term> Terms { get; set; }
        public Term Term { get; set; }

        public List<AdList> AdLists { get; set; }
        public List<ReportObject> RelatedReports { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentCollections", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;

            if (id == null)
            {
                Terms = await _cache.GetOrCreateAsync<List<Term>>(
                    "terms",
                    cacheEntry =>
                    {
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                        return _context.Terms
                            .Include(x => x.ApprovedByUser)
                            .Include(x => x.UpdatedByUser)
                            .Include(x => x.StarredTerms)
                            .AsNoTracking()
                            .ToListAsync();
                    }
                );

                return Page();
            }

            Term = await _cache.GetOrCreateAsync<Term>(
                "term-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.Terms
                        .Include(x => x.ApprovedByUser)
                        .Include(x => x.UpdatedByUser)
                        .Include(x => x.StarredTerms)
                        .AsNoTracking()
                        .SingleAsync(x => x.TermId == id);
                }
            );

            RelatedReports = _cache.GetOrCreate<List<ReportObject>>(
                "term-reports-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    var level_one = _context.ReportObjects
                        .Where(x => x.ReportObjectDoc.ReportObjectDocTerms.Any(y => y.TermId == id))
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectAttachments)
                        .Include(x => x.ReportTagLinks)
                        .ThenInclude(x => x.Tag)
                        // for authentication
                        .Include(x => x.ReportObjectHierarchyChildReportObjects)
                        .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
                        .AsNoTracking()
                        .ToList();

                    var level_two = _context.ReportObjects
                        .Where(
                            x =>
                                x.ReportObjectHierarchyParentReportObjects.Any(
                                    y =>
                                        y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(
                                            z => z.TermId == id
                                        )
                                )
                        )
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectAttachments)
                        .Include(x => x.ReportTagLinks)
                        .ThenInclude(x => x.Tag)
                        // for authentication
                        .Include(x => x.ReportObjectHierarchyChildReportObjects)
                        .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
                        .AsNoTracking()
                        .ToList();
                    var level_three = _context.ReportObjects
                        .Where(
                            x =>
                                x.ReportObjectHierarchyParentReportObjects.Any(
                                    g =>
                                        g.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(
                                            y =>
                                                y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(
                                                    z => z.TermId == id
                                                )
                                        )
                                )
                        )
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectAttachments)
                        .Include(x => x.ReportTagLinks)
                        .ThenInclude(x => x.Tag)
                        // for authentication
                        .Include(x => x.ReportObjectHierarchyChildReportObjects)
                        .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
                        .AsNoTracking()
                        .ToList();
                    var level_four = _context.ReportObjects
                        .Where(
                            x =>
                                x.ReportObjectHierarchyParentReportObjects.Any(
                                    gg =>
                                        gg.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(
                                            g =>
                                                g.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(
                                                    y =>
                                                        y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(
                                                            z => z.TermId == id
                                                        )
                                                )
                                        )
                                )
                        )
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectAttachments)
                        .Include(x => x.ReportTagLinks)
                        .ThenInclude(x => x.Tag)
                        // for authentication
                        .Include(x => x.ReportObjectHierarchyChildReportObjects)
                        .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
                        .AsNoTracking()
                        .ToList();

                    return level_one.Union(level_two).Union(level_three).Union(level_four).ToList();
                }
            );

            return Page();
        }

        public ActionResult OnGetDeleteTerm(int Id)
        {
            if (!_context.Terms.Any(x => x.TermId == Id))
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id = Id, error = "That term does not exist." }
                );
            }

            Term OldTerm = _context.Terms.Where(x => x.TermId == Id).First();

            if (
                (OldTerm.ApprovedYn == "Y" && !User.HasPermission("Delete Approved Terms"))
                || (OldTerm.ApprovedYn != "Y" && !User.HasPermission("Delete Unapproved Terms"))
            )
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id = Id, error = "You do not have permission to access that page." }
                );
            }

            // delete:
            //  replies
            //  report links
            //  initiative term annotations

            _context.RemoveRange(_context.ReportObjectDocTerms.Where(x => x.TermId == Id));
            _context.RemoveRange(_context.CollectionTerms.Where(x => x.TermId == Id));
            _context.Remove(_context.Terms.Where(x => x.TermId == Id).FirstOrDefault());
            _context.SaveChanges();

            _cache.Remove("terms");
            _cache.Remove("term-" + Id);
            _cache.Remove("term-reports-" + Id);

            return RedirectToPage("/Terms/Index");
        }
    }
}
