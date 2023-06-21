using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;

namespace Atlas_Web.Pages.Data
{
    [ResponseCache(Duration = 20 * 60)]
    public class ImgModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public ImgModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private static byte[] BuildImage(byte[] image_data, string size, string extension)
        {
            int wsize;
            int hsize;
            int width;
            int height;
            float max_ratio;

            using Image image = Image.Load<Rgba32>(image_data);
            // if height and width are specified
            if (Regex.Match(size, @"^\d+x\d+$", RegexOptions.Multiline).Success)
            {
                String[] parts = size.Split("x");
                width = Int32.Parse(parts[0]);
                height = Int32.Parse(parts[1]);

                max_ratio = (Math.Max(width / (float)image.Width, height / (float)image.Height));

                // resize only if we can shrink it
                if (max_ratio < 1)
                {
                    wsize = (int)(image.Width * max_ratio);
                    hsize = (int)(image.Height * max_ratio);

                    image.Mutate(x => x.Resize(wsize, hsize));
                }
            }
            // if only a width
            else if (Regex.Match(size, @"^\d+x_$", RegexOptions.Multiline).Success)
            {
                String[] parts = size.Split("x");
                width = Int32.Parse(parts[0]);
                max_ratio = (width / (float)image.Width);

                // resize only if we can shrink it
                if (max_ratio < 1)
                {
                    wsize = (int)(image.Width * max_ratio);
                    hsize = (int)(image.Height * max_ratio);

                    image.Mutate(x => x.Resize(wsize, hsize));
                }
            }

            using var ms = new MemoryStream();
            if (extension == "webp")
            {
                var webpEncoder = new WebpEncoder { Quality = 75 };
                image.Save(ms, webpEncoder);
            }
            else
            {
                var jpegEncoder = new JpegEncoder { Quality = 75 };
                image.Save(ms, jpegEncoder);
            }

            return ms.ToArray();
        }

        public ActionResult OnGetThumb(int id, string size, int? imgId, string type)
        {
            string Extension = type ?? "jpeg";
            ReportObjectImagesDoc img;

            if (imgId.HasValue)
            {
                img = _context.ReportObjectImagesDocs
                    .Where(x => x.ImageId == imgId)
                    .FirstOrDefault();
            }
            else
            {
                img = _context.ReportObjectImagesDocs
                    .Where(x => x.ReportObjectId == id)
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
                BuildImage(image_data, size, Extension),
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
                        BuildImage(image_data, size, "jpeg"),
                        "application/octet-stream",
                        $"{name}.jpeg"
                    );
                }
            );
        }
    }
}
