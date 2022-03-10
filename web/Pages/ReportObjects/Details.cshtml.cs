using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

/**************** notice ****************/
// this section of the website is only
// kept to allow old urls to acces
// the reports. example of the old url:
// /ReportObjects/Detials?id=73715


namespace Atlas_Web.Pages.ReportObjects
{
    public class DetailsModel : PageModel
    {
        public IActionResult OnGet(int? id)
        {
            return RedirectToPage("/Reports/Index", new { id });
        }
    }
}
