using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Text;

namespace Atlas_Web.Pages.Data
{
    [ResponseCache(Duration = 20 * 60)]
    public class FileModel : PageModel
    {
        private readonly Atlas_WebContext _context;

        public FileModel(Atlas_WebContext context)
        {
            _context = context;
        }

        public object ReportServer { get; private set; }

        public async Task<ActionResult> OnGetCube(int id)
        {
            string text = System.IO.File.ReadAllText("wwwroot/Cube.xml");
            var cube = await _context.ReportObjects.Where(x => x.ReportObjectId == id).FirstAsync();
            text = text.Replace("server", cube.SourceServer);
            text = text.Replace("Catalog_Name", cube.Name);
            byte[] bytes = Encoding.ASCII.GetBytes(text);

            return File(bytes, "text/plain", cube.Name + ".odc");
        }

        public async Task<ActionResult> OnGetCrystalRun(int id)
        {
            // check permissions first!
            var attachment = await _context.ReportObjectAttachments
                .Where(x => x.ReportObjectAttachmentId.Equals(id))
                .FirstOrDefaultAsync();

            if (attachment == null)
            {
                return Content("File does not exists");
            }

            if (
                !Helpers.UserHelpers.CheckHrxPermissions(
                    _context,
                    attachment.ReportObjectId,
                    User.Identity.Name
                )
            )
            {
                return Content("Your are not authorized to view this page.");
            }

            // headers to attempt to open pdf vs download

            System.Net.Mime.ContentDisposition cd =
                new() { FileName = attachment.Name, Inline = true };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return File(System.IO.File.ReadAllBytes(attachment.Path), "application/pdf");
        }
    }
}
