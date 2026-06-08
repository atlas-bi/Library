namespace Atlas_Web.Contracts.Api.Reports;

public sealed class ReportListResponseDto
{
    public IReadOnlyList<ReportListItemDto> Reports { get; init; } = Array.Empty<ReportListItemDto>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed class ReportListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public DateTime? LastModified { get; init; }
    public bool CanRun { get; set; }
}

public sealed class ReportDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string DisplayTitle { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
    public string DetailedDescription { get; init; }
    public string TypeName { get; init; }
    public string TypeShortName { get; init; }
    public string Url { get; init; }
    public string EpicMasterFile { get; init; }
    public decimal? EpicRecordId { get; init; }
    public decimal? EpicReportTemplateId { get; init; }
    public string ReportServerPath { get; init; }
    public string Availability { get; init; }
    public bool VisibleInSearch { get; init; }
    public string OrphanedReportObjectYn { get; init; }
    public string RepositoryDescription { get; init; }
    public int? Runs { get; init; }
    public DateTime? LastModified { get; init; }
    public DateTime? LastLoadDate { get; init; }
    public bool CanRun { get; set; }
    public bool CanEditDocumentation { get; set; }
    public bool CanViewGroups { get; set; }
    public bool CanViewUserProfiles { get; set; }
    public bool IsStarred { get; init; }
    public string RunUrl { get; set; }
    public string RecordViewerUrl { get; set; }
    public string EditReportUrl { get; set; }
    public string ManageReportUrl { get; set; }
    public ReportFeatureFlagsDto Features { get; set; }
    public UserSummaryDto Author { get; init; }
    public UserSummaryDto LastModifiedBy { get; init; }
    public ReportDocumentDto Document { get; set; }
    public IReadOnlyList<ReportTagDto> HeaderTags { get; init; } = Array.Empty<ReportTagDto>();
    public IReadOnlyList<ReportObjectTagDto> ObjectTags { get; init; } =
        Array.Empty<ReportObjectTagDto>();
    public IReadOnlyList<ReportAttachmentDto> Attachments { get; init; } =
        Array.Empty<ReportAttachmentDto>();
    public IReadOnlyList<ReportImageDto> Images { get; init; } = Array.Empty<ReportImageDto>();
    public IReadOnlyList<GroupSummaryDto> Groups { get; set; } = Array.Empty<GroupSummaryDto>();
    public IReadOnlyList<CollectionSummaryDto> Collections { get; init; } =
        Array.Empty<CollectionSummaryDto>();
    public IReadOnlyList<ReportParameterDto> Parameters { get; set; } =
        Array.Empty<ReportParameterDto>();
    public IReadOnlyList<ReportQueryDto> Queries { get; set; } = Array.Empty<ReportQueryDto>();
    public IReadOnlyList<ReportQueryDto> ComponentQueries { get; set; } =
        Array.Empty<ReportQueryDto>();
    public IReadOnlyList<TermSummaryDto> Terms { get; set; } = Array.Empty<TermSummaryDto>();
    public IReadOnlyList<ReportLinkSummaryDto> Children { get; set; } =
        Array.Empty<ReportLinkSummaryDto>();
    public IReadOnlyList<ReportLinkSummaryDto> Parents { get; set; } =
        Array.Empty<ReportLinkSummaryDto>();
    public ReportMaintenanceStatusDto MaintenanceStatus { get; set; }
    public int StarCount { get; init; }
}

public sealed class ReportFeatureFlagsDto
{
    public bool TermsEnabled { get; init; }
    public bool UserProfilesEnabled { get; init; }
    public bool FeedbackEnabled { get; init; }
    public bool RequestAccessEnabled { get; init; }
    public bool SharingEnabled { get; init; }
}

public sealed class ReportDocumentDto
{
    public int ReportObjectId { get; init; }
    public string GitLabProjectUrl { get; init; }
    public string DeveloperDescription { get; init; }
    public string KeyAssumptions { get; init; }
    public string ExecutiveVisibilityYn { get; init; }
    public DateTime? LastUpdateDateTime { get; init; }
    public DateTime? CreatedDateTime { get; init; }
    public string EnabledForHyperspace { get; init; }
    public string DoNotPurge { get; init; }
    public string Hidden { get; init; }
    public string DeveloperNotes { get; init; }
    public LookupDto OrganizationalValue { get; init; }
    public LookupDto EstimatedRunFrequency { get; init; }
    public LookupDto Fragility { get; init; }
    public LookupDto MaintenanceSchedule { get; init; }
    public UserSummaryDto OperationalOwner { get; init; }
    public UserSummaryDto Requester { get; init; }
    public UserSummaryDto UpdatedBy { get; init; }
    public IReadOnlyList<LookupDto> FragilityTags { get; init; } = Array.Empty<LookupDto>();
    public IReadOnlyList<ReportMaintenanceLogDto> MaintenanceLogs { get; init; } =
        Array.Empty<ReportMaintenanceLogDto>();
    public IReadOnlyList<ReportServiceRequestDto> ServiceRequests { get; init; } =
        Array.Empty<ReportServiceRequestDto>();
}

public sealed class UserSummaryDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}

public sealed class LookupDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}

public sealed class ReportTagDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public int? Priority { get; init; }
    public string ShowInHeader { get; init; }
}

public sealed class ReportObjectTagDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int? Line { get; init; }
}

public sealed class ReportAttachmentDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Path { get; init; }
    public string Source { get; init; }
    public string Type { get; init; }
    public DateTime? CreationDate { get; init; }
    public string RunUrl { get; set; }
}

public sealed class ReportImageDto
{
    public int Id { get; init; }
    public int Ordinal { get; init; }
    public string Source { get; init; }
}

public sealed class GroupSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Type { get; init; }
}

public sealed class CollectionSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int? Rank { get; init; }
}

public sealed class ReportParameterDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Value { get; init; }
}

public sealed class ReportQueryDto
{
    public int Id { get; init; }
    public int ReportObjectId { get; init; }
    public string Name { get; init; }
    public string Language { get; init; }
    public string SourceServer { get; init; }
    public string Query { get; init; }
    public DateTime? LastLoadDate { get; init; }
}

public sealed class TermSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Summary { get; init; }
}

public sealed class ReportLinkSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public DateTime? LastModified { get; init; }
    public int AttachmentCount { get; init; }
}

public sealed class ReportMaintenanceLogDto
{
    public int Id { get; init; }
    public DateTime? MaintenanceDate { get; init; }
    public string Comment { get; init; }
    public LookupDto Status { get; init; }
    public UserSummaryDto Maintainer { get; init; }
}

public sealed class ReportServiceRequestDto
{
    public int Id { get; init; }
    public string TicketNumber { get; init; }
    public string Description { get; init; }
    public string TicketUrl { get; init; }
}

public sealed class ReportMaintenanceStatusDto
{
    public bool IsRequired { get; init; }
    public string Message { get; init; }
    public DateTime? LastMaintenanceDate { get; init; }
    public DateTime? NextMaintenanceDate { get; init; }
    public LookupDto Schedule { get; init; }
}

public sealed class ReportQueriesResponseDto
{
    public IReadOnlyList<ReportQueryDto> Queries { get; init; } = Array.Empty<ReportQueryDto>();
    public IReadOnlyList<ReportQueryDto> ComponentQueries { get; init; } =
        Array.Empty<ReportQueryDto>();
}

public sealed class ReportRelationshipsResponseDto
{
    public bool CanViewGroups { get; init; }
    public IReadOnlyList<GroupSummaryDto> Groups { get; init; } = Array.Empty<GroupSummaryDto>();
    public IReadOnlyList<CollectionSummaryDto> Collections { get; init; } =
        Array.Empty<CollectionSummaryDto>();
    public IReadOnlyList<ReportLinkSummaryDto> Children { get; init; } =
        Array.Empty<ReportLinkSummaryDto>();
    public IReadOnlyList<ReportLinkSummaryDto> Parents { get; init; } =
        Array.Empty<ReportLinkSummaryDto>();
}

public sealed class UpdateReportDocumentRequestDto
{
    public string GitLabProjectUrl { get; init; }
    public string DeveloperDescription { get; init; }
    public string KeyAssumptions { get; init; }
    public int? OperationalOwnerUserId { get; init; }
    public int? RequesterUserId { get; init; }
    public int? OrganizationalValueId { get; init; }
    public int? EstimatedRunFrequencyId { get; init; }
    public int? FragilityId { get; init; }
    public string ExecutiveVisibilityYn { get; init; }
    public int? MaintenanceScheduleId { get; init; }
    public string EnabledForHyperspace { get; init; }
    public string DoNotPurge { get; init; }
    public string Hidden { get; init; }
    public string DeveloperNotes { get; init; }
    public IReadOnlyList<int> TermIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> CollectionIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> FragilityTagIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> ImageIds { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> ServiceRequestIds { get; init; } = Array.Empty<int>();
    public NewReportServiceRequestDto NewServiceRequest { get; init; }
    public NewMaintenanceLogDto NewMaintenanceLog { get; init; }
}

public sealed class NewReportServiceRequestDto
{
    public string TicketNumber { get; init; }
    public string Description { get; init; }
    public string TicketUrl { get; init; }
}

public sealed class NewMaintenanceLogDto
{
    public int? MaintenanceLogStatusId { get; init; }
    public string Comment { get; init; }
}

public sealed class ReportSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}
