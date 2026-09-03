namespace Atlas_Web.Contracts.Api.Settings;

public sealed class SettingValueDto
{
    public string Value { get; set; }
}

public sealed class SiteMessageDto
{
    public int Id { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
}

public sealed class SiteMessageRequestDto
{
    public string Value { get; set; }
    public string Description { get; set; }
}

public sealed class SearchSettingsDto
{
    public IReadOnlyDictionary<string, string> Visibility { get; set; }
    public IReadOnlyList<SearchReportTypeDto> ReportTypes { get; set; }
}

public sealed class SearchReportTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public bool Visible { get; set; }
}

public sealed class SearchVisibilityRequestDto
{
    public bool Visible { get; set; }
}

public sealed class SearchTextRequestDto
{
    public string Text { get; set; }
}

public sealed class ParameterDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Used { get; set; }
}

public sealed class ParameterRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public sealed class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<PermissionDto> Permissions { get; set; }
}

public sealed class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public sealed class RoleRequestDto
{
    public string Name { get; set; }
}

public sealed class RolePermissionRequestDto
{
    public bool Enabled { get; set; }
}

public sealed class UserRoleAssignmentDto
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<PermissionDto> Roles { get; set; }
}

public sealed class GroupRoleAssignmentDto
{
    public int GroupId { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<PermissionDto> Roles { get; set; }
}

public sealed class RoleAssignmentRequestDto
{
    public int RoleId { get; set; }
}
