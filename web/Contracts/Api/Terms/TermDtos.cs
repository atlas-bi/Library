using System.ComponentModel.DataAnnotations;

namespace Atlas_Web.Contracts.Api.Terms;

public sealed class TermsListDto
{
    public TermFeaturesDto Features { get; init; }
    public TermPermissionsDto Permissions { get; init; }
    public IReadOnlyList<TermListItemDto> Items { get; init; } = Array.Empty<TermListItemDto>();
}

public sealed class TermListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Summary { get; init; }
    public string TechnicalDefinition { get; init; }
    public string BodyText { get; init; }
    public string Url { get; init; }
    public bool IsApproved { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
}

public sealed class TermDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Summary { get; init; }
    public string TechnicalDefinition { get; init; }
    public bool IsApproved { get; init; }
    public string ApprovedYn { get; init; }
    public string ApprovalDateDisplay { get; init; }
    public string LastUpdatedDisplay { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
    public TermFeaturesDto Features { get; init; }
    public TermPermissionsDto Permissions { get; init; }
    public TermUserSummaryDto ApprovedBy { get; init; }
    public TermUserSummaryDto LastUpdatedBy { get; init; }
}

public sealed class TermFeaturesDto
{
    public bool UserProfilesEnabled { get; init; }
    public bool SharingEnabled { get; init; }
    public bool FeedbackEnabled { get; init; }
}

public sealed class TermPermissionsDto
{
    public bool CanCreateTerm { get; init; }
    public bool CanEditTerm { get; init; }
    public bool CanDeleteTerm { get; init; }
    public bool CanApproveTerm { get; init; }
    public bool CanViewUserProfiles { get; init; }
}

public sealed class TermUserSummaryDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public sealed class TermRelatedReportDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string BodyText { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public int AttachmentCount { get; init; }
    public bool CanRun { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
    public bool IsCertified { get; init; }
}

public sealed class CreateTermRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Summary { get; init; }
    public string TechnicalDefinition { get; init; }
    public string ApprovedYn { get; init; }
}

public sealed class UpdateTermRequestDto
{
    [Required]
    public string Name { get; init; }
    public string Summary { get; init; }
    public string TechnicalDefinition { get; init; }
    public string ApprovedYn { get; init; }
}
