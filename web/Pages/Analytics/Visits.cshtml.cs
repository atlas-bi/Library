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
            public string Date { get; set; }
            public int Pages { get; set; }
            public int Sessions { get; set; }
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

           // double diff = end_at - start_at;

             /*
            when start - end < 2days, use 1 AM, 2 AM...
            when start - end < 8 days use  Sun 3/20, Mon 3/21...
            when start - end < 365 days use Mar 1, Mar 2 ...
            when start - end > 365 days use Jan, Feb ...

            when using all time, get first day and last day and use the above rules
            */
            switch ( end_at - start_at)
                {
                    // for < 2 days
                    // 1 AM, 2 AM etc..
                    default:
                    case < 172800:
                        AccessHistory = await ( from a in _context.Analytics
                                            where a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                                                    && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
                                            group a by new DateTime((a.AccessDateTime ?? DateTime.Now).Year, (a.AccessDateTime ?? DateTime.Now).Month, (a.AccessDateTime ?? DateTime.Now).Day,(a.AccessDateTime ?? DateTime.Now).Hour,0,0) into grp
                                            select new AccessHistoryData {
                                                Date =grp.Key.ToString("h tt"),
                                                Sessions =grp.Select(x => x.SessionId).Distinct().Count(),
                                                Pages = grp.Select(x => x.PageId).Distinct().Count()

                                            }).ToListAsync();

                        break;
                    // for < 8 days
                    //  Sun 3/20, Mon 3/21...
                    case < 691200:
                        AccessHistory = await ( from a in _context.Analytics
                                            where a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                                                    && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
                                            group a by new DateTime((a.AccessDateTime ?? DateTime.Now).Year, (a.AccessDateTime ?? DateTime.Now).Month, (a.AccessDateTime ?? DateTime.Now).Day,0,0,0) into grp
                                            select new AccessHistoryData {
                                                Date =grp.Key.ToString("ddd M/d"),
                                                Sessions =grp.Select(x => x.SessionId).Distinct().Count(),
                                                Pages = grp.Select(x => x.PageId).Distinct().Count()

                                            }).ToListAsync();
                        break;
                    // for < 365 days
                    // Mar 1, Mar 2
                    case < 31536000:
                        AccessHistory = await ( from a in _context.Analytics
                                        where a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                                                && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
                                        group a by new DateTime((a.AccessDateTime ?? DateTime.Now).Year, (a.AccessDateTime ?? DateTime.Now).Month, (a.AccessDateTime ?? DateTime.Now).Day,0,0,0) into grp
                                        select new AccessHistoryData {
                                            Date =grp.Key.ToString("MMM d"),
                                            Sessions =grp.Select(x => x.SessionId).Distinct().Count(),
                                            Pages = grp.Select(x => x.PageId).Distinct().Count()

                                        }).ToListAsync();
                        break;
                    case >= 31536000:
                        AccessHistory = await ( from a in _context.Analytics
                                        where a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
                                                && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
                                        group a by new DateTime((a.AccessDateTime ?? DateTime.Now).Year, (a.AccessDateTime ?? DateTime.Now).Month, 0,0,0,0) into grp
                                        select new AccessHistoryData {
                                            Date =grp.Key.ToString("MMM"),
                                            Sessions =grp.Select(x => x.SessionId).Distinct().Count(),
                                            Pages = grp.Select(x => x.PageId).Distinct().Count()

                                        }).ToListAsync();
                        break;

                }



            // AccessHistory = (
            //     from a in _context.Analytics
            //     where
            //         a.AccessDateTime >= DateTime.Now.AddSeconds(start_at)
            //         && a.AccessDateTime <= DateTime.Now.AddSeconds(end_at)
            //     group a by new
            //     {
            //         year = a.AccessDateTime.Value.Year,
            //         month = a.AccessDateTime.Value.Month
            //     } into tmp
            //     orderby tmp.Key.year ,tmp.Key.month
            //     select new AccessHistoryData
            //     {
            //         Month = tmp.Key.month.ToString() + "/01/" + tmp.Key.year.ToString(),
            //         Hits = tmp.Count()
            //     }
            // ).ToList();

            return Page();
        }
    }
}
