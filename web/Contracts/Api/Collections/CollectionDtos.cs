using System.ComponentModel.DataAnnotations;

namespace Atlas_Web.Contracts.Api.Collections;

public sealed class CollectionListResponseDto
{
    public IReadOnlyList<CollectionListItemDto> Collections { get; init; } =
        Array.Empty<CollectionListItemDto>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed class CollectionListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Purpose { get; init; }
    public string Hidden { get; init; }
    public DateTime? LastModified { get; init; }
    public int StarCount { get; init; }
    public bool IsStarred { get; init; }
}

public sealed class CollectionDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Purpose { get; init; }
    public string Hidden { get; init; }
    public DateTime? LastModified { get; init; }
    public string LastModifiedDisplay { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
    public bool CanCreateCollection { get; init; }
    public bool CanEditCollection { get; init; }
    public bool CanDeleteCollection { get; init; }
    public bool CanViewUserProfiles { get; init; }
    public CollectionFeatureFlagsDto Features { get; init; }
    public CollectionUserSummaryDto LastUpdatedBy { get; init; }
    public InitiativeSummaryDto Initiative { get; init; }
    public IReadOnlyList<CollectionTermDto> Terms { get; init; } = Array.Empty<CollectionTermDto>();
    public IReadOnlyList<CollectionReportDto> Reports { get; init; } =
        Array.Empty<CollectionReportDto>();
}

public sealed class CollectionFeatureFlagsDto
{
    public bool TermsEnabled { get; init; }
    public bool UserProfilesEnabled { get; init; }
    public bool FeedbackEnabled { get; init; }
    public bool SharingEnabled { get; init; }
}

public sealed class CollectionUserSummaryDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public sealed class InitiativeSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}

public sealed class CollectionTermDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Summary { get; init; }
    public int? Rank { get; init; }
}

public sealed class CollectionReportDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public DateTime? LastModified { get; init; }
    public int AttachmentCount { get; init; }
    public int? Rank { get; init; }
    public bool CanRun { get; set; }
    public bool IsStarred { get; init; }
}

public sealed class CollectionSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}

public sealed class CreateCollectionRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Description { get; init; }
    public string Purpose { get; init; }
    public string Hidden { get; init; }
    public IReadOnlyList<int> TermIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> ReportIds { get; init; } = Array.Empty<int>();
}

public sealed class UpdateCollectionRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Description { get; init; }
    public string Purpose { get; init; }
    public string Hidden { get; init; }
    public IReadOnlyList<int> TermIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> ReportIds { get; init; } = Array.Empty<int>();
}
