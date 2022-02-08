using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Settings
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public GlobalSiteSetting GlobalSiteSettings { get; set; }

        public List<GlobalSiteSetting> Messages { get; set; }

        public class GlobalSettingsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
        }

        public async Task<IActionResult> OnGetSiteMessages()
        {
            Messages = await _context.GlobalSiteSettings.Where(i => i.Name == "msg").ToListAsync();

            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );
            return new PartialViewResult()
            {
                ViewName = "Partials/_SiteMessages",
                ViewData = ViewData
            };
        }

        public ActionResult OnGetDeleteSiteMessage(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                45
            );
            if (checkpoint)
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
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                45
            );
            if (ModelState.IsValid && checkpoint)
            {
                _context.Add(GlobalSiteSettings);
                _context.SaveChanges();
            }

            return RedirectToPage("/Settings/Index");
        }

        public async Task<IActionResult> OnGetEtl()
        {
            return new PartialViewResult() { ViewName = "Partials/_Etl", ViewData = ViewData };
        }

        public async Task<IActionResult> OnGetTheme()
        {
            ViewData["GlobalCss"] = await _context.GlobalSiteSettings
                .Where(x => x.Name == "global_css")
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );
            //return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Partials/_Theme", ViewData = ViewData };
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
