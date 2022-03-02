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

namespace Atlas_Web.Pages.Terms
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
        public Term Term { get; set; }

        [BindProperty]
        public List<Term> Terms { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var checkpoint_unapproved = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                13
            );

            var checkpoint_approved = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                11
            );

            Term = await _context.Terms.SingleAsync(x => x.TermId == id);

            if (
                (Term.ApprovedYn == "Y" && !checkpoint_approved)
                || (Term.ApprovedYn != "Y" && !checkpoint_unapproved)
            )
            {
                return RedirectToPage(
                    "/Terms/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            return Page();
        }

        public IActionResult OnPostAsync(int id)
        {
            var checkpoint_unapproved = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                13
            );

            var checkpoint_approved = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                11
            );

            // we get a copy of the Term and then will only update several fields.
            Term NewTerm = _context.Terms.Single(x => x.TermId == Term.TermId);

            if (
                (Term.ApprovedYn == "Y" && !checkpoint_approved)
                || (Term.ApprovedYn != "Y" && !checkpoint_unapproved)
            )
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
            NewTerm.UpdatedByUserId =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewTerm.LastUpdatedDateTime = DateTime.Now;
            NewTerm.Name = Term.Name;
            NewTerm.TechnicalDefinition = Term.TechnicalDefinition;

            if (NewTerm.ApprovedYn != "Y" && Term.ApprovedYn == "Y")
            {
                NewTerm.ApprovalDateTime = DateTime.Now;
                NewTerm.ApprovedByUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            }
            else if (Term.ApprovedYn != "Y")
            {
                NewTerm.ApprovalDateTime = null;
            }

            NewTerm.ApprovedYn = Term.ApprovedYn;

            _context.Attach(NewTerm).State = EntityState.Modified;
            _context.SaveChanges();

            _cache.Remove("terms");
            _cache.Remove("term-" + NewTerm.TermId);
            _cache.Remove("term_reports-" + NewTerm.TermId);

            return RedirectToPage("/Terms/Index", new { id, success = "Changes saved." });
        }
    }
}
