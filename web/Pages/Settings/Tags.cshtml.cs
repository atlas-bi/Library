using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Settings
{
    [ResponseCache(NoStore = true)]
    public class TagsModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public TagsModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<ValueListData> OrganizationalValueList { get; set; }
        public IEnumerable<ValueListData> EstimatedRunFrequencyList { get; set; }
        public IEnumerable<ValueListData> MaintenanceScheduleList { get; set; }
        public IEnumerable<ValueListData> FragilityTagList { get; set; }
        public IEnumerable<ValueListData> FragilityList { get; set; }
        public IEnumerable<ValueListData> MaintenanceLogStatusList { get; set; }
        public IEnumerable<ValueListData> FinancialImpactList { get; set; }
        public IEnumerable<ValueListData> StrategicImportanceList { get; set; }

        [BindProperty]
        public OrganizationalValue OrganizationalValue { get; set; }

        [BindProperty]
        public EstimatedRunFrequency EstimatedRunFrequency { get; set; }

        [BindProperty]
        public MaintenanceSchedule MaintenanceSchedule { get; set; }

        [BindProperty]
        public FragilityTag FragilityTag { get; set; }

        [BindProperty]
        public Fragility Fragility { get; set; }

        [BindProperty]
        public MaintenanceLogStatus MaintenanceLogStatus { get; set; }

        [BindProperty]
        public FinancialImpact FinancialImpact { get; set; }

        [BindProperty]
        public StrategicImportance StrategicImportance { get; set; }

        public class ValueListData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Used { get; set; }
        }

        public async Task<IActionResult> OnGetOrganizationalValueList()
        {
            ViewData["OrganizationalValueList"] = await (
                from o in _context.OrganizationalValues
                select new ValueListData
                {
                    Id = o.OrganizationalValueId,
                    Name = o.OrganizationalValueName,
                    Used = o.ReportObjectDocs.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_OrganizationalValueList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetEstimatedRunFrequencyList()
        {
            ViewData["EstimatedRunFrequencyList"] = await (
                from o in _context.EstimatedRunFrequencies
                select new ValueListData
                {
                    Id = o.EstimatedRunFrequencyId,
                    Name = o.EstimatedRunFrequencyName,
                    Used = o.ReportObjectDocs.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_EstimatedRunFrequencyList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetMaintenanceScheduleList()
        {
            ViewData["MaintenanceScheduleList"] = await (
                from o in _context.MaintenanceSchedules
                select new ValueListData
                {
                    Id = o.MaintenanceScheduleId,
                    Name = o.MaintenanceScheduleName,
                    Used = o.ReportObjectDocs.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_MaintenanceScheduleList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetFragilityList()
        {
            ViewData["FragilityList"] = await (
                from o in _context.Fragilities
                select new ValueListData
                {
                    Id = o.FragilityId,
                    Name = o.FragilityName,
                    Used = o.ReportObjectDocs.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_FragilityList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetFragilityTagList()
        {
            ViewData["FragilityTagList"] = await (
                from o in _context.FragilityTags
                select new ValueListData
                {
                    Id = o.FragilityTagId,
                    Name = o.FragilityTagName,
                    Used = o.ReportObjectDocFragilityTags.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_FragilityTagList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetMaintenanceLogStatusList()
        {
            ViewData["MaintenanceLogStatusList"] = await (
                from o in _context.MaintenanceLogStatuses
                select new ValueListData
                {
                    Id = o.MaintenanceLogStatusId,
                    Name = o.MaintenanceLogStatusName,
                    Used = o.MaintenanceLogs.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_MaintenanceLogStatusList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetFinancialImpactList()
        {
            ViewData["FinancialImpactList"] = await (
                from o in _context.FinancialImpacts
                select new ValueListData
                {
                    Id = o.FinancialImpactId,
                    Name = o.Name,
                    Used = o.Initiatives.Count + o.Collections.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_FinancialImpactList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetStrategicImportanceList()
        {
            ViewData["StrategicImportanceList"] = await (
                from o in _context.StrategicImportances
                select new ValueListData
                {
                    Id = o.StrategicImportanceId,
                    Name = o.Name,
                    Used = o.Initiatives.Count + o.Collections.Count
                }
            ).ToListAsync();
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );

            return new PartialViewResult
            {
                ViewName = "Partials/_StrategicImportanceList",
                ViewData = ViewData
            };
        }

        public ActionResult OnGet()
        {
            return Page();
        }

        public ActionResult OnPostCreateOrganizationalValue()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );

            if (
                ModelState.IsValid
                && OrganizationalValue.OrganizationalValueName != null
                && checkpoint
            )
            {
                _context.Add(OrganizationalValue);
                _context.SaveChanges();
            }
            _cache.Remove("org-value");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteOrganizationalValue()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && OrganizationalValue.OrganizationalValueId > 0 && checkpoint)
            {
                _context.ReportObjectDocs
                    .Where(
                        x =>
                            x.OrganizationalValueId.Equals(
                                OrganizationalValue.OrganizationalValueId
                            )
                    )
                    .ToList()
                    .ForEach(q => q.OrganizationalValueId = null);
                _context.Remove(OrganizationalValue);
                _context.SaveChanges();
            }
            _cache.Remove("org-value");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateEstimatedRunFrequency()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (
                ModelState.IsValid
                && EstimatedRunFrequency.EstimatedRunFrequencyName != null
                && checkpoint
            )
            {
                _context.Add(EstimatedRunFrequency);
                _context.SaveChanges();
            }
            _cache.Remove("run-freq");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteEstimatedRunFrequency()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (
                ModelState.IsValid
                && EstimatedRunFrequency.EstimatedRunFrequencyId > 0
                && checkpoint
            )
            {
                _context.ReportObjectDocs
                    .Where(
                        x =>
                            x.EstimatedRunFrequencyId.Equals(
                                EstimatedRunFrequency.EstimatedRunFrequencyId
                            )
                    )
                    .ToList()
                    .ForEach(q => q.EstimatedRunFrequencyId = null);
                _context.Remove(EstimatedRunFrequency);
                _context.SaveChanges();
            }
            _cache.Remove("run-freq");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateFragility()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (ModelState.IsValid && Fragility.FragilityName != null && checkpoint)
            {
                _context.Add(Fragility);
                _context.SaveChanges();
            }
            _cache.Remove("fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFragility()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && Fragility.FragilityId > 0 && checkpoint)
            {
                _context.ReportObjectDocs
                    .Where(x => x.FragilityId.Equals(Fragility.FragilityId))
                    .ToList()
                    .ForEach(q => q.FragilityId = null);
                _context.Remove(Fragility);
                _context.SaveChanges();
            }
            _cache.Remove("fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateMaintenanceSchedule()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (
                ModelState.IsValid
                && MaintenanceSchedule.MaintenanceScheduleName != null
                && checkpoint
            )
            {
                _context.Add(MaintenanceSchedule);
                _context.SaveChanges();
            }
            _cache.Remove("maint-sched");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteMaintenanceSchedule()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && MaintenanceSchedule.MaintenanceScheduleId > 0 && checkpoint)
            {
                _context.ReportObjectDocs
                    .Where(
                        x =>
                            x.MaintenanceScheduleId.Equals(
                                MaintenanceSchedule.MaintenanceScheduleId
                            )
                    )
                    .ToList()
                    .ForEach(q => q.MaintenanceScheduleId = null);
                _context.Remove(MaintenanceSchedule);
                _context.SaveChanges();
            }
            _cache.Remove("maint-sched");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateFragilityTag()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (ModelState.IsValid && FragilityTag.FragilityTagName != null && checkpoint)
            {
                _context.Add(FragilityTag);
                _context.SaveChanges();
            }
            _cache.Remove("ro-fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFragilityTag()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && FragilityTag.FragilityTagId > 0 && checkpoint)
            {
                _context.RemoveRange(
                    _context.ReportObjectDocFragilityTags.Where(
                        x => x.FragilityTagId.Equals(FragilityTag.FragilityTagId)
                    )
                );
                _context.Remove(FragilityTag);
                _context.SaveChanges();
            }
            _cache.Remove("ro-fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateMaintenanceLogStatus()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (
                ModelState.IsValid
                && MaintenanceLogStatus.MaintenanceLogStatusName != null
                && checkpoint
            )
            {
                _context.Add(MaintenanceLogStatus);
                _context.SaveChanges();
            }
            _cache.Remove("maint-log-status");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteMaintenanceLogStatus()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && MaintenanceLogStatus.MaintenanceLogStatusId > 0 && checkpoint)
            {
                _context.RemoveRange(
                    _context.MaintenanceLogs.Where(
                        x =>
                            x.MaintenanceLogStatusId.Equals(
                                MaintenanceLogStatus.MaintenanceLogStatusId
                            )
                    )
                );
                _context.Remove(MaintenanceLogStatus);
                _context.SaveChanges();
            }
            _cache.Remove("maint-log-status");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateFinancialImpact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (ModelState.IsValid && FinancialImpact.Name != null && checkpoint)
            {
                _context.Add(FinancialImpact);
                _context.SaveChanges();
            }
            _cache.Remove("financial-impact");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFinancialImpact()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && FinancialImpact.FinancialImpactId > 0 && checkpoint)
            {
                _context.Collections
                    .Where(x => x.FinancialImpact.Equals(FinancialImpact.FinancialImpactId))
                    .ToList()
                    .ForEach(q => q.FinancialImpact = null);
                _context.Initiatives
                    .Where(x => x.FinancialImpact.Equals(FinancialImpact.FinancialImpactId))
                    .ToList()
                    .ForEach(q => q.FinancialImpact = null);
                _context.Remove(FinancialImpact);
                _context.SaveChanges();
            }
            _cache.Remove("financial-impact");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateStrategicImportance()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                33
            );
            if (ModelState.IsValid && StrategicImportance.Name != null && checkpoint)
            {
                _context.Add(StrategicImportance);
                _context.SaveChanges();
            }
            _cache.Remove("strategic-importance");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteStrategicImportance()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                34
            );
            if (ModelState.IsValid && StrategicImportance.StrategicImportanceId > 0 && checkpoint)
            {
                _context.Collections
                    .Where(
                        x => x.StrategicImportance.Equals(StrategicImportance.StrategicImportanceId)
                    )
                    .ToList()
                    .ForEach(q => q.StrategicImportance = null);
                _context.Initiatives
                    .Where(
                        x => x.StrategicImportance.Equals(StrategicImportance.StrategicImportanceId)
                    )
                    .ToList()
                    .ForEach(q => q.StrategicImportance = null);
                _context.Remove(StrategicImportance);
                _context.SaveChanges();
            }
            _cache.Remove("strategic-importance");
            return RedirectToPage("/Settings/Index");
        }
    }
}
