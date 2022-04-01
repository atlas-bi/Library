using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Diagnostics;

namespace Atlas_Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public ErrorModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ExceptionMessage { get; set; }

        public ActionResult OnGetAsync(int? id)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            if (id == 400)
            {
                return RedirectToPage(
                    "/Index/Index",
                    new { error = "Sorry that page does not exist." }
                );
            }
            // RequestId = HttpContext.TraceIdentifier;

            // var exceptionHandlerPathFeature =
            //     HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // exceptionHandlerPathFeature?.Path
            // exceptionHandlerPathFeature?.Endpoint?.DisplayName
            // exceptionHandlerPathFeature?.Error?.Message
            // exceptionHandlerPathFeature?.Error?.StackTrace
            // if (exceptionHandlerPathFeature?.Path == "/")
            // {
            //     ExceptionMessage ??= string.Empty;
            // }

            return RedirectToPage("/Index/Index", new { error = "Sorry there was an error." });
        }
    }
}
