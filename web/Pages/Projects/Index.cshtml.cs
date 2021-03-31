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


namespace Atlas_Web.Pages.Projects
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

        [BindProperty] public DpDataProject DpDataProject { get; set; }
        [BindProperty] public DpReportAnnotation DpReportAnnotation { get; set; }
        [BindProperty] public DpTermAnnotation DpTermAnnotation { get; set; }
        [BindProperty] public DpAgreement MyDpAgreement { get; set; }
        [BindProperty] public int[] DpAgreementUsers { get; set; }
        [BindProperty] public DpMilestoneTask DpMilestone { get; set; }
        [BindProperty] public DpMilestoneTasksCompleted MilestoneCompleteTask { get; set; }
        [BindProperty] public DpDataProjectConversation NewComment { get; set; }
        [BindProperty] public DpDataProjectConversationMessage NewCommentReply { get; set; }
        [BindProperty] public string DpChecklist { get; set; }
        [BindProperty] public DpMilestoneChecklistCompleted CompletedChecklist { get; set; }
        [BindProperty] public int?[] CompletedChecklistItems { get; set; }

        public List<int?> Permissions { get; set; }
        public IEnumerable<AllProjectsData> AllProjects { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public IEnumerable<ProjectCommentsData> ProjectComments { get; set; }
        public DataProjectData DataProject { get; set; }
        public List<AdList> AdLists { get; set; }

        public class AllProjectsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Favorite { get; set; }
        }

        public class ProjectCommentsData
        {
            public string Date { get; set; }
            public int? ConvId { get; set; }
            public int? MessId { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
            public int ProjectId { get; set; }
        }

        public class DataProjectData
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
            public int? StrgImportId { get; set; }
            public string UpdatedBy { get; set; }
            public string UpdateDate { get; set; }
            public IEnumerable<RelatedItemData> RelatedReports { get; set; }
            public IEnumerable<RelatedItemData> RelatedTerms { get; set; }
            public IEnumerable<AgreementsData> Agreements { get; set; }
            public string Favorite { get; set; }
            public IEnumerable<MilestoneTasksData> MilestoneTasks { get; set; }
            public IEnumerable<ChecklistCompletedData> ChecklistCompleted { get; set; }
            public IEnumerable<MilestoneTasksCompletedData> MilestoneTasksCompleted { get; set; }
        }

        public class RelatedItemData
        {
            public int? Id { get; set; }
            public int? ItemId { get; set; }
            public string Name { get; set; }
            public int? Rank { get; set; }
            public string Annotation { get; set; }
            public string Favorite { get; set; }
            public string RunReportUrl { get; set; }
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

        public class ChecklistCompletedData
        {
            public int? Id { get; set; }
            public int? TaskId { get; set; }
            public DateTime? Date { get; set; }
            public int? ProjectId { get; set; }
            public int? ChecklistId { get; set; }
            public string CompletedBy { get; set; }
            public string CompletionDate { get; set; }
        }

        public class ChecklistData
        {
            public int? Id { get; set; }
            public string Item { get; set; }
        }

        public class AttachmentLinks
        {
            public string Name { get; set; }
            public string Src { get; set; }
            public int Id { get; set; }
            public int? Size { get; set; }
            public string Type { get; set; }
        }
        public class MilestoneTasksData
        {
            public int? Id { get; set; }
            public string Description { get; set; }
            public string Template { get; set; }
            public int? TemplateId { get; set; }
            public DateTime? StartDate { get; set; }
            public string StartDateInput { get; set; }
            public string StartDateString { get; set; }
            public DateTime? EndDate { get; set; }
            public string EndDateString { get; set; }
            public string EndDateInput { get; set; }
            public string Owner { get; set; }
            public int? OwnerId { get; set; }
            public int? Interval { get; set; }
            public string Frequency { get; set; }
            public IEnumerable<ChecklistData> Checklist { get; set; }
        }

        public class MilestoneTasksCompletedData
        {
            public int Id { get; set; }
            public string DueDate { get; set; }
            public string CompletionDate { get; set; }
            public string CompletedBy { get; set; }
            public string Comments { get; set; }
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
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentProjects", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;

            // if the id null then list all
            if (id != null)
            {
                DataProject = await (from d in _context.DpDataProjects
                                     where d.DataProjectId == id
                                     select new DataProjectData
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
                                         UpdateDate = d.LastUpdatedDateDisplayString,
                                         UpdatedBy = d.LastUpdateUserNavigation.Fullname_Cust,
                                         RelatedReports = (from r in d.DpReportAnnotations
                                                           join q in (from f in _context.UserFavorites
                                                                      where f.ItemType.ToLower() == "report"
                                                                         && f.UserId == MyUser.UserId
                                                                      select new { f.ItemId })
                                                           on r.ReportId equals q.ItemId into tmp
                                                           from rfi in tmp.DefaultIfEmpty()
                                                           orderby r.Rank, r.Report.Name
                                                           select new RelatedItemData
                                                           {
                                                               Id = r.ReportAnnotationId,
                                                               ItemId = r.ReportId,
                                                               Name = r.Report.DisplayName,
                                                               Rank = r.Rank,
                                                               Annotation = string.IsNullOrEmpty(r.Annotation) ? r.Report.ReportObjectDoc.DeveloperDescription : r.Annotation,
                                                               Favorite = rfi.ItemId == null ? "no" : "yes",
                                                               RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, r.Report.ReportObjectUrl, r.Report.Name, r.Report.ReportObjectType.Name, r.Report.EpicReportTemplateId.ToString(), r.Report.EpicRecordId.ToString(), r.Report.EpicMasterFile, r.Report.ReportObjectDoc.EnabledForHyperspace),
                                                               ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(_config["AppSettings:org_domain"], HttpContext, r.Report.ReportObjectType.Name, r.Report.ReportServerPath, r.Report.SourceServer),
                                                               EditReportUrl = HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, r.Report.ReportServerPath, r.Report.SourceServer, r.Report.EpicMasterFile, r.Report.EpicReportTemplateId.ToString(), r.Report.EpicRecordId.ToString()),
                                                           }).ToList(),
                                         RelatedTerms = (from r in d.DpTermAnnotations
                                                         join q in (from f in _context.UserFavorites
                                                                    where f.ItemType.ToLower() == "term"
                                                                       && f.UserId == MyUser.UserId
                                                                    select new { f.ItemId })
                                                           on r.TermId equals q.ItemId into tmp
                                                         from tfi in tmp.DefaultIfEmpty()
                                                         orderby r.Rank, r.Term.Name
                                                         select new RelatedItemData
                                                         {
                                                             Id = r.TermAnnotationId,
                                                             ItemId = r.TermId,
                                                             Name = r.Term.Name,
                                                             Rank = r.Rank,
                                                             Annotation = r.Annotation ?? r.Term.Summary,
                                                             Favorite = tfi.ItemId == null ? "no" : "yes"
                                                         }).ToList(),

                                         Agreements = (from r in d.DpAgreements
                                                       orderby r.Rank, r.EffectiveDate
                                                       select new AgreementsData
                                                       {
                                                           Id = r.AgreementId,
                                                           Description = r.Description,
                                                           Rank = r.Rank,
                                                           MeetingDate = r.MeetingDate,
                                                           MeetingDateString = r.MeetingDateDisplayString,
                                                           EffectiveDate = r.EffectiveDate,
                                                           EffectiveDateString = r.EffectiveDateDisplayString,
                                                           Users = from u in r.DpAgreementUsers
                                                                   select new AgreementUsersData
                                                                   {
                                                                       Id = u.UserId,
                                                                       Name = u.User.Fullname_Cust
                                                                   }
                                                       }).ToList(),
                                         Favorite = (from f in _context.UserFavorites
                                                     where f.ItemType == "project"
                                                        && f.UserId == MyUser.UserId
                                                        && f.ItemId == d.DataProjectId
                                                     select new { f.ItemId }).Any() ? "yes" : "no",
                                         MilestoneTasks = (from mt in d.DpMilestoneTasks
                                                           select new MilestoneTasksData
                                                           {
                                                               Id = mt.MilestoneTaskId,
                                                               Description = mt.Description,
                                                               Template = mt.MilestoneTemplate.Name,
                                                               Frequency = mt.MilestoneTemplate.MilestoneType.Name,
                                                               TemplateId = mt.MilestoneTemplateId,
                                                               StartDateString = mt.StartDatePretty,
                                                               StartDateInput = mt.StartDate.HasValue ? ((DateTime)mt.StartDate).ToString("yyyy-MM-dd") : "",
                                                               EndDateInput = mt.EndDate.HasValue ? ((DateTime)mt.EndDate).ToString("yyyy-MM-dd") : "",
                                                               EndDateString = mt.EndDatePretty,
                                                               Owner = mt.Owner.Fullname_Cust,
                                                               OwnerId = mt.OwnerId,
                                                               Interval = mt.MilestoneTemplate.Interval,
                                                               Checklist = (from c in mt.DpMilestoneChecklists
                                                                            select new ChecklistData
                                                                            {
                                                                                Item = c.Item,
                                                                                Id = c.MilestoneChecklistId
                                                                            }).ToList()
                                                           }).ToList(),
                                         ChecklistCompleted = (from cc in d.DpMilestoneChecklistCompleteds
                                                               select new ChecklistCompletedData
                                                               {
                                                                   Id = cc.MilestoneChecklistCompletedId,
                                                                   TaskId = cc.TaskId,
                                                                   Date = cc.TaskDate,
                                                                   ProjectId = cc.DataProjectId,
                                                                   ChecklistId = cc.MilestoneChecklistId,
                                                                   CompletedBy = cc.CompletionUserNavigation.Fullname_Cust,
                                                                   CompletionDate = cc.CompletionDatePretty
                                                               }).ToList(),
                                         MilestoneTasksCompleted = (from r in d.DpMilestoneTasksCompleteds
                                                                    select new MilestoneTasksCompletedData
                                                                    {
                                                                        Id = r.MilestoneTaskCompletedId,
                                                                        DueDate = r.DueDatePretty,
                                                                        CompletionDate = r.CompletionDatePretty,
                                                                        CompletedBy = r.CompletionUser,
                                                                        Comments = r.Comments
                                                                    }).ToList()
                                     }).FirstOrDefaultAsync();

                ViewData["Attachments"] = await (from a in _context.DpAttachments
                                                 where a.DataProjectId == id
                                                 orderby a.Rank ascending
                                                 select new AttachmentLinks
                                                 {
                                                     Name = a.AttachmentName,
                                                     Src = "data/File?id=" + a.AttachmentId,
                                                     Id = a.AttachmentId,
                                                     Size = a.AttachmentSize,
                                                     Type = a.AttachmentType
                                                 }).ToListAsync();

                if (DataProject != null)
                {
                    return Page();
                }
            }

            AllProjects = await (from i in _context.DpDataProjects
                                 orderby i.LastUpdateDate descending
                                 select new AllProjectsData
                                 {
                                     Id = i.DataProjectId,
                                     Name = i.Name,
                                     Description = string.IsNullOrEmpty(i.Description) ? i.Purpose : i.Description,
                                     Favorite = (from f in _context.UserFavorites
                                                 where f.ItemType == "project"
                                                    && f.UserId == MyUser.UserId
                                                    && f.ItemId == i.DataProjectId
                                                 select new { f.ItemId }).Any() ? "yes" : "no"
                                 }).ToListAsync();
            return Page();
        }

        public async Task<ActionResult> OnGetComments(int id)
        {
            ViewData["Comments"] = await (from c in _context.DpDataProjectConversationMessages
                                          where c.DataProjectConversation.DataProjectId == id
                                          orderby c.DataProjectConversationId descending, c.DataProjectConversationMessageId ascending
                                          select new ProjectCommentsData
                                          {
                                              ProjectId = id,
                                              Date = c.PostDateTimeDisplayString,
                                              ConvId = c.DataProjectConversationId,
                                              MessId = c.DataProjectConversationMessageId,
                                              User = c.User.Fullname_Cust,
                                              Text = c.MessageText
                                          }).ToListAsync();
            ViewData["Id"] = id;
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            //s//return Partial((".+?"));
            return new PartialViewResult()
            {
                ViewName = "Details/_Comments",
                ViewData = ViewData
            };
        }

        // new project
        public ActionResult OnPostNewDataProject()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 26);
            if (!ModelState.IsValid || DpDataProject.Name is null || !checkpoint)
            {
                return RedirectToPage("/Projects/Index");
            }

            DpDataProject.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            DpDataProject.LastUpdateDate = DateTime.Now;
            _context.DpDataProjects.Add(DpDataProject);
            _context.SaveChanges();

            return RedirectToPage("/Projects/Index", new { id = DpDataProject.DataProjectId });
        }

        public ActionResult OnGetDeleteProject(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 27);
            if (!checkpoint)
            {
                return RedirectToPage("/Projects/Index", new { id = DpDataProject.DataProjectId });
            }

            // delete report annotations, term annotations and agreements
            // then delete project and save.
            _context.RemoveRange(_context.DpDataProjectConversationMessages.Where(x => x.DataProjectConversation.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpDataProjectConversations.Where(x => x.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpReportAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpTermAnnotations.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpAgreementUsers.Where(x => x.Agreement.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpAgreements.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpMilestoneChecklists.Where(m => m.MilestoneTask.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpMilestoneTasks.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpMilestoneTasksCompleteds.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.RemoveRange(_context.DpMilestoneChecklistCompleteds.Where(m => m.DataProjectId == Id));
            _context.SaveChanges();
            _context.Remove(_context.DpDataProjects.Where(m => m.DataProjectId == Id).FirstOrDefault());
            _context.SaveChanges();

            return RedirectToPage("/Projects/Index");
        }

        // edit projects
        public ActionResult OnPostEditDataProject()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (!ModelState.IsValid || !checkpoint)
            {
                return RedirectToPage("/Projects/Index", new { id = DpDataProject.DataProjectId });
            }

            // we get a copy of the project and then will only update several fields.
            DpDataProject NewDpDataProject = _context.DpDataProjects.Where(m => m.DataProjectId == DpDataProject.DataProjectId).FirstOrDefault();

            // update last update values & update remaining fields
            NewDpDataProject.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
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

            // save updates
            _context.Attach(NewDpDataProject).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToPage("/Projects/Index", new { id = DpDataProject.DataProjectId });
        }

        public ActionResult OnPostAddLinkedReport()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && DpReportAnnotation.DataProjectId > 0 && DpReportAnnotation.ReportId > 0 && checkpoint)
            {
                _context.DpReportAnnotations.Add(DpReportAnnotation);
                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = DpReportAnnotation.DataProjectId });
        }

        public ActionResult OnPostEditLinkedReport()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && DpReportAnnotation.ReportAnnotationId > 0 && checkpoint)
            {
                var q = _context.DpReportAnnotations.Where(x => x.ReportAnnotationId == DpReportAnnotation.ReportAnnotationId).FirstOrDefault();
                q.Annotation = DpReportAnnotation.Annotation;
                q.Rank = DpReportAnnotation.Rank;

                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = DpReportAnnotation.DataProjectId });
        }

        public ActionResult OnGetDeleteLinkedReport(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (checkpoint)
            {
                var ra = _context.DpReportAnnotations.Where(x => x.ReportAnnotationId == id).FirstOrDefault().DataProjectId;
                _context.DpReportAnnotations.Remove(_context.DpReportAnnotations.Where(x => x.ReportAnnotationId == id).FirstOrDefault());
                _context.SaveChanges();
                return RedirectToPage("/Projects/Index", new { id = ra });
            }

            return RedirectToPage("/Projects/Index");
        }

        public ActionResult OnPostAddLinkedTerm()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && DpTermAnnotation.DataProjectId > 0 && DpTermAnnotation.TermId > 0 && checkpoint)
            {
                _context.DpTermAnnotations.Add(DpTermAnnotation);
                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = DpTermAnnotation.DataProjectId });
        }

        public ActionResult OnPostEditLinkedTerm()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && DpTermAnnotation.TermAnnotationId > 0 && checkpoint)
            {
                var q = _context.DpTermAnnotations.Where(x => x.TermAnnotationId == DpTermAnnotation.TermAnnotationId).FirstOrDefault();
                q.Annotation = DpTermAnnotation.Annotation;
                q.Rank = DpTermAnnotation.Rank;
                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = DpTermAnnotation.DataProjectId });
        }

        public ActionResult OnGetDeleteLinkedTerm(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (checkpoint)
            {
                var ta = _context.DpTermAnnotations.Where(x => x.TermAnnotationId == id).FirstOrDefault().DataProjectId;
                _context.DpTermAnnotations.Remove(_context.DpTermAnnotations.Where(x => x.TermAnnotationId == id).FirstOrDefault());
                _context.SaveChanges();
                return RedirectToPage("/Projects/Index", new { id = ta });

            }

            return RedirectToPage("/Projects/Index");
        }

        public ActionResult OnPostAddAgreement()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && checkpoint)
            {
                // last updated
                MyDpAgreement.LastUpdateUserNavigation = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
                MyDpAgreement.LastUpdateDate = DateTime.Now;
                _context.Add(MyDpAgreement);

                // save any linked users
                _context.AddRange(DpAgreementUsers.Select(MyUser => new DpAgreementUser { UserId = MyUser, AgreementId = MyDpAgreement.AgreementId }));
                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = MyDpAgreement.DataProjectId });
        }

        public ActionResult OnPostRemoveAgreement(int Id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (checkpoint)
            {
                // remove any linked users and remove agreement.
                var ag = _context.DpAgreements.Where(x => x.AgreementId == Id).FirstOrDefault().DataProjectId;
                _context.DpAgreementUsers.RemoveRange(_context.DpAgreementUsers.Where(x => x.AgreementId == Id));
                _context.Remove(_context.DpAgreements.Where(x => x.AgreementId == Id).FirstOrDefault());
                _context.SaveChanges();
                return RedirectToPage("/Projects/Index", new { id = Id });
            }

            return RedirectToPage("/Projects/Index");
        }

        public ActionResult OnPostEditAgreement()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (ModelState.IsValid && checkpoint)
            {
                var q = MyDpAgreement;
                DpAgreement EditedDpAgreement = _context.DpAgreements.Where(x => x.AgreementId == MyDpAgreement.AgreementId && x.DataProjectId == MyDpAgreement.DataProjectId).FirstOrDefault();
                // last updated
                EditedDpAgreement.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                EditedDpAgreement.LastUpdateDate = DateTime.Now;
                EditedDpAgreement.Description = MyDpAgreement.Description;
                EditedDpAgreement.MeetingDate = MyDpAgreement.MeetingDate;
                EditedDpAgreement.EffectiveDate = MyDpAgreement.EffectiveDate;
                EditedDpAgreement.Rank = MyDpAgreement.Rank;

                _context.Attach(EditedDpAgreement).State = EntityState.Modified;
                _context.SaveChanges();

                // save any linked users
                _context.RemoveRange(_context.DpAgreementUsers.Where(x => x.AgreementId.Equals(MyDpAgreement.AgreementId)));
                _context.AddRange(DpAgreementUsers.Select(MyUser => new DpAgreementUser { UserId = MyUser, AgreementId = MyDpAgreement.AgreementId }));
                _context.SaveChanges();
            }

            return RedirectToPage("/Projects/Index", new { id = MyDpAgreement.DataProjectId });
        }

        public ActionResult OnPostAddMilestone()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (!ModelState.IsValid || DpMilestone.StartDate is null || DpMilestone.MilestoneTemplateId is null || !checkpoint)
            {
                return RedirectToPage("/Projects/Index", new { id = DpMilestone.DataProjectId });
            }

            DpMilestone.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            DpMilestone.LastUpdateDate = DateTime.Now;
            _context.Add(DpMilestone);

            if (DpChecklist != null)
            {

                JObject json = JObject.Parse(DpChecklist);

                foreach (JProperty property in json.Properties())
                {
                    bool isNumeric = int.TryParse(property.Name, out int n);

                    if (isNumeric == true && property.Value.ToString() != "")
                    {
                        _context.Add(new DpMilestoneChecklist { Item = property.Value.ToString(), MilestoneTaskId = DpMilestone.MilestoneTaskId });
                    }
                }
            }

            _context.SaveChanges();

            return RedirectToPage("/Projects/Index", new { id = DpMilestone.DataProjectId });
        }

        public ActionResult OnPostDeleteMilestone(int id)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (checkpoint)
            {
                var ms = _context.DpMilestoneTasks.Where(x => x.MilestoneTaskId == id).FirstOrDefault().DataProjectId;
                _context.RemoveRange(_context.DpMilestoneChecklists.Where(x => x.MilestoneTaskId.Equals(DpMilestone.MilestoneTaskId)));
                _context.Remove(DpMilestone);
                _context.SaveChanges();
                return RedirectToPage("/Projects/Index", new { id = ms });
            }

            return RedirectToPage("/Projects/Index");
        }

        public ActionResult OnPostEditMilestone()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 28);
            if (!ModelState.IsValid || DpMilestone.StartDate is null || DpMilestone.MilestoneTemplateId is null || !checkpoint)
            {
                return RedirectToPage("/Projects/Index", new { id = DpMilestone.DataProjectId });
            }

            DpMilestoneTask EditedDpMilestone = _context.DpMilestoneTasks.Where(x => x.MilestoneTaskId.Equals(DpMilestone.MilestoneTaskId)).FirstOrDefault();


            EditedDpMilestone.LastUpdateUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            EditedDpMilestone.LastUpdateDate = DateTime.Now;
            EditedDpMilestone.Description = DpMilestone.Description;
            EditedDpMilestone.StartDate = DpMilestone.StartDate;
            EditedDpMilestone.EndDate = DpMilestone.EndDate;
            EditedDpMilestone.MilestoneTemplateId = DpMilestone.MilestoneTemplateId;
            EditedDpMilestone.OwnerId = DpMilestone.OwnerId;

            _context.SaveChanges();

            if (DpChecklist != null)
            {
                // remove old list then add new
                _context.RemoveRange(_context.DpMilestoneChecklists.Where(x => x.MilestoneTaskId.Equals(DpMilestone.MilestoneTaskId)));
                JObject json = JObject.Parse(DpChecklist);

                foreach (JProperty property in json.Properties())
                {
                    bool isNumeric = int.TryParse(property.Name, out int n);

                    if (isNumeric == true && property.Value.ToString() != "")
                    {
                        _context.Add(new DpMilestoneChecklist { Item = property.Value.ToString(), MilestoneTaskId = DpMilestone.MilestoneTaskId });
                    }
                }
            }

            _context.SaveChanges();

            return RedirectToPage("/Projects/Index", new { id = DpMilestone.DataProjectId });
        }

        public ActionResult OnPostCompleteTask(int Id, string Comments, DateTime DueDate)
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 29);
            DpMilestoneTask OldTask = _context.DpMilestoneTasks.Where(x => x.MilestoneTaskId == Id).Include(x => x.Owner).FirstOrDefault();
            if (!checkpoint)
            {
                return RedirectToPage("/Projects/Index", new { id = OldTask.DataProjectId });
            }

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            DpMilestoneTasksCompleted MyTask = new DpMilestoneTasksCompleted
            {
                Owner = OldTask.Owner.Fullname_Cust,
                DueDate = DueDate,
                DataProjectId = OldTask.DataProjectId,
                Comments = Comments,
                CompletionDate = DateTime.Now,
                CompletionUser = MyUser.Fullname_Cust
            };

            _context.Add(MyTask);
            _context.SaveChanges();

            return RedirectToPage("/Projects/Index", new { id = OldTask.DataProjectId });
        }

        public ActionResult OnPostCompleteChecklist()
        {
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 30);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (!ModelState.IsValid || CompletedChecklist.TaskId is null || CompletedChecklist.DataProjectId is null || CompletedChecklistItems.Length == 0 || !checkpoint)
            {
                return Content("error");
            }

            // remove any "unchecked" items.
            _context.RemoveRange(_context.DpMilestoneChecklistCompleteds.Where(x => x.DataProjectId == CompletedChecklist.DataProjectId && x.TaskId == CompletedChecklist.TaskId && x.TaskDate == CompletedChecklist.TaskDate && !CompletedChecklistItems.Contains(x.MilestoneChecklistId)));
            _context.SaveChanges();
            // get existing values
            int?[] ExistingItems = _context.DpMilestoneChecklistCompleteds.Where(x => x.DataProjectId == CompletedChecklist.DataProjectId && x.TaskId == CompletedChecklist.TaskId && x.TaskDate == CompletedChecklist.TaskDate && CompletedChecklistItems.Contains(x.MilestoneChecklistId)).Select(x => x.MilestoneChecklistId).ToArray();

            // add completions. get time before so all entries match.. but do not add doubles.
            DateTime TheCompletionDate = DateTime.Now;
            var q = CompletedChecklistItems.Where(x => !ExistingItems.Contains(x))
                .Select(x => new DpMilestoneChecklistCompleted
                {
                    MilestoneChecklistId = x,
                    TaskId = CompletedChecklist.TaskId,
                    TaskDate = CompletedChecklist.TaskDate,
                    DataProjectId = CompletedChecklist.DataProjectId,
                    CompletionDate = TheCompletionDate,
                    CompletionUser = MyUser.UserId
                });
            _context.AddRange(q);

            _context.SaveChanges();

            return Content("ok");
        }

        public async Task<ActionResult> OnPostNewComment()
        {
            if (!ModelState.IsValid || NewCommentReply.MessageText is null)
            {
                return RedirectToPage("/Projects/Index", new { id = NewComment.DataProjectId });
            }

            // if missing the conversation ID then create a new conversation.
            if (!_context.DpDataProjectConversationMessages.Any(x => x.DataProjectConversationId == NewComment.DataProjectConversationId))
            {
                // create new comment
                _context.Add(NewComment);
            }
            // add message
            NewCommentReply.UserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
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
            var checkpoint = UserHelpers.CheckUserPermissions(_cache, _context, User.Identity.Name, 25);
            if (ModelState.IsValid && checkpoint)
            {
                // first delete all replys on comment. Delete comment.
                _context.RemoveRange(_context.DpDataProjectConversationMessages.Where(x => x.DataProjectConversationId == NewComment.DataProjectConversationId));
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
                    ModelState.AddModelError(file.Name, "The file was not received by the server. If this issue persists please contact Analytics.");
                    return false;
                }
                else if (file.Length > 1024 * 1024)
                {
                    ModelState.AddModelError(file.Name, "The file is larger than 1MB. Please use a smaller image.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(file.Name, $"The file upload failed. Please contact Analytics if this issue persists. Error: {ex.Message}");
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
                var MyFile = new DpAttachment
                {
                    DataProjectId = Id
                };
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
            return RedirectToPage("/Projects/Index", new { id = Id });
        }

        public ActionResult OnPostRemoveFile(int Id, int FileId)
        {
            _context.Remove(_context.DpAttachments.Where(t => t.AttachmentId == FileId).FirstOrDefault());
            _context.SaveChanges();
            return RedirectToPage("/Projects/Index", new { id = Id });
        }
    }
}
