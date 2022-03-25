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
using UAParser;

namespace Atlas_Web.Pages.Analytics
{
    public class TraceModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public TraceModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public List<AnalyticsTrace> Traces { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            Traces = await _context.AnalyticsTraces
                .Where(x => x.UserAgent != null)
                .Include(x => x.User)
                .OrderBy(x => x.LogDateTime)
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
            var body = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
            var package = JObject.Parse(body);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (package["lg"] != null)
            {
                foreach (var x in package["lg"])
                {
                    var level = x.Value<int>("l");
                    var message = x.Value<string>("m");
                    var logger = x.Value<string>("n");
                    var timestamp = DateTime
                        .SpecifyKind(
                            DateTimeOffset.FromUnixTimeMilliseconds(
                                Int64.Parse(x["t"].ToString()) / 1000
                            ).DateTime,
                            DateTimeKind.Utc
                        )
                        .ToLocalTime();
                    var eventId = x.Value<string>("u");

                    await _context.AddAsync(
                        new AnalyticsTrace
                        {
                            UserId = MyUser.UserId,
                            Level = x.Value<int>("l"),
                            Message = x.Value<string>("m"),
                            Logger = x.Value<string>("n"),
                            LogDateTime =
                                DateTimeOffset.FromUnixTimeMilliseconds(
                                    Int64.Parse(x["t"].ToString()) / 1000
                                ).DateTime,
                            LogId = x.Value<string>("u"),
                            UserAgent = Request.Headers["User-Agent"].ToString()
                        }
                    );

                    await _context.SaveChangesAsync();
                }
            }

            return Content("ok");
        }
    }
}
