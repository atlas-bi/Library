using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }

        [BindProperty]
        public UserFavoriteFolder Folder { get; set; }

        public class BasicFavoriteData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
        }

        public class BasicFavoriteReportData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
            public string ReportUrl { get; set; }
        }

        public List<AdList> AdLists { get; set; }
        public User PublicUser { get; set; }

        public ActionResult OnGetAsync()
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = PublicUser;
            UserId = MyUser.UserId;
            FirstName = MyUser.FirstnameCalc;

            return Page();
        }

        public async Task<ActionResult> OnGetWelcomeVideo()
        {
            var Pref = await Task.Run(
                () =>
                    UserHelpers
                        .GetPreferences(_cache, _context, User.Identity.Name)
                        .Where(x => x.ItemType == "WelcomeToAtlasVideo")
                        .FirstOrDefault()
            );
            if (Pref != null)
            {
                ViewData["Open"] = Pref.ItemValue;
            }
            else
            {
                ViewData["Open"] = 1;
            }

            //return Partial("Partials/_WelcomeVideo");
            return new PartialViewResult()
            {
                ViewName = "Partials/_WelcomeVideo",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetRecentTerms()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var NewestApprovedTerms = await (
                from dp in _context.Terms
                where dp.ApprovedYn == "Y" && dp.ValidFromDateTime > DateTime.Now.AddDays(-30)
                orderby dp.ValidFromDateTime descending
                select new BasicFavoriteData { Name = dp.Name, Id = dp.TermId, }
            )
                .Take(10)
                .ToListAsync();
            ViewData["NewestApprovedTerms"] = NewestApprovedTerms;

            int myLength = 10 - NewestApprovedTerms.Count;
            if (myLength > 0)
            {
                ViewData["NewestTerms"] = await (
                    from dp in _context.Terms
                    where dp.ApprovedYn == "N" && dp.ValidFromDateTime > DateTime.Now.AddDays(-30)
                    orderby dp.LastUpdatedDateTime descending
                    select new BasicFavoriteData { Name = dp.Name, Id = dp.TermId, }
                )
                    .Take(myLength)
                    .ToListAsync();
            }
            else
            {
                List<int> MyList = new();
                ViewData["NewestTerms"] = MyList;
            }
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            //return Partial("Partials/_RecentTerms");
            return new PartialViewResult()
            {
                ViewName = "Partials/_RecentTerms",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetRecentReports()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestReports"] = await (
                from dp in _context.ReportObjectDocs
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "report" && f.UserId == user.UserId
                    select new { f.ItemId }
                )
                    on dp.ReportObjectId equals q.ItemId
                    into tmp
                from fi in tmp.DefaultIfEmpty()
                orderby dp.LastUpdateDateTime descending
                select new BasicFavoriteReportData
                {
                    Name = dp.ReportObject.Name,
                    Id = dp.ReportObjectId,
                    Favorite = fi.ItemId == null ? "no" : "yes",
                    ReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                        HttpContext,
                        dp.ReportObject,
                        _context,
                        User.Identity.Name
                    )
                }
            ).Take(10).ToListAsync();
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            //return Partial("Partials/_RecentReports");
            return new PartialViewResult()
            {
                ViewName = "Partials/_RecentReports",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetRecentCollections()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestCollections"] = await (
                from dp in _context.DpDataProjects
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "collection" && f.UserId == user.UserId
                    select new { f.ItemId }
                )
                    on dp.DataProjectId equals q.ItemId
                    into tmp
                from fi in tmp.DefaultIfEmpty()
                orderby dp.LastUpdateDate descending
                select new BasicFavoriteData
                {
                    Name = dp.Name,
                    Id = dp.DataProjectId,
                    Favorite = fi.ItemId == null ? "no" : "yes"
                }
            ).Take(10).ToListAsync();

            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");

            return new PartialViewResult()
            {
                ViewName = "Partials/_RecentCollections",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetRecentInitiatives()
        {
            var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["UserId"] = user.UserId;

            ViewData["NewestInitiatives"] = await (
                from di in _context.DpDataInitiatives
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "initiative" && f.UserId == user.UserId
                    select new { f.ItemId }
                )
                    on di.DataInitiativeId equals q.ItemId
                    into tmp
                from fi in tmp.DefaultIfEmpty()
                orderby di.LastUpdateDate descending
                select new BasicFavoriteData
                {
                    Name = di.Name,
                    Id = di.DataInitiativeId,
                    Favorite = fi.ItemId == null ? "no" : "yes"
                }
            ).Take(10).ToListAsync();
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");

            return new PartialViewResult()
            {
                ViewName = "Partials/_RecentInitiatives",
                ViewData = ViewData
            };
        }
    }
}
