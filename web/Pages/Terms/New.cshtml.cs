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

namespace Atlas_Web.Pages.Terms
{
    public class NewModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public NewModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public List<int?> Permissions { get; set; }
        public User PublicUser { get; set; }

        [BindProperty]
        public Term Term { get; set; }

        [BindProperty]
        public List<Term> Terms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                7
            );

            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            ViewData["Fullname"] = MyUser.Fullname_Cust;

            if (!checkpoint)
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
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                7
            );

            var checkpoint_approve = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                6
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id = id, error = "You do not have permission to access that page." }
                );
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id = id, error = "The data submitted was invalid." }
                );
            }

            // update last update values & values that were posted
            Term.UpdatedByUserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            Term.LastUpdatedDateTime = DateTime.Now;

            if (checkpoint_approve && Term.ApprovedYn == "Y")
            {
                Term.ApprovalDateTime = DateTime.Now;
                Term.ApprovedByUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            }

            _context.Add(Term);
            _context.SaveChanges();

            return RedirectToPage(
                "/Terms/Index",
                new { id = Term.TermId, success = "Changes saved." }
            );
        }
    }
}
