using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Atlas_Web.Pages.Data
{
    public class FileModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public FileModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public object ReportServer { get; private set; }

        public class ReportData
        {
            public string server { get; set; }
        }

        public async Task<ActionResult> OnGet(int id)
        {
            var MyFile = await _context.DpAttachments
                .Where(x => x.AttachmentId == id)
                .ToListAsync();
            if (MyFile.Count > 0)
            {
                var ThisFile = MyFile.First();
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");
                return File(
                    ThisFile.AttachmentData,
                    "application/octet-stream",
                    ThisFile.AttachmentName
                );
            }
            return Content("");
        }

        public async Task<ActionResult> OnGetCube(int id)
        {
            string text = System.IO.File.ReadAllText("wwwroot/Cube.xml");
            var cube = await _context.ReportObjects.Where(x => x.ReportObjectId == id).FirstAsync();
            text = text.Replace("server", cube.SourceServer);
            text = text.Replace("Catalog_Name", cube.Name);
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            HttpContext.Response.Headers.Remove("Cache-Control");
            return File(bytes, "text/plain", cube.Name + ".odc");
        }

        public async Task<ActionResult> OnGetCrystalRun(int id)
        {
            // check permissions first!
            var attachment = _context.ReportObjectAttachments
                .Where(x => x.ReportObjectAttachmentId.Equals(id))
                .FirstOrDefault();

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

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = attachment.Name,
                Inline = true
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return File(System.IO.File.ReadAllBytes(attachment.Path), "application/pdf");
        }
    }
}
