using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas_Web.Pages
{
    public class AboutModel : PageModel
    {
        public AboutModel() { }

        public ActionResult OnGetAsync()
        {
            return Page();
        }
    }
}
