namespace Atlas_Web.Contracts.Api.Profile;

public sealed class ProfileChartResponseDto
{
    public int Runs { get; init; }
    public int Users { get; init; }
    public double RunTime { get; init; }
    public IReadOnlyList<ProfileRunHistoryPointDto> History { get; init; } =
        Array.Empty<ProfileRunHistoryPointDto>();
}

public sealed class ProfileRunHistoryPointDto
{
    public string Date { get; init; }
    public int Runs { get; init; }
    public int Users { get; init; }
    public double RunTime { get; init; }
}

public sealed class ProfileBarItemDto
{
    public string Key { get; init; }
    public string Href { get; init; }
    public string TitleOne { get; init; }
    public string TitleTwo { get; init; }
    public string Date { get; init; }
    public string DateTitle { get; init; }
    public double Count { get; init; }
    public double? Percent { get; init; }
}

public sealed class ProfileRunListItemDto
{
    public string Name { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public int Runs { get; init; }
    public string LastRun { get; init; }
}

public sealed class ProfileStarUserDto
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public sealed class ProfileSubscriptionDto
{
    public int Id { get; init; }
    public int? UserId { get; init; }
    public string UserName { get; init; }
    public string EmailList { get; init; }
    public string Description { get; init; }
    public string LastStatus { get; init; }
    public DateTime? LastRunTime { get; init; }
    public string SubscriptionTo { get; init; }
}

public sealed class ProfileFilterItemDto
{
    public string Value { get; init; }
    public string Label { get; init; }
    public int Count { get; init; }
}

public sealed class ProfileFiltersResponseDto
{
    public IReadOnlyList<ProfileFilterItemDto> Server { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> Database { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> MasterFile { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> Visible { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> Certification { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> Availability { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
    public IReadOnlyList<ProfileFilterItemDto> ReportType { get; init; } =
        Array.Empty<ProfileFilterItemDto>();
}
