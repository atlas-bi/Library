using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Users.Settings
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        public UserSetting EnableShareNotifications { get; set; }

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            EnableShareNotifications = await _context.UserSettings.SingleOrDefaultAsync(
                x => x.Name == "share_notification" && x.UserId == MyUser.UserId
            );

            return Page();
        }

        public async Task<ActionResult> OnGetEnableShareNotification(string value)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            var shareNotification = await _context.UserSettings.SingleOrDefaultAsync(
                x => x.Name == "share_notification" && x.UserId == MyUser.UserId
            );

            if (shareNotification != null)
            {
                shareNotification.Value = value;
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.AddAsync(
                    new UserSetting
                    {
                        UserId = MyUser.UserId,
                        Name = "share_notification",
                        Value = value
                    }
                );
                await _context.SaveChangesAsync();
            }

            return Content("ok");
        }
    }
}
