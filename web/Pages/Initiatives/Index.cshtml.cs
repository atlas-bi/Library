using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Initiatives
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public DpDataInitiative Initiative { get; set; }

        public IEnumerable<DpDataInitiative> Initiatives { get; set; }
        public List<int?> Permissions { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public List<UserPreference> Preferences { get; set; }
        public User PublicUser { get; set; }
        public List<AdList> AdLists { get; set; }

        public string Favorite { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            ViewData["Fullname"] = MyUser.Fullname_Cust;

            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);

            // if the id null then list all
            if (id != null)
            {
                Initiative = await _context.DpDataInitiatives
                    .Include(x => x.DpDataProjects)
                    .ThenInclude(x => x.DpReportAnnotations)
                    .Include(x => x.DpContactLinks)
                    .Include(x => x.OperationOwner)
                    .Include(x => x.ExecutiveOwner)
                    .Include(x => x.FinancialImpactNavigation)
                    .Include(x => x.StrategicImportanceNavigation)
                    .Include(x => x.LastUpdateUserNavigation)
                    .SingleAsync(x => x.DataInitiativeId == id);

                Favorite = (
                    from f in _context.UserFavorites
                    where f.ItemType == "initiative" && f.UserId == MyUser.UserId && f.ItemId == id
                    select new { f.ItemId }
                ).Any()
                  ? "yes"
                  : "no";

                if (Initiative != null)
                {
                    return Page();
                }
            }

            Initiatives = await _context.DpDataInitiatives
                .Include(x => x.DpDataProjects)
                .ToListAsync();

            return Page();
        }

        public ActionResult OnGetDeleteInitiative(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                21
            );

            if (Id > 0 && checkpoint)
            {
                // remove project links, contacts and remove initiative.
                _context.DpDataProjects
                    .Where(d => d.DataInitiativeId == Id)
                    .ToList()
                    .ForEach(x => x.DataInitiativeId = null);

                _context.Remove(
                    _context.DpDataInitiatives.Where(x => x.DataInitiativeId == Id).FirstOrDefault()
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Initiatives/Index");
        }
    }
}
