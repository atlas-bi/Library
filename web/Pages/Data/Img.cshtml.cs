using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Atlas_Web.Pages.Data
{
    public class ImgModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public ImgModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public async Task<ActionResult> OnGet(int id)
        {
            var img = await _context.ReportObjectImagesDocs
                .Where(x => x.ImageId == id)
                .ToListAsync();
            if (img.Count > 0)
            {
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");
                return File(img.First().ImageData, "application/octet-stream", id + ".png");
            }
            return Content("");
        }

        private byte[] BuildImage(byte[] image_data, string size)
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

                wsize = (int)(image.Width * max_ratio);
                hsize = (int)(image.Height * max_ratio);

                image.Mutate(x => x.Resize(wsize, hsize));
            }
            // if only a width
            else if (Regex.Match(size, @"^\d+x_$", RegexOptions.Multiline).Success)
            {
                String[] parts = size.Split("x");
                width = Int32.Parse(parts[0]);
                max_ratio = (width / (float)image.Width);

                wsize = (int)(image.Width * max_ratio);
                hsize = (int)(image.Height * max_ratio);

                image.Mutate(x => x.Resize(wsize, hsize));



            }

            //var webpEncoder = new WebPEncoder()
            //{

            //};

            var jpegEncoder = new JpegEncoder() { Quality = 75 };

            using var ms = new MemoryStream();
            image.Save(ms, jpegEncoder);
            return ms.ToArray();
        }

        public async Task<ActionResult> OnGetThumb(int id, string size, int? imgId)
        {
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");

            ReportObjectImagesDoc img;

            if (imgId.HasValue)
            {
                img = await _context.ReportObjectImagesDocs
                    .Where(x => x.ImageId == imgId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                img = await _context.ReportObjectImagesDocs
                    .Where(x => x.ReportObjectId == id)
                    .FirstOrDefaultAsync();
            }

            string name;
            byte[] image_data;
            if (img != null)
            {
                image_data = img.ImageData;
                name = id.ToString();
            }
            else
            {
                image_data = System.IO.File.ReadAllBytes("wwwroot/img/report_placeholder.png");
                name = "placeholder";
            }

            return File(BuildImage(image_data, size), "application/octet-stream", id + ".jpeg");
        }

        public async Task<ActionResult> OnGetFirst(int id)
        {
            var img = await _context.ReportObjectImagesDocs
                .Where(x => x.ReportObjectId == id)
                .ToListAsync();
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");

            if (img.Count > 0)
            {
                return File(img.First().ImageData, "application/octet-stream", id + ".png");
            }

            byte[] bytes = System.IO.File.ReadAllBytes("wwwroot/img/placeholder.png");
            return File(bytes, "application/octet-stream", "placeholder.png");
        }
    }
}
