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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Pages.Terms
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

        public List<UserFavorite> Favorites { get; set; }
        public List<UserPreference> Preferences { get; set; }
        public User PublicUser { get; set; }

        public class ReportTermsData
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Summary { get; set; }
            public string Definition { get; set; }
            public int? ReportId { get; set; }
        }

        public class RelatedReportsData
        {
            public string Name { get; set; }
            public int? Id { get; set; }
            public string Url { get; set; }
        }

        public IEnumerable<TermsData> AllTerms { get; set; }

        public TermsData MyTerm { get; set; }

        [BindProperty]
        public Term NewTerm { get; set; }

        [BindProperty]
        public ReportObjectDocTerm NewTermLink { get; set; }

        public List<AdList> AdLists { get; set; }
        public List<int?> Permissions { get; set; }
        public List<RelatedReportsData> RelatedReports { get; set; }
        public List<RelatedReportsData> ParentRelatedReports { get; set; }
        public List<RelatedReportsData> GrandparentRelatedReports { get; set; }

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

            if (id != null)
            {
                // try to get Term by id
                MyTerm = await (
                    from t in _context.Terms
                    where t.TermId == id
                    join q in (
                        from f in _context.UserFavorites
                        where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                        select new { f.ItemId }
                    )
                        on t.TermId equals q.ItemId
                        into tmp
                    from rfi in tmp.DefaultIfEmpty()
                    orderby t.ApprovedYn ?? "N" descending,t.LastUpdatedDateTime descending
                    select new TermsData
                    {
                        Id = t.TermId,
                        Name = t.Name,
                        Approved = t.ApprovedYn ?? "N",
                        Favorite = rfi.ItemId == null ? "no" : "yes",
                        UpdateDate = t.LastUpdatedDateTimeDisplayString,
                        Summary = t.Summary,
                        Definition = t.TechnicalDefinition,
                        UpdatedBy = t.UpdatedByUser.Fullname_Cust,
                        ApprovedBy = t.ApprovedByUser.Fullname_Cust,
                        ExtStdUrl = t.ExternalStandardUrl,
                        ApprovedDate = t.ApprovalDateTimeDisplayString,
                        ValidDate = t.ValidFromDateTimeDisplayString
                    }
                ).FirstOrDefaultAsync();

                var related_reports = await (
                    from r in _context.ReportObjectDocTerms
                    where
                        r.TermId == id
                        && (r.ReportObject.Hidden ?? "N") == "N"
                        && r.ReportObject.ReportObject.DefaultVisibilityYn == "Y"
                    select new
                    {
                        Id = r.ReportObjectId,
                        Url = Helpers.HtmlHelpers.ReportUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            r.ReportObject.ReportObject,
                            _context,
                            User.Identity.Name
                        ),
                        Name = r.ReportObject.ReportObject.DisplayName
                    }
                ).ToListAsync();

                var parent_related_reports = await (
                    from r in _context.ReportObjectDocTerms
                    where r.TermId == id
                    // && (r.ReportObject.Hidden ?? "N") == "N"
                    // && r.ReportObject.ReportObject.DefaultVisibilityYn == "Y"
                    join p in _context.ReportObjectHierarchies
                        on r.ReportObjectId equals p.ChildReportObjectId
                    where
                        (p.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                        && p.ParentReportObject.DefaultVisibilityYn == "Y"
                    select new
                    {
                        Id = p.ParentReportObjectId,
                        Url = Helpers.HtmlHelpers.ReportUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            p.ParentReportObject,
                            _context,
                            User.Identity.Name
                        ),
                        Name = p.ParentReportObject.DisplayName
                    }
                ).Distinct().ToListAsync();

                var grandparent_related_reports = await (
                    from r in _context.ReportObjectDocTerms
                    where r.TermId == id
                    //   && (r.ReportObject.Hidden ?? "N") == "N"
                    //   && r.ReportObject.ReportObject.DefaultVisibilityYn == "Y"
                    join p in _context.ReportObjectHierarchies
                        on r.ReportObjectId equals p.ChildReportObjectId
                    // where (p.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                    //   && p.ParentReportObject.DefaultVisibilityYn == "Y"
                    join gp in _context.ReportObjectHierarchies
                        on p.ParentReportObjectId equals gp.ChildReportObjectId
                    where
                        (gp.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                        && gp.ParentReportObject.DefaultVisibilityYn == "Y"
                    select new
                    {
                        Id = gp.ParentReportObjectId,
                        Url = Helpers.HtmlHelpers.ReportUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            gp.ParentReportObject,
                            _context,
                            User.Identity.Name
                        ),
                        Name = gp.ParentReportObject.DisplayName
                    }
                ).Distinct().ToListAsync();

                var great_grandparent_related_reports = await (
                    from r in _context.ReportObjectDocTerms
                    where r.TermId == id
                    //   && (r.ReportObject.Hidden ?? "N") == "N"
                    //   && r.ReportObject.ReportObject.DefaultVisibilityYn == "Y"
                    join p in _context.ReportObjectHierarchies
                        on r.ReportObjectId equals p.ChildReportObjectId
                    //where (p.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                    //  && p.ParentReportObject.DefaultVisibilityYn == "Y"
                    join gp in _context.ReportObjectHierarchies
                        on p.ParentReportObjectId equals gp.ChildReportObjectId
                    //  where (gp.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                    //  && gp.ParentReportObject.DefaultVisibilityYn == "Y"
                    join ggp in _context.ReportObjectHierarchies
                        on gp.ParentReportObjectId equals ggp.ChildReportObjectId
                    where
                        (ggp.ParentReportObject.ReportObjectDoc.Hidden ?? "N") == "N"
                        && ggp.ParentReportObject.DefaultVisibilityYn == "Y"
                    select new
                    {
                        Id = ggp.ParentReportObjectId,
                        Url = Helpers.HtmlHelpers.ReportUrlFromParams(
                            _config["AppSettings:org_domain"],
                            HttpContext,
                            ggp.ParentReportObject,
                            _context,
                            User.Identity.Name
                        ),
                        Name = ggp.ParentReportObject.DisplayName
                    }
                ).Distinct().ToListAsync();

                RelatedReports = related_reports
                    .Union(parent_related_reports)
                    .Union(grandparent_related_reports)
                    .Union(great_grandparent_related_reports)
                    .Distinct()
                    .Select(
                        x =>
                            new RelatedReportsData
                            {
                                Id = x.Id,
                                Url = x.Url,
                                Name = x.Name,
                            }
                    )
                    .ToList();

                if (MyTerm != null)
                {
                    return Page();
                }
            }

            AllTerms = await (
                from t in _context.Terms
                join q in (
                    from f in _context.UserFavorites
                    where f.ItemType.ToLower() == "term" && f.UserId == MyUser.UserId
                    select new { f.ItemId }
                )
                    on t.TermId equals q.ItemId
                    into tmp
                from rfi in tmp.DefaultIfEmpty()
                orderby t.ApprovedYn ?? "N" descending,t.LastUpdatedDateTime descending
                select new TermsData
                {
                    Id = t.TermId,
                    Name = t.Name,
                    Approved = t.ApprovedYn ?? "N",
                    Favorite = rfi.ItemId == null ? "no" : "yes",
                    UpdateDate = t.LastUpdatedDateTimeDisplayString,
                    Summary = t.Summary ?? t.TechnicalDefinition,
                }
            ).ToListAsync();
            return Page();
        }

        public async Task<ActionResult> OnPostNewTerm()
        {
            // if invalid inputs redirect to list all
            if (!ModelState.IsValid || NewTerm.Name is null || NewTerm.Name == "")
            {
                if (NewTermLink.ReportObjectId > 0)
                {
                    ViewData["ReportTerms"] = await (
                        from r in _context.ReportObjectDocTerms
                        where r.ReportObjectId == NewTermLink.ReportObjectId
                        select new ReportTermsData
                        {
                            Name = r.Term.Name,
                            Id = r.TermId,
                            Summary = r.Term.Summary,
                            Definition = r.Term.TechnicalDefinition,
                            ReportId = r.ReportObjectId
                        }
                    ).ToListAsync();

                    //return Partial((".+?"));
                    return new PartialViewResult()
                    {
                        ViewName = "../Reports/Editor/_CurrentTerms",
                        ViewData = ViewData
                    };
                }

                return RedirectToPage("/Terms/Index");
            }

            // all data comes from the form, but we update 2 atribs
            NewTerm.UpdatedByUserId =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            NewTerm.LastUpdatedDateTime = DateTime.Now;
            NewTerm.HasExternalStandardYn =
                NewTerm.ExternalStandardUrl != null && NewTerm.ExternalStandardUrl.Length > 0
                    ? "Y"
                    : "N";
            NewTerm.ValidFromDateTime = NewTerm.ValidFromDateTime ?? DateTime.Now;
            NewTerm.ValidToDateTime = DateTime.Parse("12/31/9999");
            NewTerm.ApprovedYn = "N";

            _context.Terms.Add(NewTerm);
            await _context.SaveChangesAsync();

            if (NewTermLink.ReportObjectId > 0)
            {
                // create link to report, if report was specified.
                NewTermLink.TermId = NewTerm.TermId;
                _context.Add(NewTermLink);
                await _context.SaveChangesAsync();

                // update last update date on report.
                // If report doc does not exist we will create.
                if (
                    !_context.ReportObjectDocs.Any(
                        d => d.ReportObjectId == NewTermLink.ReportObjectId
                    )
                )
                {
                    _context.Add(
                        new ReportObjectDoc { ReportObjectId = NewTermLink.ReportObjectId }
                    );
                    await _context.SaveChangesAsync();
                }

                _context.ReportObjectDocs
                    .Where(d => d.ReportObjectId == NewTermLink.ReportObjectId)
                    .FirstOrDefault().LastUpdateDateTime = DateTime.Now;
                await _context.SaveChangesAsync();

                ViewData["ReportTerms"] = await (
                    from r in _context.ReportObjectDocTerms
                    where r.ReportObjectId == NewTermLink.ReportObjectId
                    select new ReportTermsData
                    {
                        Name = r.Term.Name,
                        Id = r.TermId,
                        Summary = r.Term.Summary,
                        Definition = r.Term.TechnicalDefinition,
                        ReportId = r.ReportObjectId
                    }
                ).ToListAsync();

                //return Partial((".+?"));
                return new PartialViewResult()
                {
                    ViewName = "../Reports/Editor/_CurrentTerms",
                    ViewData = ViewData
                };
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Terms/Index", new { id = NewTerm.TermId });
        }

        public ActionResult OnGetDeleteTerm(int Id)
        {
            if (!_context.Terms.Any(x => x.TermId == Id))
            {
                return RedirectToPage("/Terms/Index", new { id = Id });
            }

            // delete:
            //  comments
            //  replys
            //  report links
            //  initiative term annotations

            _context.RemoveRange(
                _context.TermConversationMessages.Where(
                    x =>
                        x.TermConversationId
                        == _context.TermConversations
                            .Where(g => g.TermId == Id)
                            .Select(h => h.TermConversationId)
                            .FirstOrDefault()
                )
            );
            _context.RemoveRange(_context.TermConversations.Where(x => x.TermId == Id));
            _context.RemoveRange(_context.ReportObjectDocTerms.Where(x => x.TermId == Id));
            _context.RemoveRange(_context.DpTermAnnotations.Where(x => x.TermId == Id));
            _context.Remove(_context.Terms.Where(x => x.TermId == Id).FirstOrDefault());
            _context.SaveChanges();

            return RedirectToPage("/Terms/Index");
        }

        // edit initiatives
        public ActionResult OnPostEditTerm()
        {
            // we get a copy of the initiative and then will only update several fields.
            Term EditedTerm = _context.Terms
                .Where(m => m.TermId == NewTerm.TermId)
                .FirstOrDefault();

            // only these values can be updated
            EditedTerm.Name = NewTerm.Name;
            EditedTerm.Summary = NewTerm.Summary;
            EditedTerm.TechnicalDefinition = NewTerm.TechnicalDefinition;
            EditedTerm.ValidFromDateTime = NewTerm.ValidFromDateTime;

            var now = DateTime.Now;
            // if status changed to approved then update approval date
            if (
                (
                    EditedTerm.ApprovedYn == "N"
                    || EditedTerm.ApprovedYn is null
                    || EditedTerm.ApprovalDateTime is null
                )
                && NewTerm.ApprovedYn == "Y"
            )
            {
                EditedTerm.ApprovalDateTime = now;
                EditedTerm.ApprovedByUser = UserHelpers.GetUser(
                    _cache,
                    _context,
                    User.Identity.Name
                );
            }
            EditedTerm.ApprovedYn = NewTerm.ApprovedYn;

            // if there is an ext url
            if (NewTerm.ExternalStandardUrl != null)
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

            EditedTerm.UpdatedByUserId =
                UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;
            EditedTerm.LastUpdatedDateTime = now;

            // save updates
            _context.Attach(EditedTerm).State = EntityState.Modified;
            _context.SaveChanges();

            // redirect back to same initiative
            return RedirectToPage("/Terms/Index", new { id = NewTerm.TermId });
        }
    }
}
