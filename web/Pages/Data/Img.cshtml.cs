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
    }
}
