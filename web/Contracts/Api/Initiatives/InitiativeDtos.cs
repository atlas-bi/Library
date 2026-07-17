using System.ComponentModel.DataAnnotations;

namespace Atlas_Web.Contracts.Api.Initiatives;

public sealed class InitiativeListResponseDto
{
    public InitiativeFeatureFlagsDto Features { get; init; }
    public InitiativePermissionsDto Permissions { get; init; }
    public IReadOnlyList<InitiativeListItemDto> Items { get; init; } = Array.Empty<InitiativeListItemDto>();
}

public sealed class InitiativeListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
}

public sealed class InitiativeDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Hidden { get; init; }
    public DateTime? LastModified { get; init; }
    public string LastModifiedDisplay { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
    public bool CanCreateInitiative { get; init; }
    public bool CanEditInitiative { get; init; }
    public bool CanDeleteInitiative { get; init; }
    public bool CanViewUserProfiles { get; init; }
    public InitiativeFeatureFlagsDto Features { get; init; }
    public InitiativeUserSummaryDto OperationOwner { get; init; }
    public InitiativeUserSummaryDto ExecutiveOwner { get; init; }
    public InitiativeUserSummaryDto LastUpdatedBy { get; init; }
    public InitiativeLookupValueDto FinancialImpact { get; init; }
    public InitiativeLookupValueDto StrategicImportance { get; init; }
    public IReadOnlyList<InitiativeLinkedCollectionDto> Collections { get; init; } =
        Array.Empty<InitiativeLinkedCollectionDto>();
}

public sealed class InitiativeFeatureFlagsDto
{
    public bool UserProfilesEnabled { get; init; }
    public bool FeedbackEnabled { get; init; }
    public bool SharingEnabled { get; init; }
}

public sealed class InitiativePermissionsDto
{
    public bool CanCreateInitiative { get; init; }
    public bool CanEditInitiative { get; init; }
    public bool CanDeleteInitiative { get; init; }
    public bool CanViewUserProfiles { get; init; }
}

public sealed class InitiativeUserSummaryDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public sealed class InitiativeLookupValueDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}

public sealed class InitiativeLinkedCollectionDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}

public sealed class InitiativeCollectionSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}

public sealed class CreateInitiativeRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Description { get; init; }
    public int? OperationOwnerId { get; init; }
    public int? ExecutiveOwnerId { get; init; }
    public int? FinancialImpact { get; init; }
    public int? StrategicImportance { get; init; }
    public string Hidden { get; init; }
    public IReadOnlyList<int> CollectionIds { get; init; } = Array.Empty<int>();
}

public sealed class UpdateInitiativeRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Description { get; init; }
    public int? OperationOwnerId { get; init; }
    public int? ExecutiveOwnerId { get; init; }
    public int? FinancialImpact { get; init; }
    public int? StrategicImportance { get; init; }
    public string Hidden { get; init; }
    public IReadOnlyList<int> CollectionIds { get; init; } = Array.Empty<int>();
}
