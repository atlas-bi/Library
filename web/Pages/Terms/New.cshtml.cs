using Atlas_Web.Authorization;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Terms
{
    public class NewModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public NewModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public Term Term { get; set; }

        public IActionResult OnGetAsync()
        {
            if (!User.HasPermission("Create New Terms"))
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            return Page();
        }

        public IActionResult OnPostAsync(int id)
        {
            if (!User.HasPermission("Create New Terms"))
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id, error = "The data submitted was invalid." }
                );
            }

            // update last update values & values that were posted
            Term.UpdatedByUserId = User.GetUserId();
            Term.LastUpdatedDateTime = DateTime.Now;

            if (User.HasPermission("Approve Terms") && Term.ApprovedYn == "Y")
            {
                Term.ApprovalDateTime = DateTime.Now;
                Term.ApprovedByUserId = User.GetUserId();
            }

            _context.Add(Term);
            _context.SaveChanges();

            _cache.Remove("terms");

            return RedirectToPage(
                "/Terms/Index",
                new { id = Term.TermId, success = "Changes saved." }
            );
        }
    }
}
