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

namespace Atlas_Web.Pages
{
    public class AboutModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public AboutModel(Atlas_WebContext context, IMemoryCache cache, IConfiguration config)
        {
            _context = context;
            _cache = cache;
            _config = config;
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }

        public ActionResult OnGetAsync()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            UserId = MyUser.UserId;
            FirstName = MyUser.FirstnameCalc;

            return Page();
        }
    }
}
