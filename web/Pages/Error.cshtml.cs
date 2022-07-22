using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Atlas_Web.Helpers;
using Atlas_Web.Authorization;
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
            RequestId = HttpContext.TraceIdentifier;

            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature == null)
            {
                var statusCodeReExecuteFeature =
                    HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

                string referer = Request.Headers["Referer"].ToString();
                if (statusCodeReExecuteFeature is not null)
                {
                    referer = string.Join(
                        statusCodeReExecuteFeature.OriginalPathBase,
                        statusCodeReExecuteFeature.OriginalPath,
                        statusCodeReExecuteFeature.OriginalQueryString
                    );
                }

                _context.Add(
                    new AnalyticsError
                    {
                        UserId = User.GetUserId(),
                        StatusCode = id,
                        Referer = referer,
                        Message = "Invalid request.",
                        LogDateTime = DateTime.Now,
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                    }
                );
                _context.SaveChanges();

                ExceptionMessage = "Sorry that page does not exist.";

                return Page();
            }

            var message =
                exceptionHandlerPathFeature?.Error?.Message
                + " Url: "
                + exceptionHandlerPathFeature?.Endpoint?.DisplayName;

            _context.Add(
                new AnalyticsError
                {
                    UserId = User.GetUserId(),
                    StatusCode = Response.StatusCode,
                    Referer = exceptionHandlerPathFeature?.Path,
                    Message = message,
                    Trace = exceptionHandlerPathFeature?.Error?.StackTrace,
                    LogDateTime = DateTime.Now,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                }
            );
            _context.SaveChanges();

            ExceptionMessage = "Sorry there was an error accessing that page.";

            return Page();
        }
    }
}
