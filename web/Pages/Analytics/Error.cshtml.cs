using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Pages.Analytics
{
    public class ErrorModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public ErrorModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public List<AnalyticsError> Errors { get; set; }
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
            var root = _context.AnalyticsErrors
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

            Errors = await root.Include(x => x.User)
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

        public async Task<ActionResult> OnPostResolved(int Id, int Type)
        {
            AnalyticsError Error = await _context.AnalyticsErrors.SingleOrDefaultAsync(
                x => x.Id == Id
            );
            if (Error != null)
            {
                if (Type == 1)
                {
                    Error.Handled = 1;
                }
                else
                {
                    Error.Handled = null;
                }
                await _context.SaveChangesAsync();
            }

            return Content("ok");
        }
    }
}
