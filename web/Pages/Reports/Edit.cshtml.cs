using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Atlas_Web.Pages.Reports
{
    public class EditModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;

        public EditModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public List<CollectionReport> Collections { get; set; }

        [BindProperty]
        public List<ReportObjectDocTerm> Terms { get; set; }

        [BindProperty]
        public List<ReportObjectImagesDoc> Images { get; set; }

        [BindProperty]
        public List<ReportObjectDocFragilityTag> FragilityTags { get; set; }

        [BindProperty]
        public MaintenanceLog MaintenanceLog { get; set; }

        [BindProperty]
        public ReportServiceRequest ServiceRequest { get; set; }

        [BindProperty]
        public List<ReportServiceRequest> ServiceRequests { get; set; }

        [BindProperty]
        public ReportObjectDoc Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.HasPermission("Edit Collection"))
            {
                return RedirectToPage(
                    "/Reports/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            if (!await _context.ReportObjectDocs.AnyAsync(x => x.ReportObjectId == id))
            {
                await _context.AddAsync(
                    new ReportObjectDoc
                    {
                        ReportObjectId = id,
                        CreatedDateTime = DateTime.Now,
                        CreatedBy = User.GetUserId()
                    }
                );
                await _context.SaveChangesAsync();
            }

            Report = await _context.ReportObjectDocs
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.ReportObjectType)
                /* images */
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.ReportObjectImagesDocs)
                /* maintenance logs */
                .Include(x => x.MaintenanceLogs)
                .ThenInclude(x => x.Maintainer)
                .Include(x => x.MaintenanceLogs)
                .ThenInclude(x => x.MaintenanceLogStatus)
                /* meta */
                .Include(x => x.EstimatedRunFrequency)
                .Include(x => x.Fragility)
                .Include(x => x.ReportObjectDocFragilityTags)
                .ThenInclude(x => x.FragilityTag)
                .Include(x => x.MaintenanceSchedule)
                .Include(x => x.OrganizationalValue)
                /* users */
                .Include(x => x.RequesterNavigation)
                .Include(x => x.OperationalOwnerUser)
                /* me tickets */
                .Include(x => x.ReportServiceRequests)
                /* collections */
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.CollectionReports)
                .ThenInclude(x => x.DataProject)
                /* terms */
                .Include(x => x.ReportObjectDocTerms)
                .ThenInclude(x => x.Term)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.ReportObjectId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!User.HasPermission("Edit Report Documentation"))
            {
                return RedirectToPage(
                    "/Reports/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            _cache.Remove("report-" + id);
            _cache.Remove("report-terms-" + id);
            _cache.Remove("search-report-" + id);

            ReportObjectDoc OldReport = await _context.ReportObjectDocs.SingleOrDefaultAsync(
                x => x.ReportObjectId == id
            );

            OldReport.DeveloperDescription = Report.DeveloperDescription;
            OldReport.KeyAssumptions = Report.KeyAssumptions;
            OldReport.OperationalOwnerUserId = Report.OperationalOwnerUserId;
            OldReport.Requester = Report.Requester;
            OldReport.GitLabProjectUrl = Report.GitLabProjectUrl;
            OldReport.OrganizationalValueId = Report.OrganizationalValueId;
            OldReport.EstimatedRunFrequencyId = Report.EstimatedRunFrequencyId;
            OldReport.FragilityId = Report.FragilityId;
            OldReport.MaintenanceScheduleId = Report.MaintenanceScheduleId;
            OldReport.ExecutiveVisibilityYn = Report.ExecutiveVisibilityYn;
            OldReport.DoNotPurge = Report.DoNotPurge;
            OldReport.Hidden = Report.Hidden;
            OldReport.LastUpdateDateTime = DateTime.Now;
            OldReport.EnabledForHyperspace = Report.EnabledForHyperspace;
            OldReport.UpdatedBy = User.GetUserId();

            await _context.SaveChangesAsync();

            // updated any linked terms that were added and remove any that were delinked.
            _cache.Remove("terms");
            foreach (var term in Terms.Distinct())
            {
                _cache.Remove("term-" + term.TermId);
                term.ReportObjectId = id;

                if (
                    !await _context.ReportObjectDocTerms.AnyAsync(
                        x => x.TermId == term.TermId && x.ReportObjectId == term.ReportObjectId
                    )
                )
                {
                    await _context.AddAsync(term);
                }
            }
            await _context.SaveChangesAsync();

            var RemovedTerms = _context.ReportObjectDocTerms
                .Where(d => d.ReportObjectId == id)
                .Where(d => !Terms.Select(x => x.TermId).Contains(d.TermId));

            foreach (var term in await RemovedTerms.ToListAsync())
            {
                _cache.Remove("term-" + term.TermId);
            }

            _context.RemoveRange(RemovedTerms);
            await _context.SaveChangesAsync();

            // update linked collections
            _cache.Remove("collections");
            for (int i = 0; i < Collections.Count; i++)
            {
                CollectionReport collection = Collections[i];

                _cache.Remove("collection-" + collection.CollectionId);
                _cache.Remove("search-collection-" + collection.CollectionId);
                collection.ReportId = id;
                collection.Rank = i;

                // if annotation exists, update rank and text
                CollectionReport oldCollection = await _context.CollectionReports
                    .Where(
                        x =>
                            x.ReportId == collection.ReportId
                            && x.CollectionId == collection.CollectionId
                    )
                    .FirstOrDefaultAsync();
                if (oldCollection != null)
                {
                    oldCollection.Rank = i;
                    _cache.Remove("collection-" + oldCollection.CollectionId);
                    _cache.Remove("search-collection-" + oldCollection.CollectionId);
                }
                else
                {
                    await _context.AddAsync(collection);
                }

                await _context.SaveChangesAsync();
            }

            var RemovedCollections = _context.CollectionReports
                .Where(d => d.ReportId == id)
                .Where(d => !Collections.Select(x => x.CollectionId).Contains(d.CollectionId));

            foreach (var collection in await RemovedCollections.ToListAsync())
            {
                _cache.Remove("collection-" + collection.CollectionId);
            }

            _cache.Remove("collections");

            _context.RemoveRange(RemovedCollections);
            await _context.SaveChangesAsync();

            // update fragility tags
            foreach (var tag in FragilityTags.Distinct())
            {
                tag.ReportObjectId = id;

                if (
                    !await _context.ReportObjectDocFragilityTags.AnyAsync(
                        x =>
                            x.FragilityTagId == tag.FragilityTagId
                            && x.ReportObjectId == tag.ReportObjectId
                    )
                )
                {
                    await _context.AddAsync(tag);
                }
            }
            await _context.SaveChangesAsync();

            _context.RemoveRange(
                _context.ReportObjectDocFragilityTags
                    .Where(d => d.ReportObjectId == id)
                    .Where(
                        d => !FragilityTags.Select(x => x.FragilityTagId).Contains(d.FragilityTagId)
                    )
            );
            await _context.SaveChangesAsync();

            // remove removed images
            _context.RemoveRange(
                _context.ReportObjectImagesDocs
                    .Where(d => d.ReportObjectId == id)
                    .Where(d => !Images.Select(x => x.ImageId).Contains(d.ImageId))
            );
            await _context.SaveChangesAsync();

            // update image rank
            for (int i = 0; i < Images.Count; i++)
            {
                ReportObjectImagesDoc image = await _context.ReportObjectImagesDocs
                    .Where(x => x.ImageId == Images[i].ImageId && x.ReportObjectId == id)
                    .FirstOrDefaultAsync();
                if (image != null)
                {
                    image.ImageOrdinal = i;
                    await _context.SaveChangesAsync();
                }
            }

            // add new maintenance log
            if (MaintenanceLog.MaintenanceLogStatusId != null)
            {
                MaintenanceLog.MaintenanceDate = DateTime.Now;
                MaintenanceLog.ReportId = id;
                MaintenanceLog.MaintainerId = User.GetUserId();
                await _context.AddAsync(MaintenanceLog);
                await _context.SaveChangesAsync();
            }

            // remove deleted service requests
            _context.RemoveRange(
                _context.ReportServiceRequests
                    .Where(d => d.ReportObjectId == id)
                    .Where(
                        d =>
                            !ServiceRequests
                                .Select(x => x.ServiceRequestId)
                                .Contains(d.ServiceRequestId)
                    )
            );
            await _context.SaveChangesAsync();

            // add service ticket
            if (ServiceRequest.TicketNumber != null)
            {
                ServiceRequest.ReportObjectId = id;
                await _context.AddAsync(ServiceRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Reports/Index", new { id, success = "Changes saved." });
        }

        public async Task<ActionResult> OnPostCurrentTermDetails(int TermId)
        {
            var Term = await _context.Terms.Where(r => r.TermId == TermId).FirstOrDefaultAsync();
            var json = JsonConvert.SerializeObject(Term);
            HttpContext.Response.Headers.Remove("Cache-Control");
            HttpContext.Response.Headers.Add("Cache-Control", "max-age=0");
            return Content(json);
        }

        // https://docs.microsoft.com/en-us/aspnet/core/razor-pages/upload-files?view=aspnetcore-2.2
        public bool ValidateFileUpload(IFormFile file)
        {
            try
            {
                // get the filename in case we need to return info to the user
                if (
                    file.ContentType.ToLower() != "image/jpeg"
                    && file.ContentType.ToLower() != "image/png"
                    && file.ContentType.ToLower() != "image/gif"
                )
                {
                    ModelState.AddModelError(
                        file.Name,
                        "You may only upload jpeg, png or gif files."
                    );
                    return false;
                }

                if (file.Length > 1024 * 1024)
                {
                    ModelState.AddModelError(
                        file.Name,
                        "The file is larger than 1MB. Please use a smaller image."
                    );
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    file.Name,
                    $"The file upload failed. Please contact Analytics if this issue persists. Error: {ex.Message}"
                );
                return false;
            }
            // if there are no errors, return true
            return true;
        }

        public async Task<ActionResult> OnPostAddImage(int Id, IFormFile File)
        {
            if (ValidateFileUpload(File))
            {
                var img = new ReportObjectImagesDoc { ReportObjectId = Id };
                using (var stream = new MemoryStream())
                {
                    await File.CopyToAsync(stream);
                    img.ImageData = stream.ToArray();
                }
                await _context.AddAsync(img);
                await _context.SaveChangesAsync();

                return Content(img.ImageId.ToString());
            }
            // be sure and clear the cache
            _cache.Remove("report-" + Id);
            return Content("error");
        }
    }
}
