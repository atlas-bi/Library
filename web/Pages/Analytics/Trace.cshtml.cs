using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atlas_Web.Pages.Analytics
{
    public class TraceModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public TraceModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public List<AnalyticsTrace> Traces { get; set; }
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
        public int Count { get; set; }
        public int CountUnresolved { get; set; }

        public async Task<ActionResult> OnGetAsync(
            int p = 0,
            double start_at = -86400,
            double end_at = 0,
            int? userId = -1,
            int? groupId = -1
        )
        {
            var page_size = 10;
            var root = _context.AnalyticsTraces
                .Where(x => x.UserAgent != null)
                .Where(
                    x =>
                        x.LogDateTime >= DateTime.Now.AddSeconds(start_at)
                        && x.LogDateTime <= DateTime.Now.AddSeconds(end_at)
                );

            if (userId > 0 && await _context.Users.AnyAsync(x => x.UserId == userId))
            {
                root = root.Where(x => x.UserId == userId);
            }

            if (groupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == groupId))
            {
                root = root.Where(x => x.User.UserGroupsMemberships.Any(y => y.GroupId == groupId));
            }

            Count = await root.CountAsync();
            CountUnresolved = await root.CountAsync(x => x.Handled != 1);
            Pages = (int)Math.Ceiling(Count / (double)page_size);
            CurrentPage = p + 1;

            Traces = await root.Include(x => x.User)
                .OrderByDescending(x => x.LogDateTime)
                .Skip(p * page_size)
                .Take(page_size)
                .ToListAsync();
            /*
            1000 trace
            2000 debug
            3000 info
            4000 warning
            5000 error
            6000 fatal
            */

            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            if (package["lg"] != null)
            {
                foreach (var x in package["lg"])
                {
                    await _context.AddAsync(
                        new AnalyticsTrace
                        {
                            UserId = User.GetUserId(),
                            Level = x.Value<int>("l"),
                            Message = x.Value<string>("m"),
                            Logger = x.Value<string>("n"),
                            LogDateTime = DateTime.Now,
                            UserAgent = Request.Headers["User-Agent"].ToString(),
                            Referer = Request.Headers["Referer"].ToString(),
                        }
                    );

                    await _context.SaveChangesAsync();
                }
            }

            return Content("ok");
        }

        public async Task<ActionResult> OnPostResolved(int Id, int Type)
        {
            AnalyticsTrace Trace = await _context.AnalyticsTraces.SingleOrDefaultAsync(
                x => x.Id == Id
            );
            if (Trace != null)
            {
                if (Type == 1)
                {
                    Trace.Handled = 1;
                }
                else
                {
                    Trace.Handled = null;
                }
                await _context.SaveChangesAsync();
            }

            return Content("ok");
        }
    }
}
