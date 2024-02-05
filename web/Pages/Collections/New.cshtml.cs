using Atlas_Web.Authorization;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Collections
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
        public Collection Collection { get; set; }

        [BindProperty]
        public int[] Terms { get; set; }

        public IActionResult OnGet()
        {
            if (!User.HasPermission("Create Collection"))
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.HasPermission("Create Collection"))
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { error = "The data submitted was invalid." }
                );
            }

            // update last update values & values that were posted
            Collection.LastUpdateUser = User.GetUserId();
            Collection.LastUpdateDate = DateTime.Now;

            _context.Add(Collection);
            await _context.SaveChangesAsync();

            _cache.Remove("collections");

            return RedirectToPage(
                "/Collections/Index",
                new { id = Collection.CollectionId, success = "Changes saved." }
            );
        }
    }
}
