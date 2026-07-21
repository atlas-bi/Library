using System.ComponentModel.DataAnnotations;

namespace Atlas_Web.Contracts.Api.Users;

public sealed class UserPageDto
{
    public UserPageUserDto User { get; init; }
    public UserPageViewerDto Viewer { get; init; }
    public UserPagePermissionsDto Permissions { get; init; }
    public UserPageTabsDto Tabs { get; init; }
    public UserPageFeaturesDto Features { get; init; }
    public IReadOnlyList<int> DefaultReportTypeIds { get; init; } = Array.Empty<int>();
}

public sealed class UserPageUserDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string FullName { get; init; }
    public string FirstName { get; init; }
    public string DisplayName { get; init; }
    public string Email { get; init; }
    public string Department { get; init; }
    public string Title { get; init; }
    public string Phone { get; init; }
    public string ProfilePhoto { get; init; }
}

public sealed class UserPageViewerDto
{
    public int Id { get; init; }
    public bool IsCurrentUser { get; init; }
    public bool IsAdministrator { get; init; }
    public string AdminEnabled { get; init; }
}

public sealed class UserPagePermissionsDto
{
    public bool CanViewOtherUsers { get; init; }
    public bool CanViewGroups { get; init; }
    public bool CanViewAnalytics { get; init; }
    public bool CanEditOtherUsers { get; init; }
    public bool CanToggleAdminMode { get; init; }
    public bool CanEditWorkspace { get; init; }
}

public sealed class UserPageTabsDto
{
    public bool StarsVisible { get; init; }
    public bool SubscriptionsVisible { get; init; }
    public bool ActivityVisible { get; init; }
    public bool RunListVisible { get; init; }
    public bool AtlasHistoryVisible { get; init; }
    public bool GroupsVisible { get; init; }
    public bool AnalyticsVisible { get; init; }
}

public sealed class UserPageFeaturesDto
{
    public bool UserProfilesEnabled { get; init; }
}

public sealed class UserGroupDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string Source { get; init; }
}

public sealed class UserSubscriptionDto
{
    public int? ReportId { get; init; }
    public string Name { get; init; }
    public string EmailList { get; init; }
    public string Description { get; init; }
    public string LastStatus { get; init; }
    public string LastRun { get; init; }
    public string SentTo { get; init; }
}

public sealed class UserHistorySectionDto
{
    public IReadOnlyList<UserHistoryItemDto> AtlasHistory { get; init; } =
        Array.Empty<UserHistoryItemDto>();
    public IReadOnlyList<UserHistoryItemDto> ReportEdits { get; init; } =
        Array.Empty<UserHistoryItemDto>();
    public IReadOnlyList<UserHistoryItemDto> InitiativeEdits { get; init; } =
        Array.Empty<UserHistoryItemDto>();
    public IReadOnlyList<UserHistoryItemDto> CollectionEdits { get; init; } =
        Array.Empty<UserHistoryItemDto>();
    public IReadOnlyList<UserHistoryItemDto> TermEdits { get; init; } =
        Array.Empty<UserHistoryItemDto>();
}

public sealed class UserHistoryItemDto
{
    public string Name { get; init; }
    public string Type { get; init; }
    public string Url { get; init; }
    public string Date { get; init; }
}

public sealed class UserSharedObjectsDto
{
    public IReadOnlyList<UserSharedObjectDto> SharedToMe { get; init; } =
        Array.Empty<UserSharedObjectDto>();
    public IReadOnlyList<UserSharedObjectDto> SharedFromMe { get; init; } =
        Array.Empty<UserSharedObjectDto>();
}

public sealed class UserSharedObjectDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string ShareDate { get; init; }
    public string SharedFrom { get; init; }
    public string Url { get; init; }
}

public sealed class UserSearchHistoryItemDto
{
    public string SearchUrl { get; init; }
    public string SearchString { get; init; }
}

public sealed class UserStarsDto
{
    public int UserId { get; init; }
    public int ViewerUserId { get; init; }
    public bool IsCurrentUser { get; init; }
    public bool CanEditWorkspace { get; init; }
    public UserWorkspacePermissionsDto Permissions { get; init; }
    public UserWorkspaceSummaryDto Summary { get; init; }
    public UserWorkspaceFilterStateDto Filters { get; init; }
    public IReadOnlyList<UserFavoriteFolderDto> Folders { get; init; } =
        Array.Empty<UserFavoriteFolderDto>();
    public IReadOnlyList<UserFavoriteItemDto> Items { get; init; } =
        Array.Empty<UserFavoriteItemDto>();
    public IReadOnlyList<UserSuggestedReportDto> SuggestedReports { get; init; } =
        Array.Empty<UserSuggestedReportDto>();
}

public sealed class UserWorkspacePermissionsDto
{
    public bool CanCreateFolders { get; init; }
    public bool CanRenameFolders { get; init; }
    public bool CanDeleteFolders { get; init; }
    public bool CanReorderFolders { get; init; }
    public bool CanReorderFavorites { get; init; }
    public bool CanMoveFavoritesToFolders { get; init; }
    public bool CanToggleFavorites { get; init; }
}

public sealed class UserWorkspaceSummaryDto
{
    public int TotalCount { get; init; }
    public int UnsortedCount { get; init; }
    public bool HasFolders { get; init; }
    public bool ShowUnsortedBucket { get; init; }
}

public sealed class UserWorkspaceFilterStateDto
{
    public bool HasReports { get; init; }
    public bool HasCollections { get; init; }
    public bool HasInitiatives { get; init; }
    public bool HasTerms { get; init; }
    public bool HasUsers { get; init; }
    public bool HasGroups { get; init; }
    public bool HasSearches { get; init; }
    public bool ShowQuickFilters { get; init; }
}

public sealed class UserFavoriteFolderDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int? Rank { get; init; }
    public int ItemCount { get; init; }
    public bool CanManage { get; init; }
    public bool CanReorder { get; init; }
}

public sealed class UserFavoriteItemDto
{
    public int StarId { get; init; }
    public string Type { get; init; }
    public string TypeLabel { get; init; }
    public int? FolderId { get; init; }
    public int? Rank { get; init; }
    public int? ItemId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Url { get; init; }
    public string SecondaryText { get; init; }
    public string FolderName { get; init; }
    public int? FolderRank { get; init; }
    public string SearchString { get; init; }
    public bool CanReorder { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
    public string BodyText { get; init; }
    public string PlaceholderImageUrl { get; init; }
    public string ThumbnailUrl { get; init; }
    public string FullImageUrl { get; init; }
    public bool IsCertified { get; init; }
    public bool IsApproved { get; init; }
    public bool CanOpenProfile { get; init; }
    public string ProfileTargetId { get; init; }
    public bool CanShare { get; init; }
    public string ShareTargetId { get; init; }
    public string ShareName { get; init; }
    public string ShareType { get; init; }
    public bool CanRequestAccess { get; init; }
    public string RequestAccessTargetId { get; init; }
    public bool CanRun { get; init; }
    public string RunUrl { get; init; }
    public bool OpensRunModal { get; init; }
    public string RunModalTargetId { get; init; }
    public string RunDisabledReason { get; init; }
    public bool CanEditInEditor { get; init; }
    public string EditUrl { get; init; }
    public bool CanManageInEditor { get; init; }
    public string ManageUrl { get; init; }
    public string ReportObjectUrl { get; init; }
    public string ReportServerPath { get; init; }
    public string SourceServer { get; init; }
    public string EpicMasterFile { get; init; }
    public decimal? EpicRecordId { get; init; }
    public decimal? EpicReportTemplateId { get; init; }
    public string EnabledForHyperspace { get; init; }
    public IReadOnlyList<UserFavoriteTagDto> Tags { get; init; } = Array.Empty<UserFavoriteTagDto>();
    public IReadOnlyList<string> RelatedCollectionNames { get; init; } = Array.Empty<string>();
}

public sealed class UserFavoriteTagDto
{
    public string Name { get; init; }
    public string Slug { get; init; }
    public bool ShowInHeader { get; init; }
}

public sealed class UserSuggestedReportDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Url { get; init; }
    public string Type { get; init; }
}

public sealed class CreateUserFavoriteFolderRequestDto
{
    [Required]
    public string Name { get; init; }
}

public sealed class UpdateUserFavoriteFolderRequestDto
{
    [Required]
    public string Name { get; init; }
}

public sealed class ReorderUserFavoriteFolderItemDto
{
    [Required]
    public string FolderId { get; init; }
    public int FolderRank { get; init; }
}

public sealed class ReorderUserFavoriteItemDto
{
    [Required]
    public string FavoriteId { get; init; }

    [Required]
    public string FavoriteType { get; init; }
    public int FavoriteRank { get; init; }
}

public sealed class UpdateUserFavoriteFolderAssignmentRequestDto
{
    public int FavoriteId { get; init; }

    [Required]
    public string FavoriteType { get; init; }
    public int? FolderId { get; init; }
}

public sealed class ToggleUserFavoriteRequestDto
{
    [Required]
    public string Type { get; init; }
    public int? Id { get; init; }
    public string Search { get; init; }
}

public sealed class ToggleUserFavoriteResponseDto
{
    public string Type { get; init; }
    public int? Id { get; init; }
    public string Search { get; init; }
    public bool IsStarred { get; init; }
    public int StarCount { get; init; }
}

public sealed class ToggleAdminModeResponseDto
{
    public string AdminEnabled { get; init; }
}
