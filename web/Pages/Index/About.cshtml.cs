using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public ActionResult OnGetAsync()
        {
            return Page();
        }
    }
}
