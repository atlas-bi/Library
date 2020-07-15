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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data_Governance_WebApp.Models;
/**************** notice ****************/
// this section of the website is only 
// kept to allow old urls to acces
// the reports. example of the old url:
// /ReportObjects/Detials?id=73715


namespace Data_Governance_WebApp.Pages.ReportObjects
{
    public class DetailsModel : PageModel
    {
        private readonly Data_GovernanceContext _context;

        public DetailsModel(Data_GovernanceContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? id)
        {
            return RedirectToPage("/Reports/Index", new { id });
        }
    }
}