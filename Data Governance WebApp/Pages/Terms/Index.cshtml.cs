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
using Microsoft.AspNetCore.Mvc.Rendering;
using Data_Governance_WebApp.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Data_Governance_WebApp.Pages.Terms
{
    public class IndexModel : PageModel
    {
        // import model
        private readonly Data_GovernanceContext _context;
        private IMemoryCache _cache;
        public IndexModel(Data_GovernanceContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<UserFavorites> Favorites { get; set; }
        public List<UserPreferences> Preferences { get; set; }
        public User PublicUser { get; set; }
        public class TermCommentsData
        {
            public int TermId { get; set; }
            public string Date { get; set; }
            public int ConvId { get; set; }
            public int MessId { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
        }
        public class ReportTermsData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Summary { get; set; }
            public string Definition { get; set; }
            public int? ReportId { get; set; }
        }

        public IEnumerable<TermsData> AllTerms { get; set; }

        public TermsData MyTerm { get; set; }
        [BindProperty] public Term NewTerm { get; set; }
        [BindProperty] public ReportObjectDocTerms NewTermLink {get; set; }

        [BindProperty] public TermConversation NewComment { get; set; }
        [BindProperty] public TermConversationMessage NewCommentReply { get; set; }
        public List<AdList> AdLists { get; set; }
        public List<int?> Permissions { get; set; }

        public class TermsData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Approved { get; set; }
            public string UpdateDate { get; set; }
            public string Favorite { get; set; }
            public string Summary { get; set; }
            public string ApprovedBy { get; set; }
            public string UpdatedBy { get; set; }
            public string Definition { get; set; }
            public string ExtStdUrl { get; set; }
            public string ApprovedDate { get; set; }
            public string ValidDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            PublicUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            Permissions = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            ViewData["Permissions"] = Permissions;
            ViewData["SiteMessage"] = HtmlHelpers.SiteMessage(HttpContext, _context);
            Favorites = UserHelpers.GetUserFavorites(_cache, _context, User.Identity.Name);
            Preferences = UserHelpers.GetPreferences(_cache, _context, User.Identity.Name);
            ViewData["MyRole"] = UserHelpers.GetMyRole(_cache, _context, User.Identity.Name);

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
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            if (id != null)
            {
                // try to get Term by id
                MyTerm = await (from t in _context.Term
                              where t.TermId == id
                              join q in (from f in _context.UserFavorites
                                         where f.ItemType.ToLower() == "term"
                                            && f.UserId == MyUser.UserId
                                         select new { f.ItemId })
                              on t.TermId equals q.ItemId into tmp
                              from rfi in tmp.DefaultIfEmpty()
                              orderby t.ApprovedYn.ToString() ?? "N" descending, t.LastUpdatedDateTime descending
                              select new TermsData
                              {
                                  Id = t.TermId,
                                  Name = t.Name,
                                  Approved = t.ApprovedYn.ToString() ?? "N",
                                  Favorite = rfi.ItemId == null ? "no" : "yes",
                                  UpdateDate = t.LastUpdatedDateTimeDisplayString,
                                  Summary = t.Summary,
                                  Definition = t.TechnicalDefinition,
                                  UpdatedBy = t.UpdatedByUser.Fullname_Cust,
                                  ApprovedBy = t.ApprovedByUser.Fullname_Cust,
                                  ExtStdUrl = t.ExternalStandardUrl,
                                  ApprovedDate = t.ApprovalDateTimeDisplayString,
                                  ValidDate = t.ValidFromDateTimeDisplayString
                              }).FirstOrDefaultAsync();

                if(MyTerm != null)
                {
                    return Page();
                }
            }

            AllTerms = await (from t in _context.Term
                    join q in (from f in _context.UserFavorites
                                where f.ItemType.ToLower() == "term"
                                && f.UserId == MyUser.UserId
                                select new { f.ItemId })
                    on t.TermId equals q.ItemId into tmp
                    from rfi in tmp.DefaultIfEmpty()
                    orderby t.ApprovedYn.ToString() ?? "N" descending, t.LastUpdatedDateTime descending
                    select new TermsData
                    {
                        Id = t.TermId,
                        Name = t.Name,
                        Approved = t.ApprovedYn.ToString() ?? "N",
                        Favorite = rfi.ItemId == null ? "no" : "yes",
                        UpdateDate = t.LastUpdatedDateTimeDisplayString,
                        Summary = t.Summary ?? t.TechnicalDefinition,
                    }).ToListAsync();
            return Page();
        }

        public async Task<ActionResult> OnGetComments(int id)
        {
            ViewData["Comments"] = await (from c in _context.TermConversationMessage
                                          where c.TermConversation.TermId == id
                                          orderby c.TermConversationId descending, c.TermConversationMessageId ascending
                                          select new TermCommentsData
                                          {
                                              TermId = id,
                                              Date = c.PostDateTimeDisplayString,
                                              ConvId = c.TermConversationId,
                                              MessId = c.TermConversationMessageId,
                                              User = c.User.Fullname_Cust,
                                              Text = c.MessageText
                                          }).ToListAsync();
            ViewData["Id"] = id;
            ViewData["Permissions"] = UserHelpers.GetUserPermissions(_cache, _context, User.Identity.Name);
            return Partial("Details/_Comments");
        }

        public async Task<ActionResult> OnPostNewTerm()
        {
            // if invalid inputs redirect to list all
            if (!ModelState.IsValid || NewTerm.Name is null || NewTerm.Name == "")
            {
                if (NewTermLink.ReportObjectId > 0)
                {
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

                    return Partial("../Reports/_Terms");
                }
                
                return RedirectToPage("/Terms/Index");
            }

            // all data comes from the form, but we update 2 atribs
            NewTerm.UpdatedByUserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewTerm.LastUpdatedDateTime = DateTime.Now;
            NewTerm.HasExternalStandardYn = NewTerm.ExternalStandardUrl != null && NewTerm.ExternalStandardUrl.Length > 0 ? "Y" : "N";
            NewTerm.ValidFromDateTime = NewTerm.ValidFromDateTime ?? DateTime.Now;
            NewTerm.ValidToDateTime = DateTime.Parse("12/31/9999");
            NewTerm.ApprovedYn = "N";


            _context.Term.Add(NewTerm);
            await _context.SaveChangesAsync();

            if (NewTermLink.ReportObjectId > 0)
            {
                // create link to report, if report was specified.
                NewTermLink.TermId = NewTerm.TermId;
                _context.Add(NewTermLink);
                await _context.SaveChangesAsync();

                // update last update date on report.
                // If report doc does not exist we will create.
                if (!_context.ReportObjectDoc.Any(d => d.ReportObjectId == NewTermLink.ReportObjectId)){
                    _context.Add(new ReportObjectDoc {ReportObjectId = NewTermLink.ReportObjectId});
                    await _context.SaveChangesAsync();
                }

                _context.ReportObjectDoc.Where(d => d.ReportObjectId == NewTermLink.ReportObjectId).FirstOrDefault().LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();

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

                return Partial("../Reports/_Terms");
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Terms/Index", new { id = NewTerm.TermId });
        }

        public ActionResult OnPostDeleteTerm()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Terms/Index", new {id = NewTerm.TermId});
            }

            // delete:
            //  comments
            //  replys
            //  report links
            //  initiative term annotations

            _context.RemoveRange(_context.TermConversationMessage.Where(x => x.TermConversationId == _context.TermConversation.Where(g => g.TermId == NewTerm.TermId).Select(h => h.TermConversationId).FirstOrDefault()));
            _context.RemoveRange(_context.TermConversation.Where(x => x.TermId == NewTerm.TermId));
            _context.RemoveRange(_context.ReportObjectDocTerms.Where(x => x.TermId == NewTerm.TermId));
            _context.RemoveRange(_context.DpTermAnnotation.Where(x => x.TermId == NewTerm.TermId));
            _context.Remove(NewTerm);
            _context.SaveChanges();

            return RedirectToPage("/Terms/Index");
        }

        // edit initiatives
        public ActionResult OnPostEditTerm()
        {
            // we get a copy of the initiative and then will only update several fields.
            Term  EditedTerm = _context.Term.Where(m => m.TermId == NewTerm.TermId).FirstOrDefault();

            // only these values can be updated
            EditedTerm.Name = NewTerm.Name;
            EditedTerm.Summary = NewTerm.Summary;
            EditedTerm.TechnicalDefinition = NewTerm.TechnicalDefinition;
            EditedTerm.ValidFromDateTime = NewTerm.ValidFromDateTime;

            var now = DateTime.Now;
            // if status changed to approved then update approval date
            if ((EditedTerm.ApprovedYn == "N" || EditedTerm.ApprovedYn is null || EditedTerm.ApprovalDateTime is null) && NewTerm.ApprovedYn == "Y")
            {
                EditedTerm.ApprovalDateTime = now;
                EditedTerm.ApprovedByUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            }
            EditedTerm.ApprovedYn = NewTerm.ApprovedYn;

            // if there is an ext url
            if(NewTerm.ExternalStandardUrl != null)
            {
                if (NewTerm.ExternalStandardUrl.Length > 0)
                {
                    EditedTerm.HasExternalStandardYn = "Y";
                }
                else // the empty string (or null) was passed in - we also need to set the URL to null
                {
                    EditedTerm.HasExternalStandardYn = "N";
                    EditedTerm.ExternalStandardUrl = null;
                }
            }
            else // the empty string (or null) was passed in - we also need to set the URL to null
            {
                EditedTerm.HasExternalStandardYn = "N";
                EditedTerm.ExternalStandardUrl = null;
            }
            EditedTerm.ExternalStandardUrl = NewTerm.ExternalStandardUrl;

            EditedTerm.UpdatedByUserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            EditedTerm.LastUpdatedDateTime = now;

            // save updates
            _context.Attach(EditedTerm).State = EntityState.Modified;
            _context.SaveChanges();

            // redirect back to same initiative
            return RedirectToPage("/Terms/Index", new { id = NewTerm.TermId });
        }

        public async Task<ActionResult> OnPostNewComment()
        {
            if (!ModelState.IsValid || NewCommentReply.MessageText is null)
            {
                return RedirectToPage("/Terms/Index", new {id = NewComment.TermId});
            }

            // if missing the conversation ID then create a new conversation.
            if(!_context.TermConversation.Any(x => x.TermConversationId == NewComment.TermConversationId)){
                // create new comment
                _context.Add(NewComment);
            }
            // add message
            NewCommentReply.UserId = UserHelpers.GetUser(_cache, _context,User.Identity.Name).UserId;
            NewCommentReply.PostDateTime = System.DateTime.Now;
            NewCommentReply.TermConversationId = NewComment.TermConversationId;
            _context.Add(NewCommentReply);
            _context.SaveChanges();

            //return RedirectToPage("/Terms/Index", new { id = NewComment.TermId });
            return await OnGetComments(NewComment.TermId);
        }

        public async Task<ActionResult> OnPostDeleteComment()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Terms/Index", new {id = NewComment.TermId});
            }

            _context.Remove(NewCommentReply);
            _context.SaveChanges();

            return await OnGetComments(NewComment.TermId);
        }

        public async Task<ActionResult> OnPostDeleteCommentStream()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/Terms/Index", new {id = NewComment.TermId});
            }

            // first delete all replys on comment. Delete comment.
            _context.RemoveRange(_context.TermConversationMessage.Where(x => x.TermConversationId == NewComment.TermConversationId));
            _context.Remove(NewComment);
            _context.SaveChanges();

            return await OnGetComments(NewComment.TermId);
        }
    }
}