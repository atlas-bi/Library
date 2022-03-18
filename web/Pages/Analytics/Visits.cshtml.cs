using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Pages.Analytics
{
    public class VisitsModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public VisitsModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public class SmallData
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public class MediumData
        {
            public string Name { get; set; }
            public double Time { get; set; }
            public int Count { get; set; }
            public int Sessions { get; set; }
        }

        public class ActiveUserData
        {
            public string Fullname { get; set; }
            public int UserId { get; set; }
            public string SessionTime { get; set; }
            public string PageTime { get; set; }
            public string Title { get; set; }
            public string Href { get; set; }
            public string AccessDateTime { get; set; }
            public string UpdateTime { get; set; }
            public int Pages { get; set; }
            public string SessionId { get; set; }
        }

        public class AccessHistoryData
        {
            public string Month { get; set; }
            public int Hits { get; set; }
        }

        public List<MediumData> TopUsers { get; set; }
        public List<AccessHistoryData> AccessHistory { get; set; }
        public List<AccessHistoryData> SearchHistory { get; set; }
        public List<AccessHistoryData> ReportHistory { get; set; }
        public List<AccessHistoryData> TermHistory { get; set; }
        public List<MediumData> TopPages { get; set; }

        [BindProperty]
        public Models.Analytic NewAnalytic { get; set; }

        public async Task<ActionResult> OnGetAsync(double start_at = -86400, double end_at = 0)
        {
            AccessHistory = (
                from a in _context.Analytics
                where
                    a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                    && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
                group a by new
                {
                    year = a.AccessDateTime.Value.Year,
                    month = a.AccessDateTime.Value.Month
                } into tmp
                orderby tmp.Key.year ,tmp.Key.month
                select new AccessHistoryData
                {
                    Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
                    Hits = tmp.Count()
                }
            ).ToList();

            return Page();
        }
    }
}
