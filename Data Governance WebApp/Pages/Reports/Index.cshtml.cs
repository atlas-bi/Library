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
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Data_Governance_WebApp.Pages.Reports
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

        public class ReportData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Author { get; set; }
            public int? AuthorId { get; set; }
            public string LastUpdatedBy { get; set; }
            public int? LastUpdatedById { get; set; }
            public string OpsOwner { get; set; }
            public string Requester { get; set; }
            public string MaintSched { get; set; }
            public int? MaintSchedId { get; set; }
            public string RunFreq { get; set; }
            public int? RunFreqId { get; set; }
            public string Fragility { get; set; }
            public string Value { get; set; }
            public string DeveloperDescription { get; set; }
            public string KeyAssumptions { get; set; }
            public string Description { get; set; }
            public string SystemDescription { get; set; }
            public string LastModifiedDate { get; set; }
            public string EpicMasterFile { get; set; }
            public string Orphaned { get; set; }
            public string GitUrl { get; set; }
            public string ExecVisibitliy { get; set; }
            public string Hyperspace { get; set; }
            public string DoNotPurge { get; set; }
            public string Type { get; set; }
            public string Hidden { get; set; }
            public decimal? EpicId { get; set; }
            public decimal? EpicTemplateId { get; set; }
            public int? OpsOwnerId { get; set; }
            public int? RequesterId { get; set; }
            public int? FragilityId { get; set; }
            public int? ValueId { get; set; }
            public string RunReportUrl { get; set; }
            public string EditReportUrl { get; set; }
            public string RecordViewerUrl { get; set; }
            public string DocLastUpdated { get; set; }
            public string Favorite { get; set; }
            public int? DocLastUpdatedById { get; set; }
            public string DocLastUpdatedBy { get; set; }
            public string ManageReportUrl { get; set; }
            public IEnumerable<ManageEngineTicketsData> ManageEngineTickets { get; set; }
        }

        public class ManageEngineTicketsData
        {
            public int Id { get; set; }
            public int? Number { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public int? ReportId { get; set; }
        }

        public class BasicFavoriteReportData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Favorite { get; set; }
            public string ReportUrl { get; set; }
        }

        public class ReportFragilityTagData
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class ReportTermsData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Summary { get; set; }
            public string Definition { get; set; }
            public int? ReportId { get; set; }
        }

        public class ReportQueryData
        {
            public string Query { get; set; }
            public int Id { get; set; }
        }

        public class ReportMaintLogsData
        {
            public string Date { get; set; }
            public string Comment { get; set; }
            public string Maintainer { get; set; }
            public string Status { get; set; }
        }

        public class ReportImagesData
        {
            public string Src { get; set; }
            public int Id { get; set; }
        }

        public class ReportCommentsData
        {
            public int ReportId { get; set; }
            public string Date { get; set; }
            public int ConvId { get; set; }
            public int MessId { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
        }

        public class ReportChildrenData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string EpicMasterFile { get; set; }
            public string Favorite { get; set; }
            public string RunReportUrl { get; set; }
            public IEnumerable<ChildImgData> Img { get; set; }
            public IEnumerable<ChildData> Child { get; set; }
        }

        public class ChildImgData
        {
            public string Src { get; set; }
            public int Id { get; set; }
        }

        public class ChildData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string EpicMasterFile { get; set; }
            public string Favorite { get; set; }
            public string RunReportUrl { get; set; }
            public IEnumerable<GrandChildData> GrandChild { get; set; }
        }

        public class GrandChildData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string EpicMasterFile { get; set; }
            public string Favorite { get; set; }
            public string RunReportUrl { get; set; }
        }
        public User PublicUser { get; set; }
        public List<UserFavorites> Favorites { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        public List<int?> Permissions { get; set; }
        public List<AdList> AdLists { get; set; }
        [BindProperty] public ReportObject ReportObject { get; set; }
        [BindProperty] public ReportObjectDoc ReportObjectDoc { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public ReportObjectConversationDoc NewComment { get; set; }
        [BindProperty] public MaintenanceLog NewMaintenanceLog { get; set; }
        [BindProperty] public ReportObjectDocMaintenanceLogs NewMaintenanceLogLink { get; set; }
        [BindProperty] public ReportObjectConversationMessageDoc NewCommentReply { get; set; }
        [BindProperty] public int[] SelectedFragilityTagIds { get; set; }
        [BindProperty] public ReportObjectDocTerms NewTermLink { get; set; }
        [BindProperty] public Term NewTerm { get; set; }
        [BindProperty] public ReportManageEngineTickets ManageEngineTicket { get; set;}
        public ReportData Report { get; set; }
        public List<ReportFragilityTagData> ReportFragilityTags { get; set; }
        public ReportObjectImagesDoc RemovedImage { get; set; }
        public IEnumerable<ReportTermsData> ReportTerms { get; set; }
        public IEnumerable<ReportQueryData> ReportQuery { get; set; }
        public IEnumerable<ReportMaintLogsData> ReportMaintLogs { get; set; }
        public IEnumerable<ReportCommentsData> ReportComments { get; set; }
        public IEnumerable<ReportChildrenData> ReportChildren { get; set; }
        public IEnumerable<ReportChildrenData> ReportParents { get; set; }
        public IEnumerable<ManageEngineTicketsData> ManageEngineTickets { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            ViewData["Fullname"] = MyUser.Fullname_Cust;
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            Report = await (from r in _context.ReportObject
                            where r.ReportObjectId == id
                            join q in (from f in _context.UserFavorites
                                       where f.ItemType.ToLower() == "report"
                                          && f.UserId == MyUser.UserId
                                       select new { f.ItemId })
                            on r.ReportObjectId equals q.ItemId into tmp
                            from rfi in tmp.DefaultIfEmpty()
                            select new ReportData {
                                Id = r.ReportObjectId,
                                Name = r.Name,
                                Author = r.AuthorUser.Fullname_Cust,
                                AuthorId = r.AuthorUserId,
                                LastUpdatedBy = r.LastModifiedByUser.Fullname_Cust,
                                LastUpdatedById = r.LastModifiedByUserId,
                                DocLastUpdated = r.ReportObjectDoc.LastUpdatedDateTimeDisplayString,
                                DocLastUpdatedBy = r.ReportObjectDoc.UpdatedByNavigation.Fullname_Cust,
                                DocLastUpdatedById = r.ReportObjectDoc.UpdatedBy,
                                OpsOwner = r.ReportObjectDoc.OperationalOwnerUser.Fullname_Cust,
                                OpsOwnerId = r.ReportObjectDoc.OperationalOwnerUserId,
                                Requester = r.ReportObjectDoc.RequesterNavigation.Fullname_Cust,
                                RequesterId = r.ReportObjectDoc.Requester,
                                MaintSched = r.ReportObjectDoc.MaintenanceSchedule.MaintenanceScheduleName,
                                MaintSchedId = r.ReportObjectDoc.MaintenanceScheduleId,
                                RunFreq = r.ReportObjectDoc.EstimatedRunFrequency.EstimatedRunFrequencyName,
                                RunFreqId = r.ReportObjectDoc.EstimatedRunFrequencyId,
                                Fragility = r.ReportObjectDoc.Fragility.FragilityName,
                                FragilityId = r.ReportObjectDoc.FragilityId,
                                Value = r.ReportObjectDoc.OrganizationalValue.OrganizationalValueName,
                                ValueId = r.ReportObjectDoc.OrganizationalValueId,
                                DeveloperDescription = r.ReportObjectDoc.DeveloperDescription,
                                KeyAssumptions = r.ReportObjectDoc.KeyAssumptions,
                                Description = r.Description,
                                SystemDescription = r.DetailedDescription,
                                LastModifiedDate = r.LastUpdatedDateDisplayString,
                                EpicMasterFile = r.EpicMasterFile,
                                EpicId = r.EpicRecordId,
                                Orphaned = r.OrphanedReportObjectYn,
                                GitUrl = r.ReportObjectDoc.GitLabProjectUrl,
                                ExecVisibitliy = r.ReportObjectDoc.ExecutiveVisibilityYn,
                                Hyperspace = r.ReportObjectDoc.EnabledForHyperspace,
                                DoNotPurge = r.ReportObjectDoc.DoNotPurge,
                                Hidden = r.ReportObjectDoc.Hidden,
                                Type = r.ReportObjectType.Name,
                                EpicTemplateId = r.EpicReportTemplateId,
                                Favorite = rfi.ItemId == null ? "no" : "yes",
                                RunReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, r.ReportObjectUrl, r.Name, r.ReportObjectType.Name, r.EpicReportTemplateId.ToString(), r.EpicRecordId.ToString(), r.EpicMasterFile, r.ReportObjectDoc.EnabledForHyperspace),
                                EditReportUrl = HtmlHelpers.EditReportFromParams(_config["AppSettings:org_domain"], HttpContext, r.ReportServerPath, r.SourceServer, r.EpicMasterFile, r.EpicReportTemplateId.ToString(), r.EpicRecordId.ToString()),
                                RecordViewerUrl = HtmlHelpers.RecordViewerLink(_config["AppSettings:org_domain"], HttpContext, r.EpicMasterFile, r.EpicRecordId.ToString()),
                                ManageReportUrl = HtmlHelpers.ReportManageUrlFromParams(_config["AppSettings:org_domain"], HttpContext,r.ReportObjectType.Name, r.ReportServerPath,r.SourceServer)
                            }
                            ).FirstOrDefaultAsync();

            ManageEngineTickets = await (from t in _context.ReportManageEngineTickets
                                         where t.ReportObjectId == id
                                         select new ManageEngineTicketsData
                                         {
                                             Id = t.ManageEngineTicketsId,
                                             Number = t.TicketNumber,
                                             Url = t.TicketUrl,
                                             Description = t.Description,
                                             ReportId = t.ReportObjectId
                                         }).ToListAsync();

            ReportFragilityTags = await (from r in _context.ReportObjectDocFragilityTags
                                         where r.ReportObjectId == id
                                         select new ReportFragilityTagData { Name = r.FragilityTag.FragilityTagName, Id = r.FragilityTagId }).ToListAsync();

            ReportTerms = await (from r in _context.ReportObjectDocTerms
                                 where r.ReportObjectId == id
                                 select new ReportTermsData
                                 {
                                     Name = r.Term.Name,
                                     Id = r.TermId,
                                     Summary = r.Term.Summary,
                                     Definition = r.Term.TechnicalDefinition,
                                     ReportId = r.ReportObjectId
                                 }).ToListAsync();

            ReportQuery = await (from r in _context.ReportObjectQuery
                                 where r.ReportObjectId == id
                                 select new ReportQueryData { Query = r.Query, Id = r.ReportObjectQueryId }).ToListAsync();

            ReportMaintLogs = await (from l in _context.ReportObjectDocMaintenanceLogs
                                     where l.ReportObjectId == id
                                     orderby l.MaintenanceLog.MaintenanceDate descending
                                     select new ReportMaintLogsData
                                     {
                                         Date = l.MaintenanceLog.MaintenanceDateDisplayString,
                                         Comment = l.MaintenanceLog.Comment,
                                         Maintainer = l.MaintenanceLog.Maintainer.FullName,
                                         Status = l.MaintenanceLog.MaintenanceLogStatus.MaintenanceLogStatusName
                                     }).ToListAsync();

            ViewData["ReportImages"] = await (from i in _context.ReportObjectImagesDoc
                                  where i.ReportObjectId == id
                                  orderby i.ImageOrdinal
                                  select new ReportImagesData
                                  {
                                      Src = "data/img?id=" + i.ImageId,
                                      Id = i.ImageId
                                  }).ToListAsync();

            ViewData["Id"] = id;

            ReportChildren = await (from c in _context.ReportObjectHierarchy
                                    where c.ParentReportObjectId == id
                                       && c.ChildReportObject.EpicMasterFile != "IDK"
                                    join q in (from f in _context.UserFavorites
                                               where f.ItemType.ToLower() == "report"
                                                  && f.UserId == MyUser.UserId
                                               select new { f.ItemId })
                                       on c.ChildReportObjectId equals q.ItemId into tmp
                                    from rfi in tmp.DefaultIfEmpty()
                                    orderby c.Line, c.ChildReportObject.Name
                                    select new ReportChildrenData
                                    {
                                        Name = c.ChildReportObject.Name,
                                        Id = c.ChildReportObjectId,
                                        Favorite = rfi.ItemId == null ? "no" : "yes",
                                        EpicMasterFile = c.ChildReportObject.EpicMasterFile,
                                        RunReportUrl = Helpers.HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, c.ChildReportObject.ReportObjectUrl, c.ChildReportObject.Name, c.ChildReportObject.ReportObjectType.Name, c.ChildReportObject.EpicReportTemplateId.ToString(), c.ChildReportObject.EpicRecordId.ToString(), c.ChildReportObject.EpicMasterFile, c.ChildReportObject.ReportObjectDoc.EnabledForHyperspace),
                                        Img =
                                            from img in c.ChildReportObject.ReportObjectImagesDoc
                                            orderby img.ImageOrdinal
                                            select new ChildImgData
                                            {
                                                Src = "data/img?id=" + img.ImageId,
                                                Id = img.ImageId
                                            },
                                        Child =
                                            from nc in c.ChildReportObject.ReportObjectHierarchyParentReportObject
                                            where nc.ChildReportObject.EpicMasterFile != "IDK"
                                            join q in (from f in _context.UserFavorites
                                                       where f.ItemType.ToLower() == "report"
                                                          && f.UserId == MyUser.UserId
                                                       select new { f.ItemId })
                                             on nc.ChildReportObjectId equals q.ItemId into tmp
                                            from rfi in tmp.DefaultIfEmpty()
                                            orderby nc.Line, nc.ChildReportObject.Name
                                            select new ChildData {
                                                Name = nc.ChildReportObject.Name,
                                                Id = nc.ChildReportObjectId,
                                                Favorite = rfi.ItemId == null ? "no" : "yes",
                                                EpicMasterFile = nc.ChildReportObject.EpicMasterFile,
                                                RunReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, nc.ChildReportObject.ReportObjectUrl, nc.ChildReportObject.Name, nc.ChildReportObject.ReportObjectType.Name, nc.ChildReportObject.EpicReportTemplateId.ToString(), nc.ChildReportObject.EpicRecordId.ToString(), nc.ChildReportObject.EpicMasterFile, nc.ChildReportObject.ReportObjectDoc.EnabledForHyperspace),
                                                GrandChild =
                                                    from gc in nc.ChildReportObject.ReportObjectHierarchyParentReportObject
                                                    where gc.ChildReportObject.EpicMasterFile != "IDK"
                                                    join q in (from f in _context.UserFavorites
                                                               where f.ItemType.ToLower() == "report"
                                                                  && f.UserId == MyUser.UserId
                                                               select new { f.ItemId })
                                                    on gc.ChildReportObjectId equals q.ItemId into tmp
                                                    from rfi in tmp.DefaultIfEmpty()
                                                    orderby gc.Line, gc.ChildReportObject.Name
                                                    select new GrandChildData
                                                    {
                                                        Name = gc.ChildReportObject.Name,
                                                        Id = gc.ChildReportObject.ReportObjectId,
                                                        EpicMasterFile = gc.ChildReportObject.EpicMasterFile,
                                                        Favorite = rfi.ItemId == null ? "no" : "yes",
                                                        RunReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, gc.ChildReportObject.ReportObjectUrl, gc.ChildReportObject.Name, gc.ChildReportObject.ReportObjectType.Name, gc.ChildReportObject.EpicReportTemplateId.ToString(), gc.ChildReportObject.EpicRecordId.ToString(), gc.ChildReportObject.EpicMasterFile, gc.ChildReportObject.ReportObjectDoc.EnabledForHyperspace),
                                                    }
                                            }
                                    }).ToListAsync();
            ReportParents = await (from p in _context.ReportObjectHierarchy
                                   where p.ChildReportObjectId == id
                                      && (p.ParentReportObject.DefaultVisibilityYn == "Y" || p.ParentReportObject.EpicMasterFile == "IDK")
                                   join q in (from f in _context.UserFavorites
                                              where f.ItemType.ToLower() == "report"
                                                 && f.UserId == MyUser.UserId
                                              select new { f.ItemId })
                                          on p.ParentReportObjectId equals q.ItemId into tmp
                                   from rfi in tmp.DefaultIfEmpty()
                                   orderby p.Line, p.ParentReportObject.Name
                                   select new ReportChildrenData
                                   {
                                       Name = p.ParentReportObject.Name,
                                       Id = p.ParentReportObjectId,
                                       Favorite = rfi.ItemId == null ? "no" : "yes",
                                       RunReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, p.ParentReportObject.ReportObjectUrl, p.ParentReportObject.Name, p.ParentReportObject.ReportObjectType.Name, p.ParentReportObject.EpicReportTemplateId.ToString(), p.ParentReportObject.EpicRecordId.ToString(), p.ParentReportObject.EpicMasterFile, p.ParentReportObject.ReportObjectDoc.EnabledForHyperspace),
                                       Img =
                                           from img in p.ParentReportObject.ReportObjectImagesDoc
                                           orderby img.ImageOrdinal
                                           select new ChildImgData
                                           {
                                               Src = "data/img?id=" + img.ImageId,
                                               Id = img.ImageId
                                           },
                                   }).ToListAsync();

            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);

            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2},
                new AdList { Url = "Reports/?handler=RelatedReports&id="+id, Column = 2 },
                new AdList { Url = "/?handler=RecentReports", Column = 2 },
                new AdList { Url = "/?handler=RecentTerms", Column = 2 },
                new AdList { Url = "/?handler=RecentInitiatives", Column = 2 },
                new AdList { Url = "/?handler=RecentProjects", Column = 2 }
            };
            ViewData["AdLists"] = AdLists;
            return Page();
        }
        
        public async Task<ActionResult> OnGetComments(int id)
        {
            ViewData["Comments"] = await (from c in _context.ReportObjectConversationMessageDoc
                                    where c.Conversation.ReportObjectId == id
                                    orderby c.ConversationId descending, c.MessageId ascending
                                    select new ReportCommentsData
                                    {
                                        ReportId = id,
                                        Date = c.PostDateTimeDisplayString,
                                        ConvId = c.ConversationId,
                                        MessId = c.MessageId,
                                        User = c.User.Fullname_Cust,
                                        Text = c.MessageText
                                    }).ToListAsync();
            ViewData["Id"] = id;
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("_Comments");
        }
        public async Task<ActionResult> OnPostNewDescription()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var package = JObject.Parse(reader.ReadToEnd());

                var IdStr = package.Value<string>("id");
                var Description = package.Value<string>("description");

                if (IdStr.Length > 0)
                {
                    var Id = Int32.Parse(IdStr);

                    DateTime Updated = DateTime.Now;
                    var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

                    if (_context.ReportObjectDoc.Any(x => x.ReportObjectId == Id))
                    {
                        ReportObjectDoc myDoc = await _context.ReportObjectDoc.Where(x => x.ReportObjectId == Id).FirstOrDefaultAsync();
                        myDoc.DeveloperDescription = Description;
                        myDoc.LastUpdateDateTime = Updated;
                        myDoc.UpdatedBy = MyUser.UserId;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ReportObjectDoc myDoc = new ReportObjectDoc
                        {
                            ReportObjectId = Id,
                            DeveloperDescription = Description,
                            CreatedBy = MyUser.UserId,
                            CreatedDateTime = Updated,
                            LastUpdateDateTime = Updated,
                            UpdatedBy = MyUser.UserId
                        };

                        _context.Add(myDoc);
                        await _context.SaveChangesAsync();
                    }

                    var d = await (from q in _context.ReportObject
                                   where q.ReportObjectId == Id
                                   select new
                                   {
                                       q.ReportObjectDoc.DeveloperDescription,
                                       q.ReportObjectDoc.KeyAssumptions,
                                       q.Description,
                                       SystemDescription = q.DetailedDescription
                                   }).FirstOrDefaultAsync();

                    ViewData["DeveloperDescription"] = d.DeveloperDescription;
                    ViewData["KeyAssumptions"] = d.KeyAssumptions;
                    ViewData["Description"] = d.Description;
                    ViewData["SystemDescription"] = d.SystemDescription;

                    return Partial("_Description");
                }
                return Content("error");
            }
             
        }
        public async Task<ActionResult> OnPostNewKeyAssumptions()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {

                var package = JObject.Parse(reader.ReadToEnd());
                var IdStr = package.Value<string>("id");
                var Description = package.Value<string>("description");

                if (IdStr.Length > 0)
                {
                    var Id = Int32.Parse(IdStr);
                    DateTime Updated = DateTime.Now;
                    var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

                    if (_context.ReportObjectDoc.Any(x => x.ReportObjectId == Id))
                    {
                        ReportObjectDoc myDoc = await _context.ReportObjectDoc.Where(x => x.ReportObjectId == Id).FirstOrDefaultAsync();
                        myDoc.KeyAssumptions = Description;
                        myDoc.LastUpdateDateTime = Updated;
                        myDoc.UpdatedBy = MyUser.UserId;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ReportObjectDoc myDoc = new ReportObjectDoc
                        {
                            ReportObjectId = Id,
                            KeyAssumptions = Description,
                            CreatedBy = MyUser.UserId,
                            CreatedDateTime = Updated,
                            LastUpdateDateTime = Updated,
                            UpdatedBy = MyUser.UserId
                        };

                        _context.Add(myDoc);
                        await _context.SaveChangesAsync();
                    }

                    var d = await (from q in _context.ReportObject
                                   where q.ReportObjectId == Id
                                   select new
                                   {
                                       q.ReportObjectDoc.DeveloperDescription,
                                       q.ReportObjectDoc.KeyAssumptions,
                                       q.Description,
                                       SystemDescription = q.DetailedDescription
                                   }).FirstOrDefaultAsync();

                    ViewData["DeveloperDescription"] = d.DeveloperDescription;
                    ViewData["KeyAssumptions"] = d.KeyAssumptions;
                    ViewData["Description"] = d.Description;
                    ViewData["SystemDescription"] = d.SystemDescription;

                    return Partial("_Description");
                }
                return Content("error");
            }

        }
        public async Task<IActionResult> OnPostNewDocumentation()
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            //Retrieve Report Object without tracking so we can attach the reportobjectdoc directly from the form. Otherwise you have to update each field in the existing reportobjectdoc.
            ReportObject oldReportObject = _context.ReportObject.AsNoTracking().Include(r => r.ReportObjectDoc).Where(o => o.ReportObjectId.Equals(ReportObject.ReportObjectId)).FirstOrDefault();
            
            // add Last update date
            DateTime Updated = DateTime.Now;
            
            if (oldReportObject == null)
            {
                return NotFound();
            }   
            else if(oldReportObject.ReportObjectDoc == null)
            {
                ReportObjectDoc.ReportObjectId = ReportObject.ReportObjectId;
                ReportObjectDoc.CreatedBy = MyUser.UserId;
                ReportObjectDoc.CreatedDateTime = Updated;
                _context.Add(ReportObjectDoc);
                await _context.SaveChangesAsync();
            }
            else
            {
                // for edited reports

                // fields are not set by the form:
                //   Creation Date, Created By, description, key assumptions


                // get reference to old record
                ReportObjectDoc oldReportObjectDoc = _context.ReportObjectDoc.AsNoTracking().Where(o => o.ReportObjectId.Equals(ReportObject.ReportObjectId)).FirstOrDefault();

                // these fields must be copied over form the record as everythign is overwritten on each save.
                ReportObjectDoc.ReportObjectId = ReportObject.ReportObjectId;
                ReportObjectDoc.DeveloperDescription = oldReportObjectDoc.DeveloperDescription;
                ReportObjectDoc.KeyAssumptions = oldReportObjectDoc.KeyAssumptions;
                ReportObjectDoc.CreatedDateTime = oldReportObjectDoc.CreatedDateTime;
                ReportObjectDoc.CreatedBy = oldReportObjectDoc.CreatedBy;
                ReportObjectDoc.LastUpdateDateTime = Updated; //Set the current datetime as the last update date
                ReportObjectDoc.UpdatedBy = MyUser.UserId;
                _context.Attach(ReportObjectDoc).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportObjectDocExists(ReportObjectDoc.ReportObjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // remove old links, add new.
            _context.RemoveRange(_context.ReportObjectDocFragilityTags.Where(t => t.ReportObjectId.Equals(ReportObject.ReportObjectId)));
            _context.AddRange(SelectedFragilityTagIds.Select(tagid => new ReportObjectDocFragilityTags {ReportObjectId = ReportObjectDoc.ReportObjectId, FragilityTagId = tagid}));

            _context.SaveChanges();

            ViewData["Report"] = await (from r in _context.ReportObject
                            where r.ReportObjectId == ReportObject.ReportObjectId
                            join q in (from f in _context.UserFavorites
                                       where f.ItemType.ToLower() == "report"
                                          && f.UserId == MyUser.UserId
                                       select new { f.ItemId })
                            on r.ReportObjectId equals q.ItemId into tmp
                            from rfi in tmp.DefaultIfEmpty()
                            select new ReportData
                            {
                                Id = r.ReportObjectId,
                                Name = r.Name,
                                Author = r.AuthorUser.Fullname_Cust,
                                AuthorId = r.AuthorUserId,
                                LastUpdatedBy = r.LastModifiedByUser.Fullname_Cust,
                                LastUpdatedById = r.LastModifiedByUserId,
                                DocLastUpdated = r.ReportObjectDoc.LastUpdatedDateTimeDisplayString,
                                DocLastUpdatedBy = r.ReportObjectDoc.UpdatedByNavigation.Fullname_Cust,
                                DocLastUpdatedById = r.ReportObjectDoc.UpdatedBy,
                                OpsOwner = r.ReportObjectDoc.OperationalOwnerUser.Fullname_Cust,
                                OpsOwnerId = r.ReportObjectDoc.OperationalOwnerUserId,
                                Requester = r.ReportObjectDoc.RequesterNavigation.Fullname_Cust,
                                RequesterId = r.ReportObjectDoc.Requester,
                                MaintSched = r.ReportObjectDoc.MaintenanceSchedule.MaintenanceScheduleName,
                                MaintSchedId = r.ReportObjectDoc.MaintenanceScheduleId,
                                RunFreq = r.ReportObjectDoc.EstimatedRunFrequency.EstimatedRunFrequencyName,
                                RunFreqId = r.ReportObjectDoc.EstimatedRunFrequencyId,
                                Fragility = r.ReportObjectDoc.Fragility.FragilityName,
                                FragilityId = r.ReportObjectDoc.FragilityId,
                                Value = r.ReportObjectDoc.OrganizationalValue.OrganizationalValueName,
                                ValueId = r.ReportObjectDoc.OrganizationalValueId,
                                LastModifiedDate = r.LastUpdatedDateDisplayString,
                                EpicMasterFile = r.EpicMasterFile,
                                EpicId = r.EpicRecordId,
                                Orphaned = r.OrphanedReportObjectYn,
                                GitUrl = r.ReportObjectDoc.GitLabProjectUrl,
                                ExecVisibitliy = r.ReportObjectDoc.ExecutiveVisibilityYn,
                                Hyperspace = r.ReportObjectDoc.EnabledForHyperspace,
                                DoNotPurge = r.ReportObjectDoc.DoNotPurge,
                                Hidden = r.ReportObjectDoc.Hidden,
                                Type = r.ReportObjectType.Name,
                                EpicTemplateId = r.EpicReportTemplateId,
                                ManageEngineTickets = from t in r.ReportManageEngineTickets select new ManageEngineTicketsData { Id = t.ManageEngineTicketsId, Number = t.TicketNumber, Url = t.TicketUrl, Description = t.Description }
                            }
                           ).FirstOrDefaultAsync();

            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["ManageEngineTickets"] = await (from t in _context.ReportManageEngineTickets
                                         where t.ReportObjectId == ReportObject.ReportObjectId
                                         select new ManageEngineTicketsData
                                         {
                                             Id = t.ManageEngineTicketsId,
                                             Number = t.TicketNumber,
                                             Url = t.TicketUrl,
                                             Description = t.Description,
                                            ReportId = t.ReportObjectId
                                        }).ToListAsync();

            ViewData["ReportFragilityTags"] = await (from r in _context.ReportObjectDocFragilityTags
                                         where r.ReportObjectId == ReportObject.ReportObjectId
                                         select new ReportFragilityTagData {
                                             Name = r.FragilityTag.FragilityTagName,
                                             Id = r.FragilityTagId
                                         }).ToListAsync();

            return Partial("_Details");
            //return RedirectToPage("/Reports/Index", new { id = ReportObject.ReportObjectId });
        }

        public async Task<ActionResult> OnPostAddTermLink()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Reports/Index", new {id = NewTermLink.ReportObjectId });
            }

            // only add link if not existing.
            if(!_context.ReportObjectDocTerms.Any(x => x.ReportObjectId == NewTermLink.ReportObjectId && x.TermId == NewTermLink.TermId))
            {
                _context.Add(NewTermLink);

                // update last update date on report. 
                // If report doc does not exist we will create.
                if (!_context.ReportObjectDoc.Any(d => d.ReportObjectId == NewTermLink.ReportObjectId))
                {
                    _context.Add(new ReportObjectDoc { ReportObjectId = NewTermLink.ReportObjectId });
                    await _context.SaveChangesAsync();
                }

                _context.ReportObjectDoc.Where(d => d.ReportObjectId == NewTermLink.ReportObjectId).FirstOrDefault().LastUpdateDateTime = DateTime.Now;
                _context.ReportObjectDoc.Where(d => d.ReportObjectId == NewTermLink.ReportObjectId).FirstOrDefault().UpdatedBy = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
                await _context.SaveChangesAsync();
            }
            
            ViewData["ReportTerms"] = await(from r in _context.ReportObjectDocTerms
                                where r.ReportObjectId == NewTermLink.ReportObjectId
                                            select new ReportTermsData
                                {
                                    Name = r.Term.Name,
                                    Id = r.TermId,
                                    Summary = r.Term.Summary,
                                    Definition = r.Term.TechnicalDefinition,
                                    ReportId = r.ReportObjectId
                                }).ToListAsync();

            return Partial("_Terms");
        }

        public async Task<ActionResult> OnPostRemoveTermLink() 
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Reports/Index", new {id = NewTermLink.ReportObjectId});
            }

            // remove term link & update last updated on report
            _context.Remove(NewTermLink);
            _context.ReportObjectDoc.Where(x => x.ReportObjectId == NewTermLink.ReportObjectId).FirstOrDefault().LastUpdateDateTime = DateTime.Now;
            _context.ReportObjectDoc.Where(x => x.ReportObjectId == NewTermLink.ReportObjectId).FirstOrDefault().UpdatedBy = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            await _context.SaveChangesAsync();

            ViewData["ReportTerms"] = await (from r in _context.ReportObjectDocTerms
                                            where r.ReportObjectId == NewTermLink.ReportObjectId
                                            select new ReportTermsData
                                            {
                                                Name = r.Term.Name,
                                                Id = r.TermId,
                                                Summary = r.Term.Summary,
                                                Definition = r.Term.TechnicalDefinition,
                                                ReportId = r.ReportObjectId
                                            }).ToListAsync();

            return Partial("_Terms");
        }

        public async Task<ActionResult> OnGetTermLinks(int Id)
        {
            ViewData["ReportTerms"] = await (from r in _context.ReportObjectDocTerms
                                             where r.ReportObjectId == Id
                                             select new ReportTermsData
                                             {
                                                 Name = r.Term.Name,
                                                 Id = r.TermId,
                                                 Summary = r.Term.Summary,
                                                 Definition = r.Term.TechnicalDefinition,
                                                 ReportId = r.ReportObjectId
                                             }).ToListAsync();

            return Partial("Editor/_CurrentTerms");
        }
        public async Task<ActionResult> OnPostNewMaintenanceLog()
        {
            // 1 check if report exists, else create it
            if (!_context.ReportObjectDoc.Any(r => r.ReportObjectId == NewMaintenanceLogLink.ReportObjectId))
            {
                _context.Add(new ReportObjectDoc { ReportObjectId = NewMaintenanceLogLink.ReportObjectId });
            }

            NewMaintenanceLog.MaintainerId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewMaintenanceLog.MaintenanceDate = System.DateTime.Now;

            _context.Add(NewMaintenanceLog);

            NewMaintenanceLogLink.MaintenanceLogId = NewMaintenanceLog.MaintenanceLogId;
            _context.Add(NewMaintenanceLogLink);
            
            await _context.SaveChangesAsync();
            
            ViewData["ReportMaintLogs"] = await (from l in _context.ReportObjectDocMaintenanceLogs
                                    where l.ReportObjectId == NewMaintenanceLogLink.ReportObjectId
                                                orderby l.MaintenanceLog.MaintenanceDate descending
                                    select new ReportMaintLogsData
                                    {
                                        Date = l.MaintenanceLog.MaintenanceDateDisplayString,
                                        Comment = l.MaintenanceLog.Comment,
                                        Maintainer = l.MaintenanceLog.Maintainer.FullName,
                                        Status = l.MaintenanceLog.MaintenanceLogStatus.MaintenanceLogStatusName
                                    }).ToListAsync();

            return Partial("_Maintenance");
        }

        public async Task<ActionResult> OnPostRemoveMeTicket()
        {
            if (ModelState.IsValid)
            {
                _context.Remove(ManageEngineTicket);
                _context.SaveChanges();
            }
            ViewData["ManageEngineTickets"] = await(from t in _context.ReportManageEngineTickets
                                                    where t.ReportObjectId == ManageEngineTicket.ReportObjectId
                                                    select new ManageEngineTicketsData
                                                    {
                                                        Id = t.ManageEngineTicketsId,
                                                        Number = t.TicketNumber,
                                                        Url = t.TicketUrl,
                                                        Description = t.Description,
                                                        ReportId = t.ReportObjectId
                                                    }).ToListAsync();

            return Partial("Partials/_MeTickets");
        }

        public async Task<ActionResult> OnPostAddMeTicket()
        {
            if (ModelState.IsValid && ManageEngineTicket.TicketNumber != null)
            {
                _context.Add(ManageEngineTicket);
                _context.SaveChanges();
            }

            ViewData["ManageEngineTickets"] = await(from t in _context.ReportManageEngineTickets
                                                    where t.ReportObjectId == ManageEngineTicket.ReportObjectId
                                                    select new ManageEngineTicketsData
                                                    {
                                                        Id = t.ManageEngineTicketsId,
                                                        Number = t.TicketNumber,
                                                        Url = t.TicketUrl,
                                                        Description = t.Description,
                                                        ReportId = t.ReportObjectId
                                                    }).ToListAsync();

            return Partial("Partials/_MeTickets");
        }

        public async Task<ActionResult> OnGetGetMeTicket(int Id)
        {
            ViewData["ManageEngineTickets"] = await (from t in _context.ReportManageEngineTickets
                                                     where t.ReportObjectId == Id
                                                     select new ManageEngineTicketsData
                                                     {
                                                         Id = t.ManageEngineTicketsId,
                                                         Number = t.TicketNumber,
                                                         Url = t.TicketUrl,
                                                         Description = t.Description,
                                                         ReportId = t.ReportObjectId
                                                     }).ToListAsync();


            return Partial("Editor/_MeTickets");
        }
        public async Task<ActionResult> OnPostNewComment()
        {
            if (!ModelState.IsValid || NewCommentReply.MessageText is null)
            {
                return RedirectToPage("/Reports/Index", new {id = NewComment.ReportObjectId});
            }

            // if missing the conversation ID then create a new conversation.
            if(!_context.ReportObjectConversationDoc.Any(x => x.ConversationId == NewComment.ConversationId)){
                // create new comment
                _context.Add(NewComment);
            }
            // add message
            NewCommentReply.UserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewCommentReply.PostDateTime = System.DateTime.Now;
            NewCommentReply.ConversationId = NewComment.ConversationId;
            _context.Add(NewCommentReply);
            _context.SaveChanges();

            return await OnGetComments(NewComment.ReportObjectId);
        }

        public async Task<ActionResult> OnPostDeleteComment()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Reports/Index", new { id = NewComment.ReportObjectId });
            }

            _context.Remove(NewCommentReply);
            _context.SaveChanges();

            return await OnGetComments(NewComment.ReportObjectId);
        }

        public async Task<ActionResult> OnPostDeleteCommentStream()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Reports/Index", new { id = NewComment.ReportObjectId });
            }

            // first delete all replys on comment. Delete comment.
            _context.RemoveRange(_context.ReportObjectConversationMessageDoc.Where(x => x.ConversationId == NewComment.ConversationId));
            _context.Remove(NewComment);
            _context.SaveChanges();

            return await OnGetComments(NewComment.ReportObjectId);
        }

        public ActionResult OnPostAddImage(int Id, IFormFile File)
        {
            if (ValidateFileUpload(File))
            {
                //ReportObjectImagesDoc img;
                var img = new ReportObjectImagesDoc
                {
                    ReportObjectId = Id
                };
                using (var stream = new MemoryStream())
                {
                    File.CopyTo(stream);
                    img.ImageData = stream.ToArray();
                }
                _context.Add(img);
                _context.SaveChanges();

                ViewData["Src"] = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(img.ImageData.ToArray()));
                ViewData["ImgId"] = img.ImageId;
                ViewData["Id"] = Id;

            return Partial("Editor/_EachImage");
            }
            return Content("error");
        }

        public async Task<ActionResult> OnPostRemoveImage(int Id, int ImgId)
        {
            _context.Remove(_context.ReportObjectImagesDoc.Where(t => t.ImageId == ImgId && t.ReportObjectId == Id).FirstOrDefault());
            _context.SaveChanges();
            return await OnGetLoadImagesAsync(Id);
        }

        public async Task<ActionResult> OnGetLoadImagesAsync(int id)
        {
            ViewData["ReportImages"] = await (from i in _context.ReportObjectImagesDoc
                                 where i.ReportObjectId == id
                                 orderby i.ImageOrdinal
                                 select new ReportImagesData
                                 {
                                     Src = "data/img?id=" + i.ImageId,
                                     Id = i.ImageId
                                 }).ToListAsync();
            ViewData["Id"] = id;
            return Partial("_Images");
        }

        public JsonResult OnPostCurrentTermDetails(int TermId)
        {
            Term termHistory = _context.Term.Where(th => th.TermId.Equals(TermId)).FirstOrDefault();
            return new JsonResult(termHistory);
        }

        // https://docs.microsoft.com/en-us/aspnet/core/razor-pages/upload-files?view=aspnetcore-2.2
        public bool ValidateFileUpload(IFormFile file)
        {
            try {
            // get the filename in case we need to return info to the user
            if (file.ContentType.ToLower() != "image/jpeg" && file.ContentType.ToLower() != "image/png")
            {
                ModelState.AddModelError(file.Name, "You may only upload jpeg and png files.");
                return false;
            }

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
            } catch (Exception ex)
            {
                ModelState.AddModelError(file.Name, $"The file upload failed. Please contact Analytics if this issue persists. Error: {ex.Message}");
                return false;
                            }
            // if there are no errors, return true
            return true;
        }
        private bool ReportObjectDocExists(int id)
        {
            return _context.ReportObjectDoc.Any(e => e.ReportObjectId == id);
        }

        public ActionResult OnGetRelatedReports(string id)
        {
            ViewData["RelatedReports"] = _cache.GetOrCreate<List<BasicFavoriteReportData>>("RelatedReports-" + id,
                cacheEntry => {
                    cacheEntry.SlidingExpiration = TimeSpan.FromHours(2);
                    // id is a number e.g. 1, or a list of numbers e.g. "1,2,3"
                    var user = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

                    var x = new List<BasicFavoriteReportData>();

                    using (var connection = new SqlConnection( _config.GetConnectionString("DataGovernanceDatabase")))
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandText = "RelatedReports";
                            command.Parameters.Add(new SqlParameter("@Id", id));
                            command.Parameters.Add(new SqlParameter("@UserId", user.UserId));

                            connection.Open();
                            var datareader = command.ExecuteReader();

                            while (datareader.Read())
                            {
                                x.Add(new BasicFavoriteReportData
                                {
                                    Id = (int)datareader["Id"],
                                    Name = datareader["Name"].ToString(),
                                    Favorite = datareader["Favorite"].ToString(),
                                    ReportUrl = HtmlHelpers.ReportUrlFromParams(_config["AppSettings:org_domain"], HttpContext, datareader["ReportObjectUrl"].ToString(),
                                                                                            datareader["Name"].ToString(),
                                                                                            datareader["ReportObjectType"].ToString(),
                                                                                            datareader["EpicReportTemplateId"].ToString(),
                                                                                            datareader["EpicRecordId"].ToString(),
                                                                                            datareader["EpicMasterFile"].ToString(),
                                                                                            datareader["EnabledForHyperspace"].ToString()
                                                                                            )
                                });
                            }
                        }
                    }
                    return x;
                });


            HttpContext.Response.Headers.Add("Cache-Control", "max-age=7200");
            return Partial("Partials/_RelatedReports");
        }
    }
}
