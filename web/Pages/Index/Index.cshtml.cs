using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Atlas_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        private readonly IAuthorizationService _authorizationService;

        public IndexModel(
            Atlas_WebContext context,
            IMemoryCache cache,
            IConfiguration config,
            IAuthorizationService authorizationService
        )
        {
            _context = context;
            _config = config;
            _cache = cache;
            _authorizationService = authorizationService;
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }

        public class BasicFavoriteData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
        }

        public List<AdList> AdLists { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
            };
            ViewData["AdLists"] = AdLists;

            ViewData["DefaultReportTypes"] = await _context.ReportObjectTypes
                .Where(v => v.Visible == "Y")
                .Select(x => x.ReportObjectTypeId)
                .ToListAsync();

            return Page();
        }

        public async Task<ActionResult> OnGetRecentTerms()
        {
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
            return new PartialViewResult
            {
                ViewName = "Partials/_RecentTerms",
                ViewData = ViewData
            };
        }
    }
}
