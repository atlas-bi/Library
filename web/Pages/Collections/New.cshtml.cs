using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;

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
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                26
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                26
            );

            if (!checkpoint)
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
            Collection.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            Collection.LastUpdateDate = DateTime.Now;

            _context.Add(Collection);
            _context.SaveChanges();

            _cache.Remove("collections");

            return RedirectToPage(
                "/Collections/Index",
                new { id = Collection.DataProjectId, success = "Changes saved." }
            );
        }
    }
}
