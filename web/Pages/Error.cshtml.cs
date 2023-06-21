using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace Atlas_Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public ErrorModel(Atlas_WebContext context)
        {
            _context = context;
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

                string referrer = Request.Headers["Referrer"].ToString();
                if (statusCodeReExecuteFeature is not null)
                {
                    referrer = string.Join(
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
                        Referrer = referrer,
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
                    Referrer = exceptionHandlerPathFeature?.Path,
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
