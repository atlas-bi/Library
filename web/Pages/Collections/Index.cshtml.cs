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
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Atlas_Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Collections
{
    public class IndexModel : PageModel
    {
        // import model
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public IndexModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        [BindProperty]
        public DpDataProject DpDataProject { get; set; }

        [BindProperty]
        public DpReportAnnotation DpReportAnnotation { get; set; }

        [BindProperty]
        public DpTermAnnotation DpTermAnnotation { get; set; }

        [BindProperty]
        public DpAgreement MyDpAgreement { get; set; }

        [BindProperty]
        public int[] DpAgreementUsers { get; set; }

        [BindProperty]
        public DpDataProjectConversation NewComment { get; set; }

        [BindProperty]
        public DpDataProjectConversationMessage NewCommentReply { get; set; }

        public List<int?> Permissions { get; set; }
        public IEnumerable<AllCollectionsData> AllCollections { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public IEnumerable<CollectionCommentsData> CollectionComments { get; set; }
        public CollectionData Collection { get; set; }
        public List<AdList> AdLists { get; set; }

        public class AllCollectionsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Favorite { get; set; }
        }

        public class CollectionCommentsData
        {
            public string Date { get; set; }
            public int? ConvId { get; set; }
            public int? MessId { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
            public int CollectionId { get; set; }
        }

        public class CollectionData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Purpose { get; set; }
            public string OpsOwner { get; set; }
            public int? OpsOwnerId { get; set; }
            public string ExOwner { get; set; }
            public int? ExOwnerId { get; set; }
            public string AnOwner { get; set; }
            public int? AnOwnerId { get; set; }
            public string DataOwner { get; set; }
            public int? DataOwnerId { get; set; }
            public string FnclImpact { get; set; }
            public int? FnclImpactId { get; set; }
            public string StrgImport { get; set; }
            public string Hidden { get; set; }
            public int? StrgImportId { get; set; }
            public string UpdatedBy { get; set; }
            public string UpdateDate { get; set; }
            public IEnumerable<RelatedItemData> RelatedReports { get; set; }
            public IEnumerable<RelatedItemData> RelatedTerms { get; set; }
            public IEnumerable<AgreementsData> Agreements { get; set; }
            public string Favorite { get; set; }
        }

        public class RelatedItemData
        {
            public int? Id { get; set; }
            public int? ItemId { get; set; }
            public int? CollectionId { get; set; }
            public string Name { get; set; }
            public int? Rank { get; set; }
            public string Image { get; set; }
            public string CertTag { get; set; }
            public string ReportType { get; set; }
            public string Annotation { get; set; }
            public string Favorite { get; set; }
            public string RunReportUrl { get; set; }
            public string EpicMasterFile { get; set; }
            public string EditReportUrl { get; set; }
            public string ManageReportUrl { get; set; }
        }

        public class AgreementsData
        {
            public int? Id { get; set; }
            public int? Rank { get; set; }
            public string Description { get; set; }
            public DateTime? MeetingDate { get; set; }
            public string MeetingDateString { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public string EffectiveDateString { get; set; }
            public IEnumerable<AgreementUsersData> Users { get; set; }
        }

        public class AttachmentLinks
        {
            public string Name { get; set; }
            public string Src { get; set; }
            public int Id { get; set; }
            public int? Size { get; set; }
            public string Type { get; set; }
        }

        public class CollectionTermsData
        {
            public string Name { get; set; }
            public int? Id { get; set; }
            public string Summary { get; set; }
            public string Definition { get; set; }
            public int? ReportId { get; set; }
        }

        public class AgreementUsersData
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }

        public List<UserPreference> Preferences { get; set; }
        public User PublicUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["Fullname"] = MyUser.Fullname_Cust;

            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentCollections", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;

            // if the id null then list all
            if (id != null)
            {
                Collection = await (
                    from d in _context.DpDataProjects
                    where d.DataProjectId == id
                    select new CollectionData
                    {
                        Id = d.DataProjectId,
                        Name = d.Name,
                        Description = d.Description,
                        Purpose = d.Purpose,
                        OpsOwner = d.OperationOwner.Fullname_Cust,
                        OpsOwnerId = d.OperationOwnerId,
                        ExOwner = d.ExecutiveOwner.Fullname_Cust,
                        ExOwnerId = d.ExecutiveOwnerId,
                        AnOwner = d.AnalyticsOwner.Fullname_Cust,
                        AnOwnerId = d.AnalyticsOwnerId,
                        DataOwner = d.DataManager.Fullname_Cust,
                        DataOwnerId = d.DataManagerId,
                        FnclImpact = d.FinancialImpactNavigation.Name,
                        FnclImpactId = d.FinancialImpact,
                        StrgImport = d.StrategicImportanceNavigation.Name,
                        StrgImportId = d.StrategicImportance,
                        Hidden = d.Hidden,
                        UpdateDate = d.LastUpdatedDateDisplayString,
                        UpdatedBy = d.LastUpdateUserNavigation.Fullname_Cust,
                        RelatedReports = (
                            from r in d.DpReportAnnotations
                            join q in (
                                from f in _context.UserFavorites
                                where f.ItemType.ToLower() == "report" && f.UserId == MyUser.UserId
                                select new { f.ItemId }
                            )
                                on r.ReportId equals q.ItemId
                                into tmp
                            from rfi in tmp.DefaultIfEmpty()
                            orderby r.Report.ReportObjectType.Name ,r.Rank ,r.Report.Name
                            select new RelatedItemData
                            {
                                Id = r.ReportAnnotationId,
                                ItemId = r.ReportId,
                                Name = r.Report.DisplayName,
                                Rank = r.Rank,
                                ReportType = r.Report.ReportObjectType.Name,
                                CertTag = r.Report.CertificationTag,
                                EpicMasterFile = r.Report.EpicMasterFile,
                                Annotation = r.Report.ReportObjectDoc.DeveloperDescription,
                                Image = r.Report.ReportObjectImagesDocs.Any()
                                  ? r.Report.ReportObjectImagesDocs.First().ImageId.ToString()
                                  : "",
                                Favorite = rfi.ItemId == null ? "no" : "yes",
                                RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                                    _config["AppSettings:org_domain"],
                                    HttpContext,
                                    r.Report,
                                    _context,
                                    User.Identity.Name
                                ),
                                ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(
                                    _config["AppSettings:org_domain"],
                                    HttpContext,
                                    r.Report.ReportObjectType.Name,
                                    r.Report.ReportServerPath,
                                    r.Report.SourceServer
                                ),
                                EditReportUrl = HtmlHelpers.EditReportFromParams(
                                    _config["AppSettings:org_domain"],
                                    HttpContext,
                                    r.Report.ReportServerPath,
                                    r.Report.SourceServer,
                                    r.Report.EpicMasterFile,
                                    r.Report.EpicReportTemplateId.ToString(),
                                    r.Report.EpicRecordId.ToString()
                                ),
                            }
                        ).ToList(),
                        Agreements = (
                            from r in d.DpAgreements
                            orderby r.Rank ,r.EffectiveDate
                            select new AgreementsData
                            {
                                Id = r.AgreementId,
                                Description = r.Description,
                                Rank = r.Rank,
                                MeetingDate = r.MeetingDate,
                                MeetingDateString = r.MeetingDateDisplayString,
                                EffectiveDate = r.EffectiveDate,
                                EffectiveDateString = r.EffectiveDateDisplayString,
                                Users =
                                    from u in r.DpAgreementUsers
                                    select new AgreementUsersData
                                    {
                                        Id = u.UserId,
                                        Name = u.User.Fullname_Cust
                                    }
                            }
                        ).ToList(),
                        Favorite = (
                            from f in _context.UserFavorites
                            where
                                f.ItemType == "collection"
                                && f.UserId == MyUser.UserId
                                && f.ItemId == d.DataProjectId
                            select new { f.ItemId }
                        ).Any()
                          ? "yes"
                          : "no"
                    }
                ).FirstOrDefaultAsync();

                ViewData["Attachments"] = await (
                    from a in _context.DpAttachments
                    where a.DataProjectId == id
                    orderby a.Rank ascending
                    select new AttachmentLinks
                    {
                        Name = a.AttachmentName,
                        Src = "/data/File?id=" + a.AttachmentId,
                        Id = a.AttachmentId,
                        Size = a.AttachmentSize,
                        Type = a.AttachmentType
                    }
                ).ToListAsync();

                ViewData["RelatedTerms"] = await (
                    from r in _context.DpTermAnnotations
                    where r.DataProjectId == id
                    join q in (
                        from f in _context.UserFavorites
                        where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                        select new { f.ItemId }
                    )
                        on r.TermId equals q.ItemId
                        into tmp
                    from tfi in tmp.DefaultIfEmpty()
                    orderby r.Rank ,r.Term.Name
                    select new RelatedItemData
                    {
                        Id = r.TermAnnotationId,
                        CollectionId = r.DataProjectId,
                        ItemId = r.TermId,
                        Name = r.Term.Name,
                        Rank = r.Rank,
                        Annotation = r.Annotation ?? r.Term.Summary,
                        Favorite = tfi.ItemId == null ? "no" : "yes"
                    }
                ).ToListAsync();

                ViewData["RelatedReports"] = await (
                    from r in _context.DpReportAnnotations
                    where r.DataProjectId == id
                    join q in (
                        from f in _context.UserFavorites
                        where f.ItemType.ToLower() == "report" && f.UserId == MyUser.UserId
                        select new { f.ItemId }
                    )
                        on r.ReportId equals q.ItemId
                        into tmp
                    from rfi in tmp.DefaultIfEmpty()
                    orderby r.Report.ReportObjectType.Name ,r.Rank ,r.Report.Name
                    select new RelatedItemData
                    {
                        Id = r.ReportAnnotationId,
                        CollectionId = r.DataProjectId,
                        ItemId = r.ReportId,
                        Name = r.Report.DisplayName,
                        Rank = r.Rank,
                        ReportType = r.Report.ReportObjectType.Name,
                        CertTag = r.Report.CertificationTag,
                        EpicMasterFile = r.Report.EpicMasterFile,
                        Annotation =
                            r.Report.ReportObjectDoc.DeveloperDescription == null
                                ? r.Report.Description
                                : r.Report.ReportObjectDoc.DeveloperDescription,
                        Image = r.Report.ReportObjectImagesDocs.Any()
                          ? "/data/img?id=" + r.Report.ReportObjectImagesDocs.First().ImageId
                          : "",
                        Favorite = rfi.ItemId == null ? "no" : "yes",
                        RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            r.Report,
                            _context,
                            User.Identity.Name
                        ),
                        ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            r.Report.ReportObjectType.Name,
                            r.Report.ReportServerPath,
                            r.Report.SourceServer
                        ),
                        EditReportUrl = HtmlHelpers.EditReportFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            r.Report.ReportServerPath,
                            r.Report.SourceServer,
                            r.Report.EpicMasterFile,
                            r.Report.EpicReportTemplateId.ToString(),
                            r.Report.EpicRecordId.ToString()
                        ),
                    }
                ).ToListAsync();

                if (Collection != null)
                {
                    return Page();
                }
            }

            AllCollections = await (
                from i in _context.DpDataProjects
                orderby i.LastUpdateDate descending
                select new AllCollectionsData
                {
                    Id = i.DataProjectId,
                    Name = i.Name,
                    Description = string.IsNullOrEmpty(i.Description) ? i.Purpose : i.Description,
                    Favorite = (
                        from f in _context.UserFavorites
                        where
                            f.ItemType == "collection"
                            && f.UserId == MyUser.UserId
                            && f.ItemId == i.DataProjectId
                        select new { f.ItemId }
                    ).Any()
                      ? "yes"
                      : "no"
                }
            ).ToListAsync();
            return Page();
        }

        public async Task<ActionResult> OnGetComments(int id)
        {
            ViewData["Comments"] = await (
                from c in _context.DpDataProjectConversationMessages
                where c.DataProjectConversation.DataProjectId == id
                orderby c.DataProjectConversationId descending,c.DataProjectConversationMessageId ascending
                select new CollectionCommentsData
                {
                    CollectionId = id,
                    Date = c.PostDateTimeDisplayString,
                    ConvId = c.DataProjectConversationId,
                    MessId = c.DataProjectConversationMessageId,
                    User = c.User.Fullname_Cust,
                    Text = c.MessageText
                }
            ).ToListAsync();
            ViewData["Id"] = id;
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(
                _cache,
                _context,
                User.Identity.Name
            );
            //s//return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Details/_Comments", ViewData = ViewData };
        }

        // new project
        public ActionResult OnPostNewCollection()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                26
            );
            if (!ModelState.IsValid || DpDataProject.Name is null || !checkpoint)
            {
                return RedirectToPage("/Collections/Index");
            }

            DpDataProject.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            DpDataProject.LastUpdateDate = DateTime.Now;
            _context.DpDataProjects.Add(DpDataProject);
            _context.SaveChanges();

            return RedirectToPage("/Collections/Index", new { id = DpDataProject.DataProjectId });
        }

        public ActionResult OnGetDeleteCollection(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                27
            );
            if (!checkpoint)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id = DpDataProject.DataProjectId }
                );
            }

            // delete report annotations, term annotations and agreements
            // then delete project and save.
            _context.RemoveRange(
                _context.DpDataProjectConversationMessages.Where(
                    x => x.DataProjectConversation.DataProjectId == Id
                )
            );
            _context.SaveChanges();
            _context.RemoveRange(
                _context.DpDataProjectConversations.Where(x => x.DataProjectId == Id)
            );
            _context.SaveChanges();
            _context.RemoveRange(_context.DpReportAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpTermAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(
                _context.DpAgreementUsers.Where(x => x.Agreement.DataProjectId == Id)
            );
            _context.SaveChanges();
            _context.RemoveRange(_context.DpAgreements.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.Remove(
                _context.DpDataProjects.Where(m => m.DataProjectId == Id).FirstOrDefault()
            );
            _context.SaveChanges();

            return RedirectToPage("/Collections/Index");
        }

        // edit projects
        public ActionResult OnPostEditCollection()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (!ModelState.IsValid || !checkpoint)
            {
                return RedirectToPage(
                    "/Collections/Index",
                    new { id = DpDataProject.DataProjectId }
                );
            }

            // we get a copy of the project and then will only update several fields.
            DpDataProject NewDpDataProject = _context.DpDataProjects
                .Where(m => m.DataProjectId == DpDataProject.DataProjectId)
                .FirstOrDefault();

            // update last update values & update remaining fields
            NewDpDataProject.LastUpdateUser =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewDpDataProject.LastUpdateDate = DateTime.Now;
            NewDpDataProject.Name = DpDataProject.Name;
            NewDpDataProject.Description = DpDataProject.Description;
            NewDpDataProject.OperationOwnerId = DpDataProject.OperationOwnerId;
            NewDpDataProject.ExecutiveOwnerId = DpDataProject.ExecutiveOwnerId;
            NewDpDataProject.FinancialImpact = DpDataProject.FinancialImpact;
            NewDpDataProject.StrategicImportance = DpDataProject.StrategicImportance;
            NewDpDataProject.Purpose = DpDataProject.Purpose;
            NewDpDataProject.AnalyticsOwnerId = DpDataProject.AnalyticsOwnerId;
            NewDpDataProject.DataManagerId = DpDataProject.DataManagerId;
            NewDpDataProject.Hidden = DpDataProject.Hidden;

            // save updates
            _context.Attach(NewDpDataProject).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToPage("/Collections/Index", new { id = DpDataProject.DataProjectId });
        }

        public async Task<ActionResult> OnPostAddLinkedReport()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (
                ModelState.IsValid
                && DpReportAnnotation.DataProjectId > 0
                && DpReportAnnotation.ReportId > 0
                && checkpoint
            )
            {
                if (
                    !_context.DpReportAnnotations.Any(
                        x =>
                            x.ReportId == DpReportAnnotation.ReportId
                            && x.DataProjectId == DpReportAnnotation.DataProjectId
                    )
                )
                {
                    _context.Add(DpReportAnnotation);

                    // update last update date on report.
                    _context.DpDataProjects
                        .Where(d => d.DataProjectId == DpReportAnnotation.DataProjectId)
                        .FirstOrDefault().LastUpdateDate = DateTime.Now;
                    _context.DpDataProjects
                        .Where(d => d.DataProjectId == DpReportAnnotation.DataProjectId)
                        .FirstOrDefault().LastUpdateUser =
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                    await _context.SaveChangesAsync();
                }
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedReports"] = await (
                from r in _context.DpReportAnnotations
                where r.DataProjectId == DpReportAnnotation.DataProjectId
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "report" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.ReportId equals q.ItemId
                    into tmp
                from rfi in tmp.DefaultIfEmpty()
                orderby r.Report.ReportObjectType.Name ,r.Rank ,r.Report.Name
                select new RelatedItemData
                {
                    Id = r.ReportAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.ReportId,
                    Name = r.Report.DisplayName,
                    Rank = r.Rank,
                    ReportType = r.Report.ReportObjectType.Name,
                    CertTag = r.Report.CertificationTag,
                    EpicMasterFile = r.Report.EpicMasterFile,
                    Annotation = r.Report.ReportObjectDoc.DeveloperDescription,
                    Image = r.Report.ReportObjectImagesDocs.Any()
                      ? "/data/img?id=" + r.Report.ReportObjectImagesDocs.First().ImageId
                      : "",
                    Favorite = rfi.ItemId == null ? "no" : "yes",
                    RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report,
                        _context,
                        User.Identity.Name
                    ),
                    ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportObjectType.Name,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer
                    ),
                    EditReportUrl = HtmlHelpers.EditReportFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer,
                        r.Report.EpicMasterFile,
                        r.Report.EpicReportTemplateId.ToString(),
                        r.Report.EpicRecordId.ToString()
                    ),
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Edit/_CurrentReports",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnPostEditLinkedReport()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (ModelState.IsValid && DpReportAnnotation.ReportAnnotationId > 0 && checkpoint)
            {
                var q = _context.DpReportAnnotations
                    .Where(x => x.ReportAnnotationId == DpReportAnnotation.ReportAnnotationId)
                    .FirstOrDefault();
                q.Annotation = DpReportAnnotation.Annotation;
                q.Rank = DpReportAnnotation.Rank;
                await _context.SaveChangesAsync();
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedReports"] = await (
                from r in _context.DpReportAnnotations
                where r.DataProjectId == DpReportAnnotation.DataProjectId
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "report" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.ReportId equals q.ItemId
                    into tmp
                from rfi in tmp.DefaultIfEmpty()
                orderby r.Report.ReportObjectType.Name ,r.Rank ,r.Report.Name
                select new RelatedItemData
                {
                    Id = r.ReportAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.ReportId,
                    Name = r.Report.DisplayName,
                    Rank = r.Rank,
                    ReportType = r.Report.ReportObjectType.Name,
                    CertTag = r.Report.CertificationTag,
                    EpicMasterFile = r.Report.EpicMasterFile,
                    Annotation = r.Report.ReportObjectDoc.DeveloperDescription,
                    Image = r.Report.ReportObjectImagesDocs.Any()
                      ? "/data/img?id=" + r.Report.ReportObjectImagesDocs.First().ImageId
                      : "",
                    Favorite = rfi.ItemId == null ? "no" : "yes",
                    RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report,
                        _context,
                        User.Identity.Name
                    ),
                    ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportObjectType.Name,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer
                    ),
                    EditReportUrl = HtmlHelpers.EditReportFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer,
                        r.Report.EpicMasterFile,
                        r.Report.EpicReportTemplateId.ToString(),
                        r.Report.EpicRecordId.ToString()
                    ),
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Edit/_CurrentReports",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnGetDeleteLinkedReport(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            var ta =
                _context.DpReportAnnotations
                    .Where(x => x.ReportAnnotationId == id)
                    .FirstOrDefault().DataProjectId;
            if (checkpoint)
            {
                _context.DpReportAnnotations.Remove(
                    _context.DpReportAnnotations
                        .Where(x => x.ReportAnnotationId == id)
                        .FirstOrDefault()
                );
                await _context.SaveChangesAsync();
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedReports"] = await (
                from r in _context.DpReportAnnotations
                where r.DataProjectId == ta
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "report" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.ReportId equals q.ItemId
                    into tmp
                from rfi in tmp.DefaultIfEmpty()
                orderby r.Report.ReportObjectType.Name ,r.Rank ,r.Report.Name
                select new RelatedItemData
                {
                    Id = r.ReportAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.ReportId,
                    Name = r.Report.DisplayName,
                    Rank = r.Rank,
                    ReportType = r.Report.ReportObjectType.Name,
                    CertTag = r.Report.CertificationTag,
                    EpicMasterFile = r.Report.EpicMasterFile,
                    Annotation = r.Report.ReportObjectDoc.DeveloperDescription,
                    Image = r.Report.ReportObjectImagesDocs.Any()
                      ? "/data/img?id=" + r.Report.ReportObjectImagesDocs.First().ImageId
                      : "",
                    Favorite = rfi.ItemId == null ? "no" : "yes",
                    RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report,
                        _context,
                        User.Identity.Name
                    ),
                    ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportObjectType.Name,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer
                    ),
                    EditReportUrl = HtmlHelpers.EditReportFromParams(
                        _config["AppSettings:org_domain"],
                        HttpContext,
                        r.Report.ReportServerPath,
                        r.Report.SourceServer,
                        r.Report.EpicMasterFile,
                        r.Report.EpicReportTemplateId.ToString(),
                        r.Report.EpicRecordId.ToString()
                    ),
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Edit/_CurrentReports",
                ViewData = ViewData
            };
        }

        public async Task<ActionResult> OnPostAddLinkedTerm()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );

            if (
                ModelState.IsValid
                && DpTermAnnotation.DataProjectId > 0
                && DpTermAnnotation.TermId > 0
                && checkpoint
            )
            {
                // only add link if not existing.
                if (
                    !_context.DpTermAnnotations.Any(
                        x =>
                            x.TermId == DpTermAnnotation.TermId
                            && x.DataProjectId == DpTermAnnotation.DataProjectId
                    )
                )
                {
                    _context.Add(DpTermAnnotation);

                    // update last update date on report.
                    _context.DpDataProjects
                        .Where(d => d.DataProjectId == DpTermAnnotation.DataProjectId)
                        .FirstOrDefault().LastUpdateDate = DateTime.Now;
                    _context.DpDataProjects
                        .Where(d => d.DataProjectId == DpTermAnnotation.DataProjectId)
                        .FirstOrDefault().LastUpdateUser =
                        UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                    await _context.SaveChangesAsync();
                }
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedTerms"] = await (
                from r in _context.DpTermAnnotations
                where r.DataProjectId == DpTermAnnotation.DataProjectId
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.TermId equals q.ItemId
                    into tmp
                from tfi in tmp.DefaultIfEmpty()
                orderby r.Rank ,r.Term.Name
                select new RelatedItemData
                {
                    Id = r.TermAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.TermId,
                    Name = r.Term.Name,
                    Rank = r.Rank,
                    Annotation = r.Annotation ?? r.Term.Summary,
                    Favorite = tfi.ItemId == null ? "no" : "yes"
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Edit/_CurrentTerms", ViewData = ViewData };
        }

        public async Task<ActionResult> OnPostEditLinkedTerm()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (ModelState.IsValid && DpTermAnnotation.TermAnnotationId > 0 && checkpoint)
            {
                var q = _context.DpTermAnnotations
                    .Where(x => x.TermAnnotationId == DpTermAnnotation.TermAnnotationId)
                    .FirstOrDefault();
                q.Annotation = DpTermAnnotation.Annotation;
                q.Rank = DpTermAnnotation.Rank;
                await _context.SaveChangesAsync();
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedTerms"] = await (
                from r in _context.DpTermAnnotations
                where r.DataProjectId == DpTermAnnotation.DataProjectId
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.TermId equals q.ItemId
                    into tmp
                from tfi in tmp.DefaultIfEmpty()
                orderby r.Rank ,r.Term.Name
                select new RelatedItemData
                {
                    Id = r.TermAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.TermId,
                    Name = r.Term.Name,
                    Rank = r.Rank,
                    Annotation = r.Annotation ?? r.Term.Summary,
                    Favorite = tfi.ItemId == null ? "no" : "yes"
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Edit/_CurrentTerms", ViewData = ViewData };
        }

        public async Task<ActionResult> OnGetDeleteLinkedTerm(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            var ta =
                _context.DpTermAnnotations
                    .Where(x => x.TermAnnotationId == id)
                    .FirstOrDefault().DataProjectId;
            if (checkpoint)
            {
                _context.DpTermAnnotations.Remove(
                    _context.DpTermAnnotations.Where(x => x.TermAnnotationId == id).FirstOrDefault()
                );
                await _context.SaveChangesAsync();
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            ViewData["RelatedTerms"] = await (
                from r in _context.DpTermAnnotations
                where r.DataProjectId == ta
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on r.TermId equals q.ItemId
                    into tmp
                from tfi in tmp.DefaultIfEmpty()
                orderby r.Rank ,r.Term.Name
                select new RelatedItemData
                {
                    Id = r.TermAnnotationId,
                    CollectionId = r.DataProjectId,
                    ItemId = r.TermId,
                    Name = r.Term.Name,
                    Rank = r.Rank,
                    Annotation = r.Annotation ?? r.Term.Summary,
                    Favorite = tfi.ItemId == null ? "no" : "yes"
                }
            ).ToListAsync();
            //return Partial((".+?"));
            return new PartialViewResult() { ViewName = "Edit/_CurrentTerms", ViewData = ViewData };
        }

        public ActionResult OnPostAddAgreement()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (ModelState.IsValid && checkpoint)
            {
                // last updated
                MyDpAgreement.LastUpdateUserNavigation = UserHelpers.GetUser(
                    _cache,
                    _context,
                    User.Identity.Name
                );
                MyDpAgreement.LastUpdateDate = DateTime.Now;
                _context.Add(MyDpAgreement);

                // save any linked users
                _context.AddRange(
                    DpAgreementUsers.Select(
                        MyUser =>
                            new DpAgreementUser
                            {
                                UserId = MyUser,
                                AgreementId = MyDpAgreement.AgreementId
                            }
                    )
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Collections/Index", new { id = MyDpAgreement.DataProjectId });
        }

        public ActionResult OnPostRemoveAgreement(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (checkpoint)
            {
                // remove any linked users and remove agreement.
                var ag =
                    _context.DpAgreements
                        .Where(x => x.AgreementId == Id)
                        .FirstOrDefault().DataProjectId;
                _context.DpAgreementUsers.RemoveRange(
                    _context.DpAgreementUsers.Where(x => x.AgreementId == Id)
                );
                _context.Remove(
                    _context.DpAgreements.Where(x => x.AgreementId == Id).FirstOrDefault()
                );
                _context.SaveChanges();
                return RedirectToPage("/Collections/Index", new { id = Id });
            }

            return RedirectToPage("/Collections/Index");
        }

        public ActionResult OnPostEditAgreement()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                28
            );
            if (ModelState.IsValid && checkpoint)
            {
                var q = MyDpAgreement;
                DpAgreement EditedDpAgreement = _context.DpAgreements
                    .Where(
                        x =>
                            x.AgreementId == MyDpAgreement.AgreementId
                            && x.DataProjectId == MyDpAgreement.DataProjectId
                    )
                    .FirstOrDefault();
                // last updated
                EditedDpAgreement.LastUpdateUser =
                    UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                EditedDpAgreement.LastUpdateDate = DateTime.Now;
                EditedDpAgreement.Description = MyDpAgreement.Description;
                EditedDpAgreement.MeetingDate = MyDpAgreement.MeetingDate;
                EditedDpAgreement.EffectiveDate = MyDpAgreement.EffectiveDate;
                EditedDpAgreement.Rank = MyDpAgreement.Rank;

                _context.Attach(EditedDpAgreement).State = EntityState.Modified;
                _context.SaveChanges();

                // save any linked users
                _context.RemoveRange(
                    _context.DpAgreementUsers.Where(
                        x => x.AgreementId.Equals(MyDpAgreement.AgreementId)
                    )
                );
                _context.AddRange(
                    DpAgreementUsers.Select(
                        MyUser =>
                            new DpAgreementUser
                            {
                                UserId = MyUser,
                                AgreementId = MyDpAgreement.AgreementId
                            }
                    )
                );
                _context.SaveChanges();
            }

            return RedirectToPage("/Collections/Index", new { id = MyDpAgreement.DataProjectId });
        }

        public async Task<ActionResult> OnPostNewComment()
        {
            if (!ModelState.IsValid || NewCommentReply.MessageText is null)
            {
                return RedirectToPage("/Collections/Index", new { id = NewComment.DataProjectId });
            }

            // if missing the conversation ID then create a new conversation.
            if (
                !_context.DpDataProjectConversationMessages.Any(
                    x => x.DataProjectConversationId == NewComment.DataProjectConversationId
                )
            )
            {
                // create new comment
                _context.Add(NewComment);
            }
            // add message
            NewCommentReply.UserId =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewCommentReply.PostDateTime = System.DateTime.Now;
            NewCommentReply.DataProjectConversationId = NewComment.DataProjectConversationId;
            _context.Add(NewCommentReply);
            _context.SaveChanges();

            return await OnGetComments(NewComment.DataProjectId);
        }

        public async Task<ActionResult> OnPostDeleteComment()
        {
            if (ModelState.IsValid)
            {
                _context.Remove(NewCommentReply);
                _context.SaveChanges();
            }

            return await OnGetComments(NewComment.DataProjectId);
        }

        public async Task<ActionResult> OnPostDeleteCommentStream()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                25
            );
            if (ModelState.IsValid && checkpoint)
            {
                // first delete all replys on comment. Delete comment.
                _context.RemoveRange(
                    _context.DpDataProjectConversationMessages.Where(
                        x => x.DataProjectConversationId == NewComment.DataProjectConversationId
                    )
                );
                _context.Remove(NewComment);
                _context.SaveChanges();
            }

            return await OnGetComments(NewComment.DataProjectId);
        }

        public bool ValidateFileUpload(IFormFile file)
        {
            try
            {
                // get the filename in case we need to return info to the user

                if (file == null)
                {
                    ModelState.AddModelError(
                        file.Name,
                        "The file was not received by the server. If this issue persists please contact Analytics."
                    );
                    return false;
                }
                else if (file.Length > 1024 * 1024)
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

        public ActionResult OnPostAddFile(int Id, IFormFile File)
        {
            if (ValidateFileUpload(File))
            {
                //ReportObjectImagesDoc img;
                var MyFile = new DpAttachment { DataProjectId = Id };
                using (var stream = new MemoryStream())
                {
                    var g = File;
                    File.CopyTo(stream);
                    MyFile.AttachmentData = stream.ToArray();
                    MyFile.AttachmentName = File.FileName;
                    MyFile.AttachmentType = File.ContentType;
                    MyFile.AttachmentSize = unchecked((int)File.Length); //bytes;
                }
                _context.Add(MyFile);
                _context.SaveChanges();
            }
            return RedirectToPage("/Collections/Index", new { id = Id });
        }

        public ActionResult OnPostRemoveFile(int Id, int FileId)
        {
            _context.Remove(
                _context.DpAttachments.Where(t => t.AttachmentId == FileId).FirstOrDefault()
            );
            _context.SaveChanges();
            return RedirectToPage("/Collections/Index", new { id = Id });
        }
    }
}
