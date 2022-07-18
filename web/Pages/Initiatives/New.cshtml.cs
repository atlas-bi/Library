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

namespace Atlas_Web.Pages.Initiatives
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
        public Initiative Initiative { get; set; }

        [BindProperty]
        public List<Collection> Collections { get; set; }

        public IActionResult OnGet()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                "Create Initiative"
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                "Create Initiative"
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Initiatives/Index",
                    new { error = "The data submitted was invalid." }
                );
            }

            // update last update values & values that were posted
            Initiative.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            Initiative.LastUpdateDate = DateTime.Now;

            await _context.AddAsync(Initiative);
            await _context.SaveChangesAsync();

            // updated any linked data projects that were added and remove any that were delinked.
            _cache.Remove("collections");

            var AddedCollections = await _context.Collections
                .Where(d => Collections.Select(x => x.DataProjectId).Contains(d.DataProjectId))
                .ToListAsync();

            foreach (var collection in AddedCollections)
            {
                _cache.Remove("collection-" + collection.DataProjectId);
                collection.DataInitiativeId = Initiative.DataInitiativeId;
                await _context.SaveChangesAsync();
            }

            _cache.Remove("initatives");

            return RedirectToPage(
                "/Initiatives/Index",
                new { id = Initiative.DataInitiativeId, success = "Changes saved." }
            );
        }
    }
}
