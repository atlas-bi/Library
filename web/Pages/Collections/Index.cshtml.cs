using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using System.Collections.Generic;
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
        public DpDataProject Collection { get; set; }

        [BindProperty]
        public DpReportAnnotation DpReportAnnotation { get; set; }

        [BindProperty]
        public DpDataProjectConversation NewComment { get; set; }

        [BindProperty]
        public DpDataProjectConversationMessage NewCommentReply { get; set; }

        public List<int?> Permissions { get; set; }
        public IEnumerable<DpDataProject> Collections { get; set; }
        public List<UserFavorite> Favorites { get; set; }
        public IEnumerable<CollectionCommentsData> CollectionComments { get; set; }

        public List<AdList> AdLists { get; set; }

        public class CollectionCommentsData
        {
            public string Date { get; set; }
            public int? ConvId { get; set; }
            public int? MessId { get; set; }
            public string User { get; set; }
            public string Text { get; set; }
            public int CollectionId { get; set; }
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

        public class CollectionTermsData
        {
            public string Name { get; set; }
            public int? Id { get; set; }
            public string Summary { get; set; }
            public string Definition { get; set; }
            public int? ReportId { get; set; }
        }

        public List<UserPreference> Preferences { get; set; }
        public User PublicUser { get; set; }
        public string Favorite { get; set; }

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

            // if the id null then list all
            if (id != null)
            {
                Collection = await _context.DpDataProjects
                    .Include(x => x.LastUpdateUserNavigation)
                    .Include(x => x.DpTermAnnotations)
                    .ThenInclude(x => x.Term)
                    .Include(x => x.DpReportAnnotations)
                    .ThenInclude(x => x.Report)
                    .ThenInclude(x => x.ReportObjectDoc)
                    .Include(x => x.DpReportAnnotations)
                    .ThenInclude(x => x.Report)
                    .ThenInclude(x => x.ReportObjectType)
                    .SingleAsync(x => x.DataProjectId == id);

                Favorite = (
                    from f in _context.UserFavorites
                    where f.ItemType == "collection" && f.UserId == MyUser.UserId && f.ItemId == id
                    select new { f.ItemId }
                ).Any()
                  ? "yes"
                  : "no";

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
                    orderby r.Rank ,r.Report.Name
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

            Collections = await _context.DpDataProjects.ToListAsync();
            //Favorite = (
            //    from f in _context.UserFavorites
            //    where
            //        f.ItemType == "collection"
            //        && f.UserId == MyUser.UserId
            //        && f.ItemId == i.DataProjectId
            //    select new { f.ItemId }
            //).Any()
            //    ? "yes"
            //    : "no";
            //    }

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
                return RedirectToPage("/Collections/Index", new { id = Collection.DataProjectId });
            }

            // delete report annotations and term annotations
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

            _context.Remove(
                _context.DpDataProjects.Where(m => m.DataProjectId == Id).FirstOrDefault()
            );
            _context.SaveChanges();

            return RedirectToPage("/Collections/Index");
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
    }
}
