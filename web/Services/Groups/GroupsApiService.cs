using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Groups;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Services;

public interface IGroupsApiService
{
    Task<GroupListResponseDto> GetGroupsAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    Task<GroupDetailDto> GetGroupAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<GroupUserDto>> GetGroupUsersAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<GroupReportDto>> GetGroupReportsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
}

public sealed class GroupsApiService : IGroupsApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _configuration;

    public GroupsApiService(
        Atlas_WebContext context,
        IConfiguration configuration,
        IMemoryCache cache
    )
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<GroupListResponseDto> GetGroupsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.UserGroups.AsNoTracking()
            .OrderBy(x => x.GroupName)
            .Select(x => new GroupListItemDto
            {
                Id = x.GroupId,
                Name = x.GroupName,
                Email = x.GroupEmail,
                Type = x.GroupType,
                Source = x.GroupSource,
                Url = "/groups?id=" + x.GroupId,
            })
            .ToListAsync(cancellationToken);

        return new GroupListResponseDto
        {
            Features = BuildFeatures(),
            Permissions = BuildPermissions(user),
            Items = items,
        };
    }

    public async Task<GroupDetailDto> GetGroupAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var group = await _context.UserGroups.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GroupId == id, cancellationToken);

        if (group == null)
        {
            return null;
        }

        return new GroupDetailDto
        {
            Id = group.GroupId,
            Name = group.GroupName,
            Email = group.GroupEmail,
            Type = group.GroupType,
            Source = group.GroupSource,
            Features = BuildFeatures(),
            Permissions = BuildPermissions(user),
        };
    }

    public async Task<IReadOnlyList<GroupUserDto>> GetGroupUsersAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!await GroupExistsAsync(id, cancellationToken))
        {
            return null;
        }

        var canViewUserProfiles = user.HasPermission("View Other User") && IsUserProfileEnabled();

        return await _context.UserGroupsMemberships.AsNoTracking()
            .Where(x => x.GroupId == id)
            .OrderBy(x => x.User.FullnameCalc)
            .Select(x => new GroupUserDto
            {
                Id = x.UserId,
                Name = x.User.FullnameCalc,
                Email = x.User.Email,
                Phone = x.User.Phone,
                EpicId = x.User.EpicId,
                EmployeeId = x.User.EmployeeId,
                CanOpenUserProfile = canViewUserProfiles,
                Url = canViewUserProfiles ? "/users?id=" + x.UserId : null,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupReportDto>> GetGroupReportsAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!await GroupExistsAsync(id, cancellationToken))
        {
            return null;
        }

        return await _context.ReportGroupsMemberships.AsNoTracking()
            .Where(x => x.GroupId == id)
            .OrderBy(x => x.Report.DisplayTitle ?? x.Report.Name)
            .Select(x => new GroupReportDto
            {
                Id = x.ReportId,
                Name = x.Report.DisplayTitle ?? x.Report.Name,
                LastUpdated = x.Report.LastUpdatedDateDisplayString,
                SubscriptionCount = x.Report.ReportObjectSubscriptions.Count,
                FavoriteCount = x.Report.StarredReports.Count,
                RunCount = x.Report.ReportObjectRunDataBridges.Sum(y => y.Runs),
                Url = "/reports?id=" + x.ReportId,
            })
            .ToListAsync(cancellationToken);
    }

    private GroupFeatureFlagsDto BuildFeatures()
    {
        return new GroupFeatureFlagsDto
        {
            UserProfilesEnabled = IsUserProfileEnabled(),
        };
    }

    private static GroupPermissionsDto BuildPermissions(ClaimsPrincipal user)
    {
        return new GroupPermissionsDto
        {
            CanViewGroups = user.HasPermission("View Groups"),
            CanViewUserProfiles = user.HasPermission("View Other User"),
            CanViewSiteAnalytics = user.HasPermission("View Site Analytics"),
        };
    }

    private bool IsUserProfileEnabled()
    {
        var value = _configuration["features:enable_user_profile"];
        return string.IsNullOrWhiteSpace(value)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private Task<bool> GroupExistsAsync(int id, CancellationToken cancellationToken)
    {
        return _context.UserGroups.AsNoTracking().AnyAsync(x => x.GroupId == id, cancellationToken);
    }
}
