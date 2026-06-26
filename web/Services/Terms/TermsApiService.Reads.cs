using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Terms;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public sealed partial class TermsApiService
{
    public async Task<TermsListDto> GetTermsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var features = BuildFeatures();
        var permissions = new TermPermissionsDto
        {
            CanCreateTerm = user.HasPermission("Create New Terms"),
            CanApproveTerm = user.HasPermission("Approve Terms"),
            CanEditTerm = false,
            CanDeleteTerm = false,
            CanViewUserProfiles = user.HasPermission("View Other User"),
        };

        var items = await _context.Terms.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TermListItemDto
            {
                Id = x.TermId,
                Name = x.Name,
                Summary = x.Summary,
                TechnicalDefinition = x.TechnicalDefinition,
                Url = "/terms?id=" + x.TermId,
                IsApproved = (x.ApprovedYn ?? "N") == "Y",
                IsStarred = x.StarredTerms.Any(y => y.Ownerid == currentUserId),
                StarCount = x.StarredTerms.Count,
                BodyText =
                    !string.IsNullOrEmpty(x.Summary)
                        ? TruncateWithReadMore(x.Summary)
                        : TruncateWithReadMore(x.TechnicalDefinition),
            })
            .ToListAsync(cancellationToken);

        return new TermsListDto
        {
            Features = features,
            Permissions = permissions,
            Items = items,
        };
    }

    public async Task<TermDetailDto> GetTermAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var term = await _context.Terms.AsNoTracking()
            .Include(x => x.ApprovedByUser)
            .Include(x => x.UpdatedByUser)
            .Include(x => x.StarredTerms)
            .SingleOrDefaultAsync(x => x.TermId == id, cancellationToken);

        if (term == null)
        {
            return null;
        }

        return new TermDetailDto
        {
            Id = term.TermId,
            Name = term.Name,
            Summary = term.Summary,
            TechnicalDefinition = term.TechnicalDefinition,
            IsApproved = IsApproved(term.ApprovedYn),
            ApprovedYn = IsApproved(term.ApprovedYn) ? "Y" : "N",
            ApprovalDateDisplay = term.ApprovalDateTimeDisplayString,
            LastUpdatedDisplay = term.LastUpdatedDateTimeDisplayString,
            IsStarred = term.StarredTerms.Any(x => x.Ownerid == currentUserId),
            StarCount = term.StarredTerms.Count,
            Features = BuildFeatures(),
            Permissions = BuildPermissions(user, term),
            ApprovedBy = ToUserSummary(term.ApprovedByUser),
            LastUpdatedBy = ToUserSummary(term.UpdatedByUser),
        };
    }

    public async Task<IReadOnlyList<TermRelatedReportDto>> GetTermReportsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var directReports = await LoadRelatedReportsAtDepthAsync(id, 1, cancellationToken);
        var parentReports = await LoadRelatedReportsAtDepthAsync(id, 2, cancellationToken);
        var grandParentReports = await LoadRelatedReportsAtDepthAsync(id, 3, cancellationToken);
        var fourthLevelReports = await LoadRelatedReportsAtDepthAsync(id, 4, cancellationToken);

        var relatedReports = directReports
            .Concat(parentReports)
            .Concat(grandParentReports)
            .Concat(fourthLevelReports)
            .GroupBy(x => x.ReportObjectId)
            .Select(x => x.First())
            .OrderBy(x => x.DisplayTitle ?? x.Name)
            .ToList();

        return await BuildRelatedReportDtosAsync(user, relatedReports);
    }

    private async Task<List<ReportObject>> LoadRelatedReportsAtDepthAsync(
        int termId,
        int depth,
        CancellationToken cancellationToken
    )
    {
        IQueryable<ReportObject> query = _context.ReportObjects.AsNoTracking();

        query = depth switch
        {
            1 => query.Where(x => x.ReportObjectDoc.ReportObjectDocTerms.Any(y => y.TermId == termId)),
            2 => query.Where(x => x.ReportObjectHierarchyParentReportObjects.Any(y =>
                y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(z => z.TermId == termId))),
            3 => query.Where(x => x.ReportObjectHierarchyParentReportObjects.Any(g =>
                g.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(y =>
                    y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(z => z.TermId == termId)))),
            4 => query.Where(x => x.ReportObjectHierarchyParentReportObjects.Any(gg =>
                gg.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(g =>
                    g.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(y =>
                        y.ChildReportObject.ReportObjectDoc.ReportObjectDocTerms.Any(z => z.TermId == termId))))),
            _ => query.Where(_ => false),
        };

        return await query
            .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
            .Where(x => x.DefaultVisibilityYn == "Y")
            .Include(x => x.ReportObjectType)
            .Include(x => x.ReportObjectDoc)
            .Include(x => x.ReportObjectAttachments)
            .Include(x => x.ReportTagLinks)
                .ThenInclude(x => x.Tag)
            .Include(x => x.StarredReports)
            .Include(x => x.ReportGroupsMemberships)
            .Include(x => x.ReportObjectHierarchyChildReportObjects)
                .ThenInclude(x => x.ParentReportObject)
                    .ThenInclude(x => x.ReportGroupsMemberships)
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<TermRelatedReportDto>> BuildRelatedReportDtosAsync(
        ClaimsPrincipal user,
        IReadOnlyList<ReportObject> reports
    )
    {
        var currentUserId = user.GetUserId();
        var result = new List<TermRelatedReportDto>(reports.Count);

        foreach (var report in reports)
        {
            var canRun = false;
            if (_authorizationService != null)
            {
                canRun = (await _authorizationService.AuthorizeAsync(
                    user,
                    report,
                    "ReportRunPolicy"
                )).Succeeded;
            }

            result.Add(
                new TermRelatedReportDto
                {
                    Id = report.ReportObjectId,
                    Name = report.DisplayTitle ?? report.DisplayName ?? report.Name,
                    Description = report.Description,
                    BodyText =
                        !string.IsNullOrEmpty(report.ReportObjectDoc?.DeveloperDescription)
                            ? TruncateWithReadMore(report.ReportObjectDoc.DeveloperDescription)
                            : TruncateWithReadMore(report.Description),
                    Type = string.IsNullOrEmpty(report.ReportObjectType?.ShortName)
                        ? report.ReportObjectType?.Name
                        : report.ReportObjectType.ShortName,
                    Url = "/reports?id=" + report.ReportObjectId,
                    AttachmentCount = report.ReportObjectAttachments.Count,
                    CanRun = canRun,
                    IsStarred = report.StarredReports.Any(x => x.Ownerid == currentUserId),
                    StarCount = report.StarredReports.Count,
                    IsCertified = report.ReportTagLinks.Any(x =>
                        x.Tag.Name == "Analytics Certified" || x.Tag.Name == "Analytics Reviewed"),
                }
            );
        }

        return result;
    }
}
