/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Formats;

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

            var img = await _context.ReportObjectImagesDocs.Where(x => x.ImageId == id).ToListAsync();
            if (img.Count > 0)
            {
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");
                return File(img.First().ImageData, "application/octet-stream", id + ".png");
            }
            return Content("");
        }
        public async Task<ActionResult> OnGetThumb(int id, string size)
        {

            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");

            var img = await _context.ReportObjectImagesDocs.Where(x => x.ReportObjectId == id).FirstOrDefaultAsync();
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

            // if height and width are specified
            if (Regex.Match(size, @"^\d+x\d+$", RegexOptions.Multiline).Success)
            {
                String[] parts = size.Split("x");
                int width = Int32.Parse(parts[0]);
                int height = Int32.Parse(parts[1]);

                using (Image image = Image.Load<Rgba32>(image_data))
                {
                    float max_ratio = (Math.Max(width / (float)image.Width, height / (float)image.Height));

                    int wsize = (int)(image.Width * max_ratio);
                    int hsize = (int)(image.Height * max_ratio);


                    image.Mutate(x => x.Resize(wsize, hsize));

                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
                        return File(ms.ToArray(), "application/octet-stream", name + ".png");
                    }
                }
            }
            // if only a width
            else if (Regex.Match(size, @"^\d+x_$", RegexOptions.Multiline).Success)
            {
                String[] parts = size.Split("x");
                int width = Int32.Parse(parts[0]);
                using (Image image = Image.Load<Rgba32>(image_data))
                {
                    float max_ratio = (width / (float)image.Width);

                    int wsize = (int)(image.Width * max_ratio);
                    int hsize = (int)(image.Height * max_ratio);

                    image.Mutate(x => x.Resize(wsize, hsize));

                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
                        return File(ms.ToArray(), "application/octet-stream", name + ".png");
                    }
                }
            }

            return File(image_data, "application/octet-stream", id + ".png");



        }
        public async Task<ActionResult> OnGetFirst(int id)
        {

            var img = await _context.ReportObjectImagesDocs.Where(x => x.ReportObjectId == id).ToListAsync();
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
