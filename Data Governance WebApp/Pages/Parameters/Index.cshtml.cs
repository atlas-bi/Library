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
using System.Collections.Generic;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.Parameters
{
    public class IndexModel : PageModel
    {
        private readonly Data_GovernanceContext _context;
        private IMemoryCache _cache;
        public IndexModel(Data_GovernanceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public List<UserFavorites> Favorites { get; set; }

        public List<int?> Permissions { get; set; }
        public IEnumerable<ValueListData> OrganizationalValueList { get; set; }
        public IEnumerable<ValueListData> EstimatedRunFrequencyList { get; set; }
        public IEnumerable<ValueListData> MaintenanceScheduleList { get; set; }
        public IEnumerable<ValueListData> FragilityTagList { get; set; }
        public IEnumerable<ValueListData> FragilityList { get; set; }
        public IEnumerable<ValueListData> MaintenanceLogStatusList { get; set; }
        public IEnumerable<ValueListData> FinancialImpactList { get; set; }
        public IEnumerable<ValueListData> StrategicImportanceList { get; set; }
		public IEnumerable<ValueListData> MilestoneTemplateList { get; set;}
		public IEnumerable<ContactListData> DpContactList { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        [BindProperty] public OrganizationalValue OrganizationalValue { get; set; }
        [BindProperty] public EstimatedRunFrequency EstimatedRunFrequency { get; set; }
        [BindProperty] public MaintenanceSchedule MaintenanceSchedule { get; set; }
        [BindProperty] public FragilityTag FragilityTag { get; set; }
        [BindProperty] public Fragility Fragility { get; set; }
        [BindProperty] public MaintenanceLogStatus MaintenanceLogStatus { get; set; }
        [BindProperty] public FinancialImpact FinancialImpact { get; set; }
        [BindProperty] public StrategicImportance StrategicImportance { get; set; }
		[BindProperty] public DpMilestoneTemplates MilestoneTemplates { get; set;}
        [BindProperty] public DpContact DpContact { get; set; }
        [BindProperty] public GlobalSiteSettings GlobalSiteSettings { get; set; }

        public class GlobalSettingsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
        }
        public class ValueListData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Used { get; set; }
        }

        public class ContactListData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Company { get; set; }
            public int Used { get; set; }
        }
        public User PublicUser { get; set; }
        public async Task<IActionResult> OnGetGlobalSettings()
        {
            ViewData["GlobalSettings"] = await (from o in _context.GlobalSiteSettings
                                                         select new GlobalSettingsData
                                                         {
                                                             Id = o.Id,
                                                             Name = o.Name,
                                                             Description = o.Description,
                                                             Value = o.Value
                                                         }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            return Partial("Partials/_GlobalSettings");
        }

        public ActionResult OnPostDeleteGlobalSetting()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 45);
            if (ModelState.IsValid && GlobalSiteSettings.Id > 0 && checkpoint)
            {
                _context.Remove(GlobalSiteSettings);
                _context.SaveChanges();
            }

            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostAddGlobalSetting()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 45);
            if (ModelState.IsValid && checkpoint)
            {
                _context.Add(GlobalSiteSettings);
                _context.SaveChanges();
            }

            return RedirectToPage("/Parameters/Index");
        }

        public async Task<IActionResult> OnGetOrganizationalValueList()
        {
            ViewData["OrganizationalValueList"] = await (from o in _context.OrganizationalValue
                                             select new ValueListData
                                             {
                                                 Id = o.OrganizationalValueId,
                                                 Name = o.OrganizationalValueName,
                                                 Used = o.ReportObjectDoc.Count()
                                             }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_OrganizationalValueList");
        }

        public async Task<IActionResult> OnGetEstimatedRunFrequencyList()
        {
        	ViewData["EstimatedRunFrequencyList"] = await (from o in _context.EstimatedRunFrequency
                                               select new ValueListData
                                               {
                                                   Id = o.EstimatedRunFrequencyId,
                                                   Name = o.EstimatedRunFrequencyName,
                                                   Used = o.ReportObjectDoc.Count()
                                               }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_EstimatedRunFrequencyList");
        }

        public async Task<IActionResult> OnGetMaintenanceScheduleList()
        {
            ViewData["MaintenanceScheduleList"] = await (from o in _context.MaintenanceSchedule
                                             select new ValueListData
                                             {
                                                 Id = o.MaintenanceScheduleId,
                                                 Name = o.MaintenanceScheduleName,
                                                 Used = o.ReportObjectDoc.Count()
                                             }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_MaintenanceScheduleList");
        }

        public async Task<IActionResult> OnGetFragilityList()
        {
            ViewData["FragilityList"] = await (from o in _context.Fragility
                                   select new ValueListData
                                   {
                                       Id = o.FragilityId,
                                       Name = o.FragilityName,
                                       Used = o.ReportObjectDoc.Count()
                                   }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_FragilityList");
        }

        public async Task<IActionResult> OnGetFragilityTagList()
        {
            ViewData["FragilityTagList"] = await (from o in _context.FragilityTag
                                      select new ValueListData
                                      {
                                          Id = o.FragilityTagId,
                                          Name = o.FragilityTagName,
                                          Used = o.ReportObjectDocFragilityTags.Count()
                                      }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_FragilityTagList");
        }

        public async Task<IActionResult> OnGetMaintenanceLogStatusList()
        {
            ViewData["MaintenanceLogStatusList"] = await (from o in _context.MaintenanceLogStatus
                                              select new ValueListData
                                              {
                                                  Id = o.MaintenanceLogStatusId,
                                                  Name = o.MaintenanceLogStatusName,
                                                  Used = o.MaintenanceLog.Count()
                                              }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_MaintenanceLogStatusList");
        }

        public async Task<IActionResult> OnGetFinancialImpactList()
        {
            ViewData["FinancialImpactList"] = await (from o in _context.FinancialImpact
                                         select new ValueListData
                                         {
                                             Id = o.FinancialImpactId,
                                             Name = o.Name,
                                             Used = o.DpDataInitiative.Count() + o.DpDataProject.Count()
                                         }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_FinancialImpactList");
        }

        public async Task<IActionResult> OnGetStrategicImportanceList()
        {
            ViewData["StrategicImportanceList"] = await (from o in _context.StrategicImportance
                                             select new ValueListData
                                             {
                                                 Id = o.StrategicImportanceId,
                                                 Name = o.Name,
                                                 Used = o.DpDataInitiative.Count() + o.DpDataProject.Count()
                                             }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_StrategicImportanceList");
        }

        public async Task<IActionResult> OnGetMilestoneTemplateList()
        {
            ViewData["MilestoneTemplateList"] = await (from o in _context.DpMilestoneTemplates
                                           select new ValueListData
                                           {
                                               Id = o.MilestoneTemplateId,
                                               Name = o.Name,
                                               Used = o.DpMilestoneTasks.Count()
                                           }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_MilestoneTemplateList");
        }

        public async Task<IActionResult> OnGetDpContactList()
        {
            ViewData["DpContactList"] = await (from o in _context.DpContact
                                   select new ContactListData
                                   {
                                       Id = o.ContactId,
                                       Name = o.Name,
                                       Phone = o.Phone,
                                       Email = o.Email,
                                       Company = o.Company,
                                       Used = o.DpContactLinks.Count()
                                   }).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Partials/_DpContactList");
        }

        public ActionResult OnGet()
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            return Page();
        }

        public ActionResult OnPostCreateOrganizationalValue()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);

            if (ModelState.IsValid && OrganizationalValue.OrganizationalValueName != null && checkpoint)
            {
                _context.Add(OrganizationalValue);
                _context.SaveChanges();
            }
            _cache.Remove("org-value");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteOrganizationalValue()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && OrganizationalValue.OrganizationalValueId > 0 && checkpoint)
            {
                _context.ReportObjectDoc.Where(x => x.OrganizationalValueId.Equals(OrganizationalValue.OrganizationalValueId)).ToList().ForEach(q => q.OrganizationalValueId = null);
                _context.Remove(OrganizationalValue);
                _context.SaveChanges();
            }
            _cache.Remove("org-value");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateEstimatedRunFrequency()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && EstimatedRunFrequency.EstimatedRunFrequencyName != null && checkpoint)
            {
                _context.Add(EstimatedRunFrequency);
                _context.SaveChanges();
            }
            _cache.Remove("run-freq");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteEstimatedRunFrequency()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && EstimatedRunFrequency.EstimatedRunFrequencyId > 0 && checkpoint)
            {
                _context.ReportObjectDoc.Where(x => x.EstimatedRunFrequencyId.Equals(EstimatedRunFrequency.EstimatedRunFrequencyId)).ToList().ForEach(q => q.EstimatedRunFrequencyId = null);
                _context.Remove(EstimatedRunFrequency);
                _context.SaveChanges();
            }
            _cache.Remove("run-freq");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateFragility()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && Fragility.FragilityName != null && checkpoint)
            {
                _context.Add(Fragility);
                _context.SaveChanges();
            }
            _cache.Remove("fragility");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteFragility()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && Fragility.FragilityId > 0 && checkpoint)
            {
                _context.ReportObjectDoc.Where(x => x.FragilityId.Equals(Fragility.FragilityId)).ToList().ForEach(q => q.FragilityId = null);
                _context.Remove(Fragility);
                _context.SaveChanges();
            }
            _cache.Remove("fragility");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateMaintenanceSchedule()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && MaintenanceSchedule.MaintenanceScheduleName != null && checkpoint)
            {
                _context.Add(MaintenanceSchedule);
                _context.SaveChanges();
            }
            _cache.Remove("maint-sched");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteMaintenanceSchedule()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && MaintenanceSchedule.MaintenanceScheduleId > 0 && checkpoint)
            {
                _context.ReportObjectDoc.Where(x => x.MaintenanceScheduleId.Equals(MaintenanceSchedule.MaintenanceScheduleId)).ToList().ForEach(q => q.MaintenanceScheduleId = null);
                _context.Remove(MaintenanceSchedule);
                _context.SaveChanges();
            }
            _cache.Remove("maint-sched");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateFragilityTag()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && FragilityTag.FragilityTagName != null && checkpoint)
            {
                _context.Add(FragilityTag);
                _context.SaveChanges();
            }
            _cache.Remove("ro-fragility");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteFragilityTag()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && FragilityTag.FragilityTagId > 0 && checkpoint)
            {
                _context.RemoveRange(_context.ReportObjectDocFragilityTags.Where(x => x.FragilityTagId.Equals(FragilityTag.FragilityTagId)));
                _context.Remove(FragilityTag);
                _context.SaveChanges();
            }
            _cache.Remove("ro-fragility");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateMaintenanceLogStatus()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && MaintenanceLogStatus.MaintenanceLogStatusName != null && checkpoint)
            {
                _context.Add(MaintenanceLogStatus);
                _context.SaveChanges();
            }
            _cache.Remove("maint-log-status");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteMaintenanceLogStatus()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && MaintenanceLogStatus.MaintenanceLogStatusId > 0 && checkpoint)
            {
                _context.RemoveRange(_context.ReportObjectDocMaintenanceLogs.Where(x => x.MaintenanceLog.MaintenanceLogStatusId.Equals(MaintenanceLogStatus.MaintenanceLogStatusId)));
                _context.RemoveRange(_context.MaintenanceLog.Where(x => x.MaintenanceLogStatusId.Equals(MaintenanceLogStatus.MaintenanceLogStatusId)));
                _context.Remove(MaintenanceLogStatus);
                _context.SaveChanges();
            }
            _cache.Remove("maint-log-status");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateMilestoneTemplates()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && MilestoneTemplates.MilestoneTypeId != null && MilestoneTemplates.Interval > 0 && checkpoint)
            {
                MilestoneTemplates.Name = Helpers.HtmlHelpers.MilestoneFrequencyName(MilestoneTemplates, _context.DpMilestoneFrequency.Where(x => x.MilestoneTypeId == MilestoneTemplates.MilestoneTypeId).FirstOrDefault().Name);
                MilestoneTemplates.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                MilestoneTemplates.LastUpdateDate = DateTime.Now;
                _context.Add(MilestoneTemplates);
                _context.SaveChanges();
            }
            _cache.Remove("mile-temp");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteMilestoneTemplate()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && MilestoneTemplates.MilestoneTemplateId > 0 && checkpoint)
            {
                _context.RemoveRange(_context.DpMilestoneTasks.Where(x => x.MilestoneTemplateId.Equals(MilestoneTemplates.MilestoneTemplateId)));
                _context.Remove(MilestoneTemplates);
                _context.SaveChanges();
            }
            _cache.Remove("mile-temp");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateFinancialImpact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && FinancialImpact.Name != null && checkpoint)
            {
                _context.Add(FinancialImpact);
                _context.SaveChanges();
            }
            _cache.Remove("financial-impact");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteFinancialImpact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && FinancialImpact.FinancialImpactId > 0 && checkpoint)
            {
                _context.DpDataProject.Where(x => x.FinancialImpact.Equals(FinancialImpact.FinancialImpactId)).ToList().ForEach(q => q.FinancialImpact = null);
                _context.DpDataInitiative.Where(x => x.FinancialImpact.Equals(FinancialImpact.FinancialImpactId)).ToList().ForEach(q => q.FinancialImpact = null);
                _context.Remove(FinancialImpact);
                _context.SaveChanges();
            }
            _cache.Remove("financial-impact");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateStrategicImportance()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && StrategicImportance.Name != null && checkpoint)
            {
                _context.Add(StrategicImportance);
                _context.SaveChanges();
            }
            _cache.Remove("strategic-importance");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteStrategicImportance()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && StrategicImportance.StrategicImportanceId > 0 && checkpoint)
            {
                _context.DpDataProject.Where(x => x.StrategicImportance.Equals(StrategicImportance.StrategicImportanceId)).ToList().ForEach(q => q.StrategicImportance = null);
                _context.DpDataInitiative.Where(x => x.StrategicImportance.Equals(StrategicImportance.StrategicImportanceId)).ToList().ForEach(q => q.StrategicImportance = null);
                _context.Remove(StrategicImportance);
                _context.SaveChanges();
            }
            _cache.Remove("strategic-importance");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostCreateDpContact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 33);
            if (ModelState.IsValid && DpContact.Name != null && checkpoint)
            {
                _context.DpContact.Add(DpContact);
                _context.SaveChanges();
            }
            _cache.Remove("ext-cont");
            return RedirectToPage("/Parameters/Index");
        }

        public ActionResult OnPostDeleteDpContact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 34);
            if (ModelState.IsValid && DpContact.ContactId > 0 && checkpoint)
            {
                _context.RemoveRange(_context.DpContactLinks.Where(d => d.ContactId.Equals(DpContact.ContactId)));
                _context.Remove(DpContact);
                _context.SaveChanges();
            }
            _cache.Remove("ext-cont");
            return RedirectToPage("/Parameters/Index");
        }
    }
}