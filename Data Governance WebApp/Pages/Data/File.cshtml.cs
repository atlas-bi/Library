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
using Data_Governance_WebApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Caching.Memory;


namespace Data_Governance_WebApp.Pages.Data
{
    public class FileModel : PageModel
    {
        private readonly Data_GovernanceContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public FileModel(Data_GovernanceContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public async Task<ActionResult> OnGet(int id)
        {

            var MyFile = await _context.DpAttachments.Where(x => x.AttachmentId == id).ToListAsync();
            if (MyFile.Count > 0)
            {
                var ThisFile = MyFile.First();
                HttpContext.Response.Headers.Add("Cache-Control", "max-age=315360000");
                return File(ThisFile.AttachmentData, "application/octet-stream", ThisFile.AttachmentName);
            }
            return Content("");
        }
    }
}