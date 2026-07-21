using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Terms;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public sealed partial class TermsApiService
{
    public async Task<TermDetailDto> CreateTermAsync(
        ClaimsPrincipal user,
        CreateTermRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var canApprove = user.HasPermission("Approve Terms");
        var approved = canApprove && IsApproved(request.ApprovedYn);

        var term = new Term
        {
            Name = request.Name.Trim(),
            Summary = request.Summary,
            TechnicalDefinition = request.TechnicalDefinition,
            ApprovedYn = approved ? "Y" : "N",
            UpdatedByUserId = currentUserId,
            LastUpdatedDateTime = DateTime.Now,
            ApprovedByUserId = approved ? currentUserId : null,
            ApprovalDateTime = approved ? DateTime.Now : null,
        };

        await _context.Terms.AddAsync(term, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateTermCaches(term.TermId);

        return await GetTermAsync(user, term.TermId, cancellationToken);
    }

    public async Task<TermDetailDto> UpdateTermAsync(
        ClaimsPrincipal user,
        int id,
        UpdateTermRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var canApprove = user.HasPermission("Approve Terms");
        var term = await _context.Terms.SingleOrDefaultAsync(x => x.TermId == id, cancellationToken);
        if (term == null)
        {
            return null;
        }

        term.Name = request.Name.Trim();
        term.Summary = request.Summary;
        term.TechnicalDefinition = request.TechnicalDefinition;
        term.UpdatedByUserId = currentUserId;
        term.LastUpdatedDateTime = DateTime.Now;

        if (canApprove && IsApproved(request.ApprovedYn))
        {
            term.ApprovedYn = "Y";
            term.ApprovalDateTime ??= DateTime.Now;
            term.ApprovedByUserId ??= currentUserId;
        }
        else
        {
            term.ApprovedYn = "N";
            term.ApprovalDateTime = null;
            term.ApprovedByUserId = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        InvalidateTermCaches(id);

        return await GetTermAsync(user, id, cancellationToken);
    }

    public async Task DeleteTermAsync(int id, CancellationToken cancellationToken)
    {
        var reportLinks = await _context.ReportObjectDocTerms.Where(x => x.TermId == id)
            .ToListAsync(cancellationToken);
        var collectionLinks = await _context.CollectionTerms.Where(x => x.TermId == id)
            .ToListAsync(cancellationToken);
        var starredLinks = await _context.StarredTerms.Where(x => x.Termid == id)
            .ToListAsync(cancellationToken);
        var term = await _context.Terms.SingleOrDefaultAsync(x => x.TermId == id, cancellationToken);

        _context.ReportObjectDocTerms.RemoveRange(reportLinks);
        _context.CollectionTerms.RemoveRange(collectionLinks);
        _context.StarredTerms.RemoveRange(starredLinks);
        if (term != null)
        {
            _context.Terms.Remove(term);
        }

        await _context.SaveChangesAsync(cancellationToken);
        InvalidateTermCaches(id);
    }
}
