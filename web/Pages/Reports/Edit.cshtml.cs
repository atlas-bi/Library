using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Atlas_Web.Helpers;
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
        public ReportManageEngineTicket ServiceRequest { get; set; }

        [BindProperty]
        public List<ReportManageEngineTicket> ServiceRequests { get; set; }

        public ReportObjectImagesDoc RemovedImage { get; set; }

        [BindProperty]
        public ReportObjectDoc Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );

            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Reports/Index",
                    new { id, error = "You do not have permission to access that page." }
                );
            }

            if (!_context.ReportObjectDocs.Any(x => x.ReportObjectId == id))
            {
                _context.Add(
                    new ReportObjectDoc
                    {
                        ReportObjectId = id,
                        CreatedDateTime = DateTime.Now,
                        CreatedBy = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId
                    }
                );
                _context.SaveChanges();
            }

            Report = await _context.ReportObjectDocs
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.ReportObjectType)
                /* images */
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.ReportObjectImagesDocs)
                /* maintenance logs */
                .Include(x => x.ReportObjectDocMaintenanceLogs)
                .ThenInclude(x => x.MaintenanceLog)
                .ThenInclude(x => x.Maintainer)
                .Include(x => x.ReportObjectDocMaintenanceLogs)
                .ThenInclude(x => x.MaintenanceLog)
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
                .Include(x => x.ReportObject)
                .ThenInclude(x => x.ReportObjectDoc)
                .ThenInclude(x => x.ReportManageEngineTickets)
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
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );

            if (!checkpoint)
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
            OldReport.UpdatedBy = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            _context.SaveChanges();

            // updated any linked terms that were added and remove any that were delinked.
            _cache.Remove("terms");
            foreach (var term in Terms.Distinct())
            {
                _cache.Remove("term-" + term.TermId);
                term.ReportObjectId = id;

                if (
                    !_context.ReportObjectDocTerms.Any(
                        x => x.TermId == term.TermId && x.ReportObjectId == term.ReportObjectId
                    )
                )
                {
                    _context.Add(term);
                }
            }
            _context.SaveChanges();

            var RemovedTerms = _context.ReportObjectDocTerms
                .Where(d => d.ReportObjectId == id)
                .Where(d => !Terms.Select(x => x.TermId).Contains(d.TermId));

            foreach (var term in RemovedTerms)
            {
                _cache.Remove("term-" + term.TermId);
            }

            _context.RemoveRange(RemovedTerms);
            _context.SaveChanges();

            // update linked collections
            _cache.Remove("collections");
            for (int i = 0; i < Collections.Count; i++)
            {
                CollectionReport collection = Collections[i];

                _cache.Remove("collection-" + collection.DataProjectId);
                collection.ReportId = id;
                collection.Rank = i;

                // if annotation exists, update rank and text
                CollectionReport oldCollection = _context.CollectionReports
                    .Where(
                        x =>
                            x.ReportId == collection.ReportId
                            && x.DataProjectId == collection.DataProjectId
                    )
                    .FirstOrDefault();
                if (oldCollection != null)
                {
                    oldCollection.Rank = i;
                    _cache.Remove("collection-" + oldCollection.DataProjectId);
                }
                else
                {
                    _context.Add(collection);
                }

                _context.SaveChanges();
            }

            var RemovedCollections = _context.CollectionReports
                .Where(d => d.ReportId == id)
                .Where(
                    d => !Collections.Select(x => x.DataProjectId).Contains((int)d.DataProjectId)
                );

            foreach (var collection in RemovedCollections)
            {
                _cache.Remove("collection-" + collection.DataProjectId);
            }

            _context.RemoveRange(RemovedCollections);
            _context.SaveChanges();

            // update fragility tags
            foreach (var tag in FragilityTags.Distinct())
            {
                tag.ReportObjectId = id;

                if (
                    !_context.ReportObjectDocFragilityTags.Any(
                        x =>
                            x.FragilityTagId == tag.FragilityTagId
                            && x.ReportObjectId == tag.ReportObjectId
                    )
                )
                {
                    _context.Add(tag);
                }
            }
            _context.SaveChanges();

            _context.RemoveRange(
                _context.ReportObjectDocFragilityTags
                    .Where(d => d.ReportObjectId == id)
                    .Where(
                        d => !FragilityTags.Select(x => x.FragilityTagId).Contains(d.FragilityTagId)
                    )
            );
            _context.SaveChanges();

            // remove removed images
            _context.RemoveRange(
                _context.ReportObjectImagesDocs
                    .Where(d => d.ReportObjectId == id)
                    .Where(d => !Images.Select(x => x.ImageId).Contains(d.ImageId))
            );
            _context.SaveChanges();

            // update image rank
            for (int i = 0; i < Images.Count; i++)
            {
                ReportObjectImagesDoc image = _context.ReportObjectImagesDocs
                    .Where(x => x.ImageId == Images[i].ImageId && x.ReportObjectId == id)
                    .FirstOrDefault();
                if (image != null)
                {
                    image.ImageOrdinal = i;
                    _context.SaveChanges();
                }
            }

            // add new maintenance log
            if (MaintenanceLog.MaintenanceLogStatusId != null)
            {
                MaintenanceLog.MaintenanceDate = DateTime.Now;
                MaintenanceLog.MaintainerId =
                    UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                _context.Add(MaintenanceLog);
                _context.SaveChanges();

                _context.Add(
                    new ReportObjectDocMaintenanceLog
                    {
                        MaintenanceLogId = MaintenanceLog.MaintenanceLogId,
                        ReportObjectId = id
                    }
                );
                _context.SaveChanges();
            }

            // remove deleted service requests
            _context.RemoveRange(
                _context.ReportManageEngineTickets
                    .Where(d => d.ReportObjectId == id)
                    .Where(
                        d =>
                            !ServiceRequests
                                .Select(x => x.ManageEngineTicketsId)
                                .Contains(d.ManageEngineTicketsId)
                    )
            );
            _context.SaveChanges();

            // add service ticket
            if (ServiceRequest.TicketNumber != null)
            {
                ServiceRequest.ReportObjectId = id;
                _context.Add(ServiceRequest);
                _context.SaveChanges();
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

        public ActionResult OnPostAddImage(int Id, IFormFile File)
        {
            if (ValidateFileUpload(File))
            {
                var img = new ReportObjectImagesDoc { ReportObjectId = Id };
                using (var stream = new MemoryStream())
                {
                    File.CopyTo(stream);
                    img.ImageData = stream.ToArray();
                }
                _context.Add(img);
                _context.SaveChanges();

                return Content(img.ImageId.ToString());
            }
            // be sure and clear the cache
            _cache.Remove("report-" + Id);
            return Content("error");
        }
    }
}
