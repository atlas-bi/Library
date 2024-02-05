using Atlas_Web.Authorization;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
                    Name = o.Name,
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
                    Id = o.Id,
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
                    Id = o.Id,
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
                && OrganizationalValue.Name != null
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
                && OrganizationalValue.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context
                    .ReportObjectDocs.Where(x =>
                        x.OrganizationalValueId.Equals(OrganizationalValue.Id)
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
                && EstimatedRunFrequency.Name != null
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
                && EstimatedRunFrequency.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context
                    .ReportObjectDocs.Where(x =>
                        x.EstimatedRunFrequencyId.Equals(EstimatedRunFrequency.Id)
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
                && Fragility.Name != null
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
            if (ModelState.IsValid && Fragility.Id > 0 && User.HasPermission("Delete Parameters"))
            {
                _context
                    .ReportObjectDocs.Where(x => x.FragilityId.Equals(Fragility.Id))
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
                && MaintenanceSchedule.Name != null
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
                && MaintenanceSchedule.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context
                    .ReportObjectDocs.Where(x =>
                        x.MaintenanceScheduleId.Equals(MaintenanceSchedule.Id)
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
                && FragilityTag.Name != null
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
                && FragilityTag.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context.RemoveRange(
                    _context.ReportObjectDocFragilityTags.Where(x =>
                        x.FragilityTagId.Equals(FragilityTag.Id)
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
                && MaintenanceLogStatus.Name != null
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
                && MaintenanceLogStatus.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context.RemoveRange(
                    _context.MaintenanceLogs.Where(x =>
                        x.MaintenanceLogStatusId.Equals(MaintenanceLogStatus.Id)
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
                && FinancialImpact.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context
                    .Collections.Where(x => x.FinancialImpact.Equals(FinancialImpact.Id))
                    .ToList()
                    .ForEach(q => q.FinancialImpact = null);
                _context
                    .Initiatives.Where(x => x.FinancialImpact.Equals(FinancialImpact.Id))
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
                && StrategicImportance.Id > 0
                && User.HasPermission("Delete Parameters")
            )
            {
                _context
                    .Collections.Where(x => x.StrategicImportance.Equals(StrategicImportance.Id))
                    .ToList()
                    .ForEach(q => q.StrategicImportance = null);
                _context
                    .Initiatives.Where(x => x.StrategicImportance.Equals(StrategicImportance.Id))
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
