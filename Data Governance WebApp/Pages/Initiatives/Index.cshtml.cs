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
using Microsoft.AspNetCore.Mvc.Rendering;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.Initiatives
{
    public class IndexModel : PageModel
    {
        private readonly Data_GovernanceContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public IndexModel(Data_GovernanceContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public class DataInitiativeData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string OpOwner { get; set; }
            public int? OpOwnerId { get; set; }
            public string ExOwner { get; set; }
            public int? ExOwnerId { get; set; }
            public string FinancialImp { get; set; }
            public int? FinancialImpId { get; set; }
            public string StrategicImp { get; set; }
            public int? StrategicImpId { get; set; }
            public string UpdatedBy { get; set; }
            public string UpdateDate { get; set; }
            public string Description { get; set; }
            public IEnumerable<RelatedProjectsData> RelatedProjects { get; set; }
            public IEnumerable<RelatedContactsData> RelatedContacts { get; set; }
            public string Favorite { get; set; }
        }

        public class RelatedProjectsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<RelatedReportsData> RelatedReports { get; set; }
        }

        public class RelatedReportsData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Annotation { get; set; }
        }

        public class RelatedContactsData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Company { get; set; }
        }
        public class AllInitiativesData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Favorite { get; set; }
        }

        [BindProperty] public int[] LinkedDataProjects { get; set; }
        [BindProperty] public int?[] LinkedContacts { get; set; }
        [BindProperty] public DpDataInitiative DpDataInitiative { get; set; }

        public DataInitiativeData DataInitiative { get; set; }
        public IEnumerable<AllInitiativesData> AllInitiatives { get; set; }
        public List<int?> Permissions { get; set; }
        public List<UserFavorites> Favorites { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        public User PublicUser { get; set; }
        public List<AdList> AdLists { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            ViewData["Fullname"] = MyUser.Fullname_Cust;

             AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentProjects", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;

            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);

            // if the id null then list all
            if (id != null)
            {
                DataInitiative = await (from d in _context.DpDataInitiative
                                        where d.DataInitiativeId == id
                                        join q in (from f in _context.UserFavorites
                                                   where f.ItemType.ToLower() == "initiative"
                                                      && f.UserId == MyUser.UserId
                                                   select new { f.ItemId })
                                              on d.DataInitiativeId equals q.ItemId into tmp
                                        from fi in tmp.DefaultIfEmpty()
                                        select new DataInitiativeData
                                        {
                                            Id = d.DataInitiativeId,
                                            Name = d.Name,
                                            OpOwner = d.OperationOwner.Fullname_Cust,
                                            OpOwnerId = d.OperationOwnerId,
                                            ExOwner = d.ExecutiveOwner.Fullname_Cust,
                                            ExOwnerId = d.ExecutiveOwnerId,
                                            FinancialImp = d.FinancialImpactNavigation.Name,
                                            FinancialImpId = d.FinancialImpact,
                                            StrategicImp = d.StrategicImportanceNavigation.Name,
                                            StrategicImpId = d.StrategicImportance,
                                            UpdateDate = d.LastUpdatedDateDisplayString,
                                            UpdatedBy = d.LastUpdateUserNavigation.Fullname_Cust,
                                            Description = d.Description,
                                            RelatedProjects = from p in _context.DpDataProject
                                                              where p.DataInitiativeId == d.DataInitiativeId
                                                              select new RelatedProjectsData
                                                              {
                                                                  Id = p.DataProjectId,
                                                                  Name = p.Name,
                                                                  RelatedReports = from r in _context.DpReportAnnotation
                                                                                   where r.DataProjectId == p.DataProjectId
                                                                                   select new RelatedReportsData
                                                                                   {
                                                                                       Id = r.ReportId,
                                                                                       Name = r.Report.Name,
                                                                                       Annotation = r.Annotation
                                                                                   }
                                                              },
                                            RelatedContacts = from c in _context.DpContactLinks
                                                              where c.InitiativeId == d.DataInitiativeId
                                                              select new RelatedContactsData
                                                              {
                                                                  Id = c.ContactId,
                                                                  Name = c.Contact.Name,
                                                                  Email = c.Contact.Email,
                                                                  Phone = c.Contact.Phone,
                                                                  Company = c.Contact.Company
                                                              },
                                            Favorite = fi.ItemId == null ? "no" : "yes"
                                        }).FirstOrDefaultAsync();
                if (DataInitiative != null)
                {
                    return Page();
                }
            }
            
            AllInitiatives = await (from i in _context.DpDataInitiative
                                    join q in (from f in _context.UserFavorites
                                                where f.ItemType.ToLower() == "initiative"
                                                    && f.UserId == MyUser.UserId
                                                select new { f.ItemId })
                                    on i.DataInitiativeId equals q.ItemId into tmp
                                    orderby i.LastUpdateDate descending
                                    from fi in tmp.DefaultIfEmpty()
                                    select new AllInitiativesData
                                    {
                                        Id = i.DataInitiativeId,
                                        Name = i.Name,
                                        Description = i.Description,
                                        Favorite = fi.ItemId == null ? "no" : "yes"
                                    }).ToListAsync();

           
            return Page();
        }

        public ActionResult OnPostNewDataInitiative()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 20);
            // if invalid inputs redirect to list all
            if (!ModelState.IsValid || DpDataInitiative.Name is null || !checkpoint)
            {
                return RedirectToPage("/Initiatives/Index");
            }

            // all data comes from the form, but we update 2 atribs
            // get last update values and save
            DpDataInitiative.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            DpDataInitiative.LastUpdateDate = DateTime.Now;
            _context.Add(DpDataInitiative);

            // add any links to the projects
            _context.DpDataProject.Where(t => LinkedDataProjects.Contains(t.DataProjectId)).ToList().ForEach(x => x.DataInitiativeId = DpDataInitiative.DataInitiativeId);

            // add contacts
            _context.AddRange(LinkedContacts.Select(id => new DpContactLinks {ContactId = id, InitiativeId = DpDataInitiative.DataInitiativeId}));
            _context.SaveChanges();
           
            return RedirectToPage("/Initiatives/Index", new { id = DpDataInitiative.DataInitiativeId });
        }

        public ActionResult OnGetDeleteInitiative(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 21);

            if (Id > 0 && checkpoint)
            {
                // remove project links, contacts and remove initiative.
                _context.DpDataProject.Where(d => d.DataInitiativeId == Id).ToList().ForEach(x => x.DataInitiativeId = null);
                _context.RemoveRange(_context.DpContactLinks.Where(i => i.InitiativeId ==Id));
                _context.Remove(_context.DpDataInitiative.Where(x => x.DataInitiativeId == Id).FirstOrDefault());
                _context.SaveChanges();
            }
            
            return RedirectToPage("/Initiatives/Index");
        }

        public ActionResult OnPostEditInitiative()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 22);

            if (!ModelState.IsValid || !checkpoint)
            {
                return RedirectToPage("/Initiatives/Index", new { id = DpDataInitiative.DataInitiativeId });
            }
            
            // we get a copy of the initiative and then will only update several fields.
            DpDataInitiative  NewDpDataInitiative = _context.DpDataInitiative.Where(m => m.DataInitiativeId == DpDataInitiative.DataInitiativeId).FirstOrDefault();

            // update last update values & values that were posted      
            NewDpDataInitiative.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewDpDataInitiative.LastUpdateDate = DateTime.Now;
            NewDpDataInitiative.Name = DpDataInitiative.Name;
            NewDpDataInitiative.Description = DpDataInitiative.Description;
            NewDpDataInitiative.OperationOwnerId = DpDataInitiative.OperationOwnerId;
            NewDpDataInitiative.ExecutiveOwnerId = DpDataInitiative.ExecutiveOwnerId;
            NewDpDataInitiative.FinancialImpact = DpDataInitiative.FinancialImpact;
            NewDpDataInitiative.StrategicImportance = DpDataInitiative.StrategicImportance;
            
            _context.Attach(NewDpDataInitiative).State = EntityState.Modified;
            
            // updated any linked data projects that were added and remove any that were delinked.
            _context.DpDataProject.Where(d => LinkedDataProjects.Contains(d.DataProjectId)).ToList().ForEach(x => x.DataInitiativeId = DpDataInitiative.DataInitiativeId);
            _context.DpDataProject.Where(d => d.DataInitiativeId == DpDataInitiative.DataInitiativeId).Except(_context.DpDataProject.Where(d => LinkedDataProjects.Contains(d.DataProjectId))).ToList().ForEach(x => x.DataInitiativeId = null);
            // first delete contacts
            _context.RemoveRange(_context.DpContactLinks.Where(i => i.InitiativeId == DpDataInitiative.DataInitiativeId));
            
            // add contacts
            _context.AddRange(LinkedContacts.Select(id => new DpContactLinks { ContactId = id, InitiativeId = DpDataInitiative.DataInitiativeId }));
            _context.SaveChanges();

            // redirect back to same initiative
            return RedirectToPage("/Initiatives/Index", new { id = DpDataInitiative.DataInitiativeId });
        }
    }
}