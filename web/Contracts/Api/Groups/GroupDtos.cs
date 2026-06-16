namespace Atlas_Web.Contracts.Api.Groups;

public sealed class GroupListResponseDto
{
    public GroupFeatureFlagsDto Features { get; init; }
    public GroupPermissionsDto Permissions { get; init; }
    public IReadOnlyList<GroupListItemDto> Items { get; init; } = Array.Empty<GroupListItemDto>();
}

public sealed class GroupListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Type { get; init; }
    public string Source { get; init; }
    public string Url { get; init; }
}

public sealed class GroupDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Type { get; init; }
    public string Source { get; init; }
    public GroupFeatureFlagsDto Features { get; init; }
    public GroupPermissionsDto Permissions { get; init; }
}

public sealed class GroupFeatureFlagsDto
{
    public bool UserProfilesEnabled { get; init; }
}

public sealed class GroupPermissionsDto
{
    public bool CanViewGroups { get; init; }
    public bool CanViewUserProfiles { get; init; }
    public bool CanViewSiteAnalytics { get; init; }
}

public sealed class GroupUserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public string EpicId { get; init; }
    public string EmployeeId { get; init; }
    public bool CanOpenUserProfile { get; init; }
    public string Url { get; init; }
}

public sealed class GroupReportDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string LastUpdated { get; init; }
    public int SubscriptionCount { get; init; }
    public int FavoriteCount { get; init; }
    public int RunCount { get; init; }
    public string Url { get; init; }
}
