using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;

namespace Atlas_Web.Pages.Settings
{
    public class SearchModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public SearchModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public GlobalSiteSetting UserVisibility { get; set; }
        public GlobalSiteSetting InitiativeVisibility { get; set; }
        public GlobalSiteSetting CollectionVisibility { get; set; }
        public GlobalSiteSetting GroupVisibility { get; set; }
        public GlobalSiteSetting TermVisibility { get; set; }

        public List<ReportObjectType> ReportTypes { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ReportTypes = await _context.ReportObjectTypes.ToListAsync();

            UserVisibility = await _context.GlobalSiteSettings.SingleOrDefaultAsync(
                x => x.Name == "users_search_visibility"
            );
            GroupVisibility = await _context.GlobalSiteSettings.SingleOrDefaultAsync(
                x => x.Name == "groups_search_visibility"
            );
            TermVisibility = await _context.GlobalSiteSettings.SingleOrDefaultAsync(
                x => x.Name == "terms_search_visibility"
            );
            InitiativeVisibility = await _context.GlobalSiteSettings.SingleOrDefaultAsync(
                x => x.Name == "initiatives_search_visibility"
            );
            CollectionVisibility = await _context.GlobalSiteSettings.SingleOrDefaultAsync(
                x => x.Name == "collections_search_visibility"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostSearchUpdateVisibility(
            string TypeId,
            int? GroupId,
            int Type
        )
        {
            // type 1 = add
            // type 2 = remove

            if (TypeId == "reports" && GroupId != null)
            {
                var report_type = await _context.ReportObjectTypes
                    .Where(x => x.ReportObjectTypeId == GroupId)
                    .FirstOrDefaultAsync();
                if (report_type != null && Type == 2)
                {
                    report_type.Visible = "N";
                }
                else if (report_type != null)
                {
                    report_type.Visible = "Y";
                }
            }
            else
            {
                var current_vis = await _context.GlobalSiteSettings
                    .Where(x => x.Name == TypeId + "_search_visibility")
                    .FirstOrDefaultAsync();

                if (current_vis == null)
                {
                    _context.Add(
                        new GlobalSiteSetting { Name = TypeId + "_search_visibility", Value = "Y" }
                    );
                }
                else
                {
                    if (Type == 2)
                    {
                        current_vis.Value = "N";
                    }
                    else
                    {
                        current_vis.Value = "Y";
                    }
                }
            }
            _context.SaveChanges();
            return Content("success");
        }

        public async Task<IActionResult> OnPostSearchUpdateText(int id, string text)
        {
            var report_type = await _context.ReportObjectTypes
                .Where(x => x.ReportObjectTypeId == id)
                .FirstOrDefaultAsync();

            report_type.ShortName = text;

            _context.SaveChanges();
            return Content("success");
        }
    }
}
