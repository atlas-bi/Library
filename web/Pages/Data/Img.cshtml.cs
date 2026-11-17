using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Data
{
    // added no store as IE11 doesn't seem to connect cache to url params.
    [ResponseCache(NoStore = true)]
    public class ImgModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public ImgModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public ActionResult OnGetThumb(int id, string size, int? imgId, string type)
        {
            string Extension = type ?? "jpeg";
            ReportObjectImagesDoc img;

            if (imgId.HasValue)
            {
                img = _context
                    .ReportObjectImagesDocs.Where(x => x.ImageId == imgId)
                    .FirstOrDefault();
            }
            else
            {
                img = _context
                    .ReportObjectImagesDocs.Where(x => x.ReportObjectId == id)
                    .FirstOrDefault();
            }

            byte[] image_data;
            if (img != null)
            {
                image_data = img.ImageData;
            }
            else
            {
                image_data = System.IO.File.ReadAllBytes("wwwroot/img/report_placeholder.png");
            }

            return File(
                ReportThumbnail.Build(image_data, size, Extension),
                "application/octet-stream",
                $"{id}.{Extension}"
            );
        }

        public ActionResult OnGetPlaceholder(string size)
        {
            return _cache.GetOrCreate<FileContentResult>(
                "placeholder",
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    string name;
                    byte[] image_data;

                    image_data = System.IO.File.ReadAllBytes("wwwroot/img/report_placeholder.png");
                    name = "placeholder";

                    return File(
                        ReportThumbnail.Build(image_data, size, "jpeg"),
                        "application/octet-stream",
                        $"{name}.jpeg"
                    );
                }
            );
        }
    }
}
