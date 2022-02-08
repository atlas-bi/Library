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

using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Dotnet.Pages
{
    public class AboutModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public AboutModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }


        public int UserId { get; set; }
        public string FirstName { get; set; }

        [BindProperty]
        public UserFavoriteFolder Folder { get; set; }


        public class BasicFavoriteData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
        }

        public class BasicFavoriteReportData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
            public string ReportUrl { get; set; }
        }

        public List<AdList> AdLists { get; set; }


        public ActionResult OnGetAsync()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name); ;
            UserId = MyUser.UserId;
            FirstName = MyUser.Firstname_Cust;


            return Page();
        }
    }
}
