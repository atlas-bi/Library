using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Terms;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Services;

public interface ITermsApiService
{
    Task<TermsListDto> GetTermsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    Task<TermDetailDto> GetTermAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TermRelatedReportDto>> GetTermReportsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<TermDetailDto> CreateTermAsync(
        ClaimsPrincipal user,
        CreateTermRequestDto request,
        CancellationToken cancellationToken
    );
    Task<TermDetailDto> UpdateTermAsync(
        ClaimsPrincipal user,
        int id,
        UpdateTermRequestDto request,
        CancellationToken cancellationToken
    );
    Task DeleteTermAsync(int id, CancellationToken cancellationToken);
}

public sealed partial class TermsApiService : ITermsApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly IAuthorizationService _authorizationService;

    public TermsApiService(
        Atlas_WebContext context,
        IConfiguration configuration,
        IMemoryCache cache,
        IAuthorizationService authorizationService = null
    )
    {
        _context = context;
        _configuration = configuration;
        _cache = cache;
        _authorizationService = authorizationService;
    }

    private bool IsFeatureEnabled(string key)
    {
        var value = _configuration[key];
        return string.IsNullOrWhiteSpace(value)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private TermFeaturesDto BuildFeatures()
    {
        return new TermFeaturesDto
        {
            UserProfilesEnabled = IsFeatureEnabled("features:enable_user_profile"),
            SharingEnabled = IsFeatureEnabled("features:enable_sharing"),
            FeedbackEnabled = IsFeatureEnabled("features:enable_feedback"),
        };
    }

    private TermPermissionsDto BuildPermissions(ClaimsPrincipal user, Term term)
    {
        var isApproved = string.Equals(term.ApprovedYn, "Y", StringComparison.OrdinalIgnoreCase);

        return new TermPermissionsDto
        {
            CanCreateTerm = user.HasPermission("Create New Terms"),
            CanEditTerm = isApproved
                ? user.HasPermission("Edit Approved Terms")
                : user.HasPermission("Edit Unapproved Terms"),
            CanDeleteTerm = isApproved
                ? user.HasPermission("Delete Approved Terms")
                : user.HasPermission("Delete Unapproved Terms"),
            CanApproveTerm = user.HasPermission("Approve Terms"),
            CanViewUserProfiles = user.HasPermission("View Other User"),
        };
    }

    private static TermUserSummaryDto ToUserSummary(User user)
    {
        if (user == null)
        {
            return null;
        }

        return new TermUserSummaryDto
        {
            Id = user.UserId,
            Username = user.Username,
            FullName = user.FullnameCalc ?? user.FullName ?? user.DisplayName,
            Email = user.Email,
        };
    }

    private static string TruncateWithReadMore(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "Open to view details.";
        }

        var trimmed = text.Trim();
        return trimmed.Substring(0, Math.Min(160, trimmed.Length)) + "... ";
    }

    private static bool IsApproved(string approvedYn)
    {
        return string.Equals(approvedYn, "Y", StringComparison.OrdinalIgnoreCase);
    }

    private void InvalidateTermCaches(int termId)
    {
        _cache.Remove("terms");
        _cache.Remove("term-" + termId);
        _cache.Remove("term-reports-" + termId);
    }
}
