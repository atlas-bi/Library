using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public class MaintStatus
        {
            public string Required { get; set; }
        }

        public List<AdList> AdLists { get; set; }
        public ReportObject Report { get; set; }
        public List<ReportObject> Children { get; set; }
        public List<ReportObject> Parents { get; set; }
        public List<Term> Terms { get; set; }
        public List<ReportObjectQuery> ComponentQueries { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Report = await _cache.GetOrCreateAsync<ReportObject>(
                "report-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                    return _context.ReportObjects
                        /* run the 1:1 as a single query */
                        .Include(x => x.ReportObjectImagesDocs)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.MaintenanceSchedule)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.OrganizationalValue)
                        .Include(x => x.LastModifiedByUser)
                        .Include(x => x.AuthorUser)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.UpdatedByNavigation)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.RequesterNavigation)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.OperationalOwnerUser)
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.EstimatedRunFrequency)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.Fragility)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.MaintenanceLogs)
                        .ThenInclude(x => x.Maintainer)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.MaintenanceLogs)
                        .ThenInclude(x => x.MaintenanceLogStatus)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.ReportObjectDocFragilityTags)
                        .ThenInclude(x => x.FragilityTag)
                        .Include(x => x.ReportObjectTagMemberships)
                        .ThenInclude(x => x.Tag)
                        .Include(x => x.ReportObjectDoc)
                        .ThenInclude(x => x.ReportServiceRequests)
                        .Include(x => x.ReportObjectAttachments)
                        .Include(x => x.ReportGroupsMemberships)
                        .ThenInclude(x => x.Group)
                        .Include(x => x.ReportObjectQueries)
                        .Include(x => x.CollectionReports)
                        .ThenInclude(x => x.DataProject)
                        .Include(x => x.StarredReports)
                        .Include(x => x.ReportObjectParameters)
                        .Include(x => x.ReportTagLinks)
                        .ThenInclude(x => x.Tag)
                        // needed for authorization
                        .Include(x => x.ReportObjectHierarchyChildReportObjects)
                        .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.ReportObjectId == id);
                }
            );

            Terms = await _cache.GetOrCreateAsync<List<Term>>(
                "report-terms-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.Terms
                        .Where(x => x.ReportObjectDocTerms.Any(x => x.ReportObjectId == id))
                        // from first children
                        .Union(
                            _context.Terms.Where(
                                x =>
                                    x.ReportObjectDocTerms.Any(
                                        x =>
                                            x.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                x => x.ParentReportObjectId == id
                                            )
                                    )
                            )
                        // second level children
                        )
                        .Union(
                            _context.Terms.Where(
                                x =>
                                    x.ReportObjectDocTerms.Any(
                                        x =>
                                            x.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                x =>
                                                    x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                        x => x.ParentReportObjectId == id
                                                    )
                                            )
                                    )
                            )
                        // third level children
                        )
                        .Union(
                            _context.Terms.Where(
                                x =>
                                    x.ReportObjectDocTerms.Any(
                                        x =>
                                            x.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                x =>
                                                    x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                        x =>
                                                            x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                                x => x.ParentReportObjectId == id
                                                            )
                                                    )
                                            )
                                    )
                            )
                        // fourth level children
                        )
                        .Union(
                            _context.Terms.Where(
                                x =>
                                    x.ReportObjectDocTerms.Any(
                                        x =>
                                            x.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                x =>
                                                    x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                        x =>
                                                            x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                                x =>
                                                                    x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                                        x =>
                                                                            x.ParentReportObjectId
                                                                            == id
                                                                    )
                                                            )
                                                    )
                                            )
                                    )
                            )
                        )
                        .Distinct()
                        .AsNoTracking()
                        .ToListAsync();
                }
            );

            ComponentQueries = await _cache.GetOrCreateAsync<List<ReportObjectQuery>>(
                "report-comp-queries-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.ReportObjectQueries
                        .Where(
                            x =>
                                x.ReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                    x =>
                                        x.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                            x =>
                                                x.ParentReportObjectId == id
                                                && x.ParentReportObject.EpicMasterFile == "IDB"
                                        )
                                )
                        )
                        .Include(x => x.ReportObject)
                        .AsNoTracking()
                        .ToListAsync();
                }
            );

            // a child is anything in hierarchy that is not an IDK (resource)
            // if there is an IDK, its children should be bumped up on level.
            // IDN > IDK (ignored) > IDB
            Children = await _cache.GetOrCreateAsync<List<ReportObject>>(
                "report-children-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.ReportObjects
                        .Where(
                            x =>
                                x.ReportObjectHierarchyChildReportObjects.Any(
                                    y => y.ParentReportObjectId == id
                                )
                        )
                        .Where(x => x.EpicMasterFile != "IDK")
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectAttachments)
                        .Union(
                            _context.ReportObjects
                                .Where(
                                    x =>
                                        x.ReportObjectHierarchyChildReportObjects.Any(
                                            y =>
                                                y.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(
                                                    g =>
                                                        g.ParentReportObjectId == id
                                                        && g.ParentReportObject.DefaultVisibilityYn
                                                            == "Y"
                                                )
                                                && y.ParentReportObject.EpicMasterFile == "IDK"
                                        )
                                )
                                .Where(x => x.EpicMasterFile == "IDN")
                                .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                                .Include(x => x.ReportObjectDoc)
                                .Include(x => x.ReportObjectType)
                                .Include(x => x.ReportObjectAttachments)
                        )
                        .AsNoTracking()
                        .ToListAsync();
                }
            );

            // a parent is anything in hierarchy that is not an IDK (resource)
            // if there is an IDK, its parent should be bumped up one level.
            // also exclude personal dashboards
            // IDN > IDK (ignored) > IDB
            Parents = await _cache.GetOrCreateAsync<List<ReportObject>>(
                "report-parents-" + id,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.ReportObjects
                        .Where(
                            x =>
                                x.ReportObjectHierarchyParentReportObjects.Any(
                                    y => y.ChildReportObjectId == id
                                )
                        )
                        .Where(x => x.ReportObjectTypeId != 12) //Personal dashboard
                        .Where(x => x.EpicMasterFile != "IDK")
                        .Where(x => x.DefaultVisibilityYn == "Y")
                        .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                        .Include(x => x.ReportObjectDoc)
                        .Include(x => x.ReportObjectType)
                        .Include(x => x.ReportObjectAttachments)
                        .Union(
                            _context.ReportObjects
                                .Where(
                                    x =>
                                        x.ReportObjectHierarchyParentReportObjects.Any(
                                            y =>
                                                y.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(
                                                    g => g.ChildReportObjectId == id
                                                )
                                                && y.ChildReportObject.EpicMasterFile == "IDK"
                                        )
                                )
                                .Where(x => x.EpicMasterFile == "IDB")
                                .Where(x => x.DefaultVisibilityYn == "Y")
                                .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
                                .Include(x => x.ReportObjectDoc)
                                .Include(x => x.ReportObjectType)
                                .Include(x => x.ReportObjectAttachments)
                        )
                        .AsNoTracking()
                        .ToListAsync();
                }
            );

            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentCollections", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;
            return Page();
        }

        public async Task<ActionResult> OnGetMaintStatusAsync(int id)
        {
            DateTime Today = DateTime.Now;
            ViewData["MaintStatus"] = await (
                from n in (
                    from l in (
                        from x in _context.ReportObjectDocs
                        where (x.MaintenanceScheduleId ?? 1) != 5 && x.ReportObjectId == id
                        select new
                        {
                            sch = x.MaintenanceScheduleId,
                            thiss = _context.MaintenanceLogs
                                .Where(
                                    x =>
                                        x.MaintainerId
                                        == _context.MaintenanceLogs
                                            .Select(x => x.MaintenanceLogId)
                                            .Max()
                                )
                                .First()
                                .MaintenanceDate,
                            x.ReportObjectId,
                            name = x.ReportObject.DisplayName
                        }
                    )
                    select new
                    {
                        l.ReportObjectId,
                        NextDate = l.sch == 1
                            ? (l.thiss ?? Today).AddMonths(3)
                            : // quarterly
                            l.sch == 2
                                ? (l.thiss ?? Today).AddMonths(6)
                                : // twice a year
                                l.sch == 3
                                    ? (l.thiss ?? Today).AddYears(1)
                                    : // yearly
                                    l.sch == 4
                                        ? (l.thiss ?? Today).AddYears(2)
                                        : // every two years
                                        (l.thiss ?? Today),
                        Name = l.name
                    }
                )
                where n.NextDate < Today
                orderby n.NextDate
                select new MaintStatus { Required = "Report requires maintenance." }
            ).AsNoTracking().FirstOrDefaultAsync();

            return new PartialViewResult
            {
                ViewName = "Partials/_MaintStatus",
                ViewData = ViewData
            };
        }
    }
}
