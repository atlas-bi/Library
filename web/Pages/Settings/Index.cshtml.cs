using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Settings
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public GlobalSiteSetting GlobalSiteSettings { get; set; }

        public List<GlobalSiteSetting> Messages { get; set; }

        public async Task<IActionResult> OnGetSiteMessages()
        {
            Messages = await _context.GlobalSiteSettings.Where(i => i.Name == "msg").ToListAsync();

            return new PartialViewResult
            {
                ViewName = "Partials/_SiteMessages",
                ViewData = ViewData
            };
        }

        public ActionResult OnGetDeleteSiteMessage(int Id)
        {
            if (User.HasPermission("Manage Global Site Settings"))
            {
                _context.Remove(
                    _context.GlobalSiteSettings.Where(x => x.Id == Id).FirstOrDefault()
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostAddSiteMessage()
        {
            if (ModelState.IsValid && User.HasPermission("Manage Global Site Settings"))
            {
                _context.Add(GlobalSiteSettings);
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }

        public async Task<IActionResult> OnGetEtl()
        {
            ViewData["ReportTagEtl"] = await _context.GlobalSiteSettings
                .Where(x => x.Name == "report_tag_etl")
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            return new PartialViewResult { ViewName = "Partials/_Etl", ViewData = ViewData };
        }

        public async Task<IActionResult> OnPostUpdateReportTagsEtl()
        {
            var report_tag_etl = await _context.GlobalSiteSettings
                .Where(x => x.Name == "report_tag_etl")
                .FirstOrDefaultAsync();

            if (report_tag_etl != null)
            {
                report_tag_etl.Value = GlobalSiteSettings.Value;
            }
            else
            {
                await _context.AddAsync(
                    new GlobalSiteSetting
                    {
                        Name = "report_tag_etl",
                        Value = GlobalSiteSettings.Value
                    }
                );
            }

            await _context.SaveChangesAsync();

            _cache.Set("global_css", GlobalSiteSettings.Value);

            return RedirectToPage("/Settings/Index");
        }

        public async Task<IActionResult> OnGetDefaultEtl()
        {
            string text = await System.IO.File.ReadAllTextAsync(
                "wwwroot/defaults/report_tags_etl.sql"
            );

            return Content(text);
        }

        public async Task<IActionResult> OnGetTheme()
        {
            ViewData["GlobalCss"] = await _context.GlobalSiteSettings
                .Where(x => x.Name == "global_css")
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            return new PartialViewResult { ViewName = "Partials/_Theme", ViewData = ViewData };
        }

        public ActionResult OnPostUpdateGlobalCss()
        {
            var global_css = _context.GlobalSiteSettings
                .Where(x => x.Name == "global_css")
                .FirstOrDefault();

            if (global_css != null)
            {
                global_css.Value = GlobalSiteSettings.Value;
            }
            else
            {
                _context.Add(
                    new GlobalSiteSetting { Name = "global_css", Value = GlobalSiteSettings.Value }
                );
            }

            _context.SaveChanges();

            _cache.Set("global_css", GlobalSiteSettings.Value);

            return RedirectToPage("/Settings/Index");
        }
    }
}
