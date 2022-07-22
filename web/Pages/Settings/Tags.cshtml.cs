using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Atlas_Web.Authorization;
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
        public IEnumerable<ValueListData> TagList { get; set; }
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
        public Tag Tag { get; set; }

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
            public string Description { get; set; }
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

            return new PartialViewResult
            {
                ViewName = "Partials/_FragilityTagList",
                ViewData = ViewData
            };
        }

        public async Task<IActionResult> OnGetTagList()
        {
            ViewData["TagList"] = await (
                from o in _context.Tags
                select new ValueListData
                {
                    Id = o.TagId,
                    Name = o.Name,
                    Description = o.Description,
                    Used = o.ReportTagLinks.Count
                }
            ).ToListAsync();

            return new PartialViewResult { ViewName = "Partials/_TagList", ViewData = ViewData };
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
            if (
                ModelState.IsValid
                && OrganizationalValue.OrganizationalValueName != null
                && User.HasPermission("Create Parameters")
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
            if (
                ModelState.IsValid
                && OrganizationalValue.OrganizationalValueId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && EstimatedRunFrequency.EstimatedRunFrequencyName != null
                && User.HasPermission("Create Parameters")
            )
            {
                _context.Add(EstimatedRunFrequency);
                _context.SaveChanges();
            }
            _cache.Remove("run-freq");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateTag()
        {
            if (ModelState.IsValid && Tag.Name != null && User.HasPermission("Create Parameters"))
            {
                _context.Add(Tag);
                _context.SaveChanges();
            }
            _cache.Remove("tag");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteEstimatedRunFrequency()
        {
            if (
                ModelState.IsValid
                && EstimatedRunFrequency.EstimatedRunFrequencyId > 0
                && User.HasPermission("Delete Parameters")
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

        public ActionResult OnPostDeleteTag()
        {
            if (ModelState.IsValid && Tag.TagId > 0 && User.HasPermission("Delete Parameters"))
            {
                _context.RemoveRange(_context.ReportTagLinks.Where(x => x.TagId == Tag.TagId));
                _context.Remove(Tag);
                _context.SaveChanges();
            }
            _cache.Remove("tag");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostCreateFragility()
        {
            if (
                ModelState.IsValid
                && Fragility.FragilityName != null
                && User.HasPermission("Create Parameters")
            )
            {
                _context.Add(Fragility);
                _context.SaveChanges();
            }
            _cache.Remove("fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFragility()
        {
            if (
                ModelState.IsValid
                && Fragility.FragilityId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && MaintenanceSchedule.MaintenanceScheduleName != null
                && User.HasPermission("Create Parameters")
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
            if (
                ModelState.IsValid
                && MaintenanceSchedule.MaintenanceScheduleId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && FragilityTag.FragilityTagName != null
                && User.HasPermission("Create Parameters")
            )
            {
                _context.Add(FragilityTag);
                _context.SaveChanges();
            }
            _cache.Remove("ro-fragility");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFragilityTag()
        {
            if (
                ModelState.IsValid
                && FragilityTag.FragilityTagId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && MaintenanceLogStatus.MaintenanceLogStatusName != null
                && User.HasPermission("Create Parameters")
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
            if (
                ModelState.IsValid
                && MaintenanceLogStatus.MaintenanceLogStatusId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && FinancialImpact.Name != null
                && User.HasPermission("Create Parameters")
            )
            {
                _context.Add(FinancialImpact);
                _context.SaveChanges();
            }
            _cache.Remove("financial-impact");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteFinancialImpact()
        {
            if (
                ModelState.IsValid
                && FinancialImpact.FinancialImpactId > 0
                && User.HasPermission("Delete Parameters")
            )
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
            if (
                ModelState.IsValid
                && StrategicImportance.Name != null
                && User.HasPermission("Create Parameters")
            )
            {
                _context.Add(StrategicImportance);
                _context.SaveChanges();
            }
            _cache.Remove("strategic-importance");
            return RedirectToPage("/Settings/Index");
        }

        public ActionResult OnPostDeleteStrategicImportance()
        {
            if (
                ModelState.IsValid
                && StrategicImportance.StrategicImportanceId > 0
                && User.HasPermission("Delete Parameters")
            )
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
