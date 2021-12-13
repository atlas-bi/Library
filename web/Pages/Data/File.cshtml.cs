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
using System.Xml;
using static Atlas_Web.Pages.Reports.IndexModel;

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

            var MyFile = await _context.DpAttachments.Where(x => x.AttachmentId == id).ToListAsync();
            if (MyFile.Count > 0)
            {
                var ThisFile = MyFile.First();
                HttpContext.Response.Headers.Remove("Cache-Control");
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");
                return File(ThisFile.AttachmentData, "application/octet-stream", ThisFile.AttachmentName);
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
            var attachment = _context.ReportObjectAttachments.Where(x => x.ReportObjectAttachmentId.Equals(id)).FirstOrDefault();

            if (attachment == null)
            {
                return Content("File does not exists");
            }

            // get data
            byte[] bytes = System.IO.File.ReadAllBytes(attachment.Path);
            return File(bytes, "application/pdf", attachment.Name);

        }
    }
}
