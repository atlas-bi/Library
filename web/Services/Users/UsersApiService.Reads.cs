using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Users;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public sealed partial class UsersApiService
{
    public async Task<UserPageDto> GetUserPageAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    )
    {
        var viewerId = user.GetUserId();
        var targetUserId = ResolveTargetUserId(user, requestedId);
        var targetUser = await _context.Users.AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == targetUserId, cancellationToken);

        if (targetUser == null)
        {
            return null;
        }

        var canViewOtherUsers = user.HasPermission("View Other User");
        var canViewGroups = user.HasPermission("View Groups");
        var canViewAnalytics = user.HasPermission("View Site Analytics");
        var canEditOtherUsers = user.HasPermission("Edit Other Users");
        var isAdministrator = user.HasPermission("Administrator");
        var isCurrentUser = targetUserId == viewerId;
        var canEditWorkspace = isCurrentUser || canEditOtherUsers;

        return new UserPageDto
        {
            User = new UserPageUserDto
            {
                Id = targetUser.UserId,
                Username = targetUser.Username,
                FullName = targetUser.FullnameCalc ?? targetUser.FullName ?? targetUser.DisplayName,
                FirstName = targetUser.FirstnameCalc ?? targetUser.FirstName,
                DisplayName = targetUser.DisplayName,
                Email = targetUser.Email,
                Department = targetUser.Department,
                Title = targetUser.Title,
                Phone = targetUser.Phone,
                ProfilePhoto = targetUser.ProfilePhoto,
            },
            Viewer = new UserPageViewerDto
            {
                Id = viewerId,
                IsCurrentUser = isCurrentUser,
                IsAdministrator = isAdministrator,
                AdminEnabled = user.HasAdminEnabled(),
            },
            Permissions = new UserPagePermissionsDto
            {
                CanViewOtherUsers = canViewOtherUsers,
                CanViewGroups = canViewGroups,
                CanViewAnalytics = canViewAnalytics,
                CanEditOtherUsers = canEditOtherUsers,
                CanToggleAdminMode = isAdministrator,
                CanEditWorkspace = canEditWorkspace,
            },
            Tabs = new UserPageTabsDto
            {
                StarsVisible = true,
                SubscriptionsVisible = true,
                ActivityVisible = true,
                RunListVisible = true,
                AtlasHistoryVisible = true,
                GroupsVisible = canViewGroups,
                AnalyticsVisible = canViewAnalytics,
            },
            Features = new UserPageFeaturesDto
            {
                UserProfilesEnabled = IsUserProfileEnabled(),
            },
            DefaultReportTypeIds = await _context.ReportObjectTypes.AsNoTracking()
                .Where(x => x.Visible == "Y")
                .OrderBy(x => x.ReportObjectTypeId)
                .Select(x => x.ReportObjectTypeId)
                .ToListAsync(cancellationToken),
        };
    }

    public async Task<UserStarsDto> GetStarsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    )
    {
        var viewerId = user.GetUserId();
        var targetUserId = ResolveTargetUserId(user, requestedId);
        var isCurrentUser = viewerId == targetUserId;
        var canManageWorkspace = isCurrentUser || user.HasPermission("Edit Other Users");
        var folderLookup = await _context.UserFavoriteFolders.AsNoTracking()
            .Where(x => x.UserId == targetUserId)
            .ToDictionaryAsync(x => x.UserFavoriteFolderId, cancellationToken);

        var folders = await _context.UserFavoriteFolders.AsNoTracking()
            .Where(x => x.UserId == targetUserId)
            .Select(x => new UserFavoriteFolderDto
            {
                Id = x.UserFavoriteFolderId,
                Name = x.FolderName,
                Rank = x.FolderRank,
                ItemCount =
                    x.StarredCollections.Count
                    + x.StarredGroups.Count
                    + x.StarredInitiatives.Count
                    + x.StarredReports.Count
                    + x.StarredSearches.Count
                    + x.StarredTerms.Count
                    + x.StarredUsers.Count,
                CanManage = canManageWorkspace,
                CanReorder = isCurrentUser,
            })
            .ToListAsync(cancellationToken);

        var items = new List<UserFavoriteItemDto>();
        items.AddRange(
            await GetStarredReportsAsync(
                user,
                targetUserId,
                folderLookup,
                isCurrentUser,
                cancellationToken
            )
        );
        items.AddRange(await GetStarredCollectionsAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));
        items.AddRange(await GetStarredInitiativesAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));
        items.AddRange(await GetStarredTermsAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));
        items.AddRange(await GetStarredUsersAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));
        items.AddRange(await GetStarredGroupsAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));
        items.AddRange(await GetStarredSearchesAsync(targetUserId, folderLookup, isCurrentUser, cancellationToken));

        var hasReports = items.Any(x => x.Type == "report");
        var hasCollections = items.Any(x => x.Type == "collection");
        var hasInitiatives = items.Any(x => x.Type == "initiative");
        var hasTerms = items.Any(x => x.Type == "term");
        var hasUsers = items.Any(x => x.Type == "user");
        var hasGroups = items.Any(x => x.Type == "group");
        var hasSearches = items.Any(x => x.Type == "search");
        var unsortedCount = items.Count(x => x.FolderId == null);
        var totalCount = items.Count;

        var suggestedReports = new List<UserSuggestedReportDto>();
        if (items.Count == 0)
        {
            suggestedReports = await _context.ReportObjects.AsNoTracking()
                .Where(x =>
                    x.ReportObjectRunDataBridges.Any(y => y.RunData.RunUserId == targetUserId)
                    && x.ReportObjectType.Visible == "Y"
                )
                .OrderByDescending(x =>
                    x.ReportObjectRunDataBridges.Where(y => y.RunData.RunUserId == targetUserId)
                        .Sum(y => y.Runs)
                )
                .Take(30)
                .Select(x => new UserSuggestedReportDto
                {
                    Id = x.ReportObjectId,
                    Name = x.DisplayTitle ?? x.DisplayName ?? x.Name,
                    Description = x.Description,
                    Url = "/reports?id=" + x.ReportObjectId,
                    Type = x.ReportObjectType.ShortName,
                })
                .ToListAsync(cancellationToken);
        }

        return new UserStarsDto
        {
            UserId = targetUserId,
            ViewerUserId = viewerId,
            IsCurrentUser = isCurrentUser,
            CanEditWorkspace = canManageWorkspace,
            Permissions = new UserWorkspacePermissionsDto
            {
                CanCreateFolders = canManageWorkspace,
                CanRenameFolders = canManageWorkspace,
                CanDeleteFolders = canManageWorkspace,
                CanReorderFolders = isCurrentUser,
                CanReorderFavorites = isCurrentUser,
                CanMoveFavoritesToFolders = canManageWorkspace,
                CanToggleFavorites = canManageWorkspace,
            },
            Summary = new UserWorkspaceSummaryDto
            {
                TotalCount = totalCount,
                UnsortedCount = unsortedCount,
                HasFolders = folders.Count > 0,
                ShowUnsortedBucket = folders.Count > 0 && unsortedCount > 0,
            },
            Filters = new UserWorkspaceFilterStateDto
            {
                HasReports = hasReports,
                HasCollections = hasCollections,
                HasInitiatives = hasInitiatives,
                HasTerms = hasTerms,
                HasUsers = hasUsers,
                HasGroups = hasGroups,
                HasSearches = hasSearches,
                ShowQuickFilters =
                    new[] { hasReports, hasCollections, hasInitiatives, hasTerms, hasUsers, hasGroups, hasSearches }
                        .Count(x => x) > 1,
            },
            Folders = folders.OrderBy(x => x.Rank ?? 999).ToList(),
            Items = items.OrderBy(x => x.Rank ?? int.MaxValue).ThenBy(x => x.Name).ToList(),
            SuggestedReports = suggestedReports,
        };
    }

    public async Task<IReadOnlyList<UserGroupDto>> GetGroupsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    )
    {
        var targetUserId = ResolveTargetUserId(user, requestedId);
        return await _context.UserGroupsMemberships.AsNoTracking()
            .Where(x => x.UserId == targetUserId)
            .Select(x => new UserGroupDto
            {
                Id = x.GroupId,
                Name = x.Group.GroupName,
                Type = x.Group.GroupType,
                Source = x.Group.GroupSource,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserSubscriptionDto>> GetSubscriptionsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    )
    {
        var targetUserId = ResolveTargetUserId(user, requestedId);
        return await (
            from r in _context.ReportObjectSubscriptions.Where(x => x.UserId == targetUserId)
                .Union(
                    from m in _context.UserGroupsMemberships
                    join s in _context.ReportObjectSubscriptions on m.Group.GroupEmail equals s.SubscriptionTo
                    where m.UserId == targetUserId
                    select s
                )
            orderby r.InactiveFlags, r.LastRunTime descending
            select new UserSubscriptionDto
            {
                ReportId = r.ReportObjectId,
                Name = r.ReportObject.DisplayName,
                Description = r.Description,
                LastStatus = r.LastStatus.Replace(";", "; "),
                LastRun = r.LastRunDisplayString,
                SentTo = r.SubscriptionTo.Replace(";", "; "),
                EmailList = r.EmailList,
            }
        ).ToListAsync(cancellationToken);
    }

    public async Task<UserHistorySectionDto> GetHistoryAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    )
    {
        var targetUserId = ResolveTargetUserId(user, requestedId);

        var atlasHistory = await _context.Analytics.AsNoTracking()
            .Where(x =>
                x.UserId == targetUserId
                && x.AccessDateTime > DateTime.Today.AddDays(-7)
                && x.Pathname != "/"
            )
            .OrderByDescending(x => x.AccessDateTime)
            .Select(x => new UserHistoryItemDto
            {
                Name = x.Pathname,
                Type = ToHistoryType(x.Pathname),
                Url = x.Href,
                Date = x.AccessDateTimeDisplayString,
            })
            .ToListAsync(cancellationToken);

        var reportEdits = await _context.ReportObjectDocs.AsNoTracking()
            .Where(x => x.UpdatedBy == targetUserId && x.LastUpdateDateTime > DateTime.Today.AddDays(-30))
            .OrderByDescending(x => x.LastUpdateDateTime)
            .Take(10)
            .Select(x => new UserHistoryItemDto
            {
                Name = x.ReportObject.DisplayName,
                Type = "Report",
                Url = "/reports?id=" + x.ReportObjectId,
                Date = x.LastUpdatedDateTimeDisplayString,
            })
            .ToListAsync(cancellationToken);

        var initiativeEdits = await _context.Initiatives.AsNoTracking()
            .Where(x => x.LastUpdateUser == targetUserId && x.LastUpdateDate > DateTime.Today.AddDays(-30))
            .OrderByDescending(x => x.LastUpdateDate)
            .Take(10)
            .Select(x => new UserHistoryItemDto
            {
                Name = x.Name,
                Type = "Initiative",
                Url = "/initiatives?id=" + x.InitiativeId,
                Date = x.LastUpdatedDateDisplayString,
            })
            .ToListAsync(cancellationToken);

        var collectionEdits = await _context.Collections.AsNoTracking()
            .Where(x => x.LastUpdateUser == targetUserId && x.LastUpdateDate > DateTime.Today.AddDays(-30))
            .OrderByDescending(x => x.LastUpdateDate)
            .Take(10)
            .Select(x => new UserHistoryItemDto
            {
                Name = x.Name,
                Type = "Collection",
                Url = "/collections?id=" + x.CollectionId,
                Date = x.LastUpdatedDateDisplayString,
            })
            .ToListAsync(cancellationToken);

        var termEdits = await _context.Terms.AsNoTracking()
            .Where(x => x.UpdatedByUserId == targetUserId && x.LastUpdatedDateTime > DateTime.Today.AddDays(-30))
            .OrderByDescending(x => x.LastUpdatedDateTime)
            .Take(10)
            .Select(x => new UserHistoryItemDto
            {
                Name = x.Name,
                Type = "Term",
                Url = "/terms?id=" + x.TermId,
                Date = x.LastUpdatedDateTimeDisplayString,
            })
            .ToListAsync(cancellationToken);

        return new UserHistorySectionDto
        {
            AtlasHistory = atlasHistory,
            ReportEdits = reportEdits,
            InitiativeEdits = initiativeEdits,
            CollectionEdits = collectionEdits,
            TermEdits = termEdits,
        };
    }

    private static string ToHistoryType(string path)
    {
        return path.ToLowerInvariant() switch
        {
            "/reports" => "Reports",
            "/terms" => "Terms",
            "/projects" => "Collections",
            "/collections" => "Collections",
            "/initiatives" => "Initiatives",
            "/users" => "Users",
            "/contacts" => "Reports",
            "/tasks" => "Tasks",
            "/search" => "Search",
            _ => "Other",
        };
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredReportsAsync(
        ClaimsPrincipal user,
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var httpContext = GetCurrentHttpContext();
        var canOpenInEditor = user.HasPermission("Open In Editor");
        var sharingEnabled = IsFeatureEnabled("features:enable_sharing");
        var requestAccessEnabled = IsFeatureEnabled("features:enable_request_access");

        var stars = await _context.StarredReports.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectDoc)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectType)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectAttachments)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportTagLinks)
                    .ThenInclude(x => x.Tag)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportGroupsMemberships)
            .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectHierarchyChildReportObjects)
                    .ThenInclude(x => x.ParentReportObject)
                        .ThenInclude(x => x.ReportGroupsMemberships)
            .ToListAsync(cancellationToken);
        var reportIds = stars.Select(x => x.Reportid).Distinct().ToArray();
        var reportTypeIds = stars
            .Select(x => x.Report.ReportObjectTypeId)
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .Distinct()
            .ToArray();
        var starCounts = await _context.StarredReports.AsNoTracking()
            .Where(x => reportIds.Contains(x.Reportid))
            .GroupBy(x => x.Reportid)
            .Select(x => new { ReportId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.ReportId, x => x.Count, cancellationToken);
        var reportTypes = await _context.ReportObjectTypes.AsNoTracking()
            .Where(x => reportTypeIds.Contains(x.ReportObjectTypeId))
            .ToDictionaryAsync(x => x.ReportObjectTypeId, cancellationToken);

        var items = new List<UserFavoriteItemDto>(stars.Count);
        foreach (var star in stars)
        {
            var report = star.Report;
            var canRun = await CanRunFavoriteReportAsync(user, report);
            var runUrl = report.RunReportUrl(httpContext, _configuration, canRun);
            var editUrl = report.EditReportUrl(httpContext, _configuration);
            var manageUrl = report.ManageReportUrl(httpContext, _configuration);
            var hasRunAttachments = report.ReportObjectAttachments.Count > 0 && !httpContext.IsAgl();
            var reportType =
                report.ReportObjectType
                ?? (
                    report.ReportObjectTypeId.HasValue
                    && reportTypes.TryGetValue(report.ReportObjectTypeId.Value, out var loadedType)
                        ? loadedType
                        : null
                );
            var bodyText =
                TruncateWithReadMore(report.ReportObjectDoc?.DeveloperDescription)
                ?? TruncateWithReadMore(report.Description)
                ?? "Open to view details.";

            items.Add(
                new UserFavoriteItemDto
                {
                    StarId = star.StarId,
                    Type = "report",
                    TypeLabel =
                        string.IsNullOrEmpty(reportType?.ShortName)
                            ? reportType?.Name
                            : reportType.ShortName,
                    FolderId = star.Folderid,
                    Rank = star.Rank,
                    ItemId = star.Reportid,
                    Name = report.DisplayTitle ?? report.DisplayName ?? report.Name,
                    Description = report.Description,
                    BodyText = bodyText,
                    Url = "/reports?id=" + star.Reportid,
                    SecondaryText = reportType?.ShortName,
                    CanReorder = canReorder,
                    IsStarred = true,
                    StarCount = starCounts.GetValueOrDefault(report.ReportObjectId),
                    PlaceholderImageUrl = "/img/report_placeholder_128x128.png",
                    ThumbnailUrl =
                        "/data/img?handler=Thumb&id=" + report.ReportObjectId + "&size=128x128",
                    FullImageUrl =
                        "/data/img?handler=Thumb&id=" + report.ReportObjectId + "&size=1200x2000",
                    IsCertified = report.ReportTagLinks.Any(x =>
                        x.Tag.Name == "Analytics Certified" || x.Tag.Name == "Analytics Reviewed"
                    ),
                    CanOpenProfile = true,
                    ProfileTargetId = "report-profile-" + report.ReportObjectId,
                    CanShare = sharingEnabled,
                    ShareTargetId = "report-share-" + report.ReportObjectId,
                    ShareName = report.DisplayTitle ?? report.DisplayName ?? report.Name,
                    ShareType = "report",
                    CanRequestAccess =
                        requestAccessEnabled && (report.ReportObjectAttachments.Count > 0 || !string.IsNullOrEmpty(runUrl)),
                    RequestAccessTargetId = "request-access-" + report.ReportObjectId,
                    CanRun = !string.IsNullOrEmpty(runUrl),
                    RunUrl = runUrl,
                    OpensRunModal = hasRunAttachments,
                    RunModalTargetId = hasRunAttachments ? "report-run-" + report.ReportObjectId : null,
                    RunDisabledReason = BuildReportRunDisabledReason(report, runUrl, editUrl),
                    CanEditInEditor = !string.IsNullOrEmpty(editUrl) && canOpenInEditor,
                    EditUrl = editUrl,
                    CanManageInEditor = !string.IsNullOrEmpty(manageUrl) && canOpenInEditor,
                    ManageUrl = manageUrl,
                    ReportObjectUrl = report.ReportObjectUrl,
                    ReportServerPath = report.ReportServerPath,
                    SourceServer = report.SourceServer,
                    EpicMasterFile = report.EpicMasterFile,
                    EpicRecordId = report.EpicRecordId,
                    EpicReportTemplateId = report.EpicReportTemplateId,
                    EnabledForHyperspace = report.ReportObjectDoc?.EnabledForHyperspace ?? "N",
                    Tags = report.ReportTagLinks
                        .Select(
                            x =>
                                new UserFavoriteTagDto
                                {
                                    Name = x.Tag.Name,
                                    Slug = HtmlHelpers.Slug(x.Tag.Name),
                                    ShowInHeader =
                                        x.ShowInHeader == "Y" || x.Tag.ShowInHeader == "Y",
                                }
                        )
                        .ToList(),
                }
            );
        }

        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredCollectionsAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredCollections.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "collection",
                TypeLabel = "collection",
                FolderId = x.Folderid,
                Rank = x.Rank,
                ItemId = x.Collectionid,
                Name = x.Collection.Name,
                Description = x.Collection.Description,
                BodyText =
                    TruncateWithReadMore(HtmlHelpers.MarkdownToText(x.Collection.Description))
                    ?? TruncateWithReadMore(x.Collection.Purpose)
                    ?? "Open to view details.",
                Url = "/collections?id=" + x.Collectionid,
                SecondaryText = "Collection",
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = x.Collection.StarredCollections.Count,
                PlaceholderImageUrl = "/img/report_placeholder_128x128.png",
                IsCertified = true,
                CanOpenProfile = true,
                ProfileTargetId = "collection-profile-" + x.Collectionid,
                CanShare = IsFeatureEnabled("features:enable_sharing"),
                ShareTargetId = "collection-share-" + x.Collectionid,
                ShareName = x.Collection.Name,
                ShareType = "collection",
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredInitiativesAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredInitiatives.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "initiative",
                TypeLabel = "initiative",
                FolderId = x.Folderid,
                Rank = x.Rank,
                ItemId = x.Initiativeid,
                Name = x.Initiative.Name,
                Description = x.Initiative.Description,
                BodyText =
                    TruncateWithReadMore(x.Initiative.Description) ?? "Open to view details",
                Url = "/initiatives?id=" + x.Initiativeid,
                SecondaryText = "Initiative",
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = x.Initiative.StarredInitiatives.Count,
                PlaceholderImageUrl = "/img/report_placeholder_128x128.png",
                IsCertified = true,
                CanShare = IsFeatureEnabled("features:enable_sharing"),
                ShareTargetId = "initiative-share-" + x.Initiativeid,
                ShareName = x.Initiative.Name,
                ShareType = "initiative",
                RelatedCollectionNames = x.Initiative.Collections.Select(c => c.Name).ToList(),
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredTermsAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredTerms.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "term",
                TypeLabel = "term",
                FolderId = x.Folderid,
                Rank = x.Rank,
                ItemId = x.Termid,
                Name = x.Term.Name,
                Description = x.Term.Summary,
                BodyText =
                    TruncateWithReadMore(x.Term.Summary)
                    ?? TruncateWithReadMore(x.Term.TechnicalDefinition)
                    ?? "Open to view details.",
                Url = "/terms?id=" + x.Termid,
                SecondaryText = "Term",
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = x.Term.StarredTerms.Count,
                PlaceholderImageUrl = "/img/report_placeholder_128x128.png",
                IsApproved = x.Term.ApprovedYn == "Y",
                CanOpenProfile = true,
                ProfileTargetId = "term-profile-" + x.Termid,
                CanShare = IsFeatureEnabled("features:enable_sharing"),
                ShareTargetId = "term-share-" + x.Termid,
                ShareName = x.Term.Name,
                ShareType = "term",
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredUsersAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredUsers.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "user",
                TypeLabel = "user",
                FolderId = x.Folderid,
                Rank = x.Rank,
                ItemId = x.Userid,
                Name = x.User.FullnameCalc ?? x.User.DisplayName ?? x.User.Username,
                Description = x.User.Email,
                BodyText = "View user profile.",
                Url = "/users?id=" + x.Userid,
                SecondaryText = "User",
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = x.User.StarredUserUsers.Count,
                PlaceholderImageUrl = "/img/user_placeholder_128x128.png",
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredGroupsAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredGroups.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "group",
                TypeLabel = "group",
                FolderId = x.Folderid,
                Rank = x.Rank,
                ItemId = x.Groupid,
                Name = x.Group.GroupName,
                Description = x.Group.GroupEmail,
                BodyText = "View group profile.",
                Url = "/groups?id=" + x.Groupid,
                SecondaryText = x.Group.GroupType,
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = x.Group.StarredGroups.Count,
                PlaceholderImageUrl = "/img/group_placeholder_128x128.png",
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<List<UserFavoriteItemDto>> GetStarredSearchesAsync(
        int targetUserId,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup,
        bool canReorder,
        CancellationToken cancellationToken
    )
    {
        var items = await _context.StarredSearches.AsNoTracking()
            .Where(x => x.Ownerid == targetUserId)
            .Select(x => new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = "search",
                TypeLabel = "search",
                FolderId = x.Folderid,
                Rank = x.Rank,
                Name = DecodeSearchString(x.Search),
                Description = null,
                BodyText = "Open search results.",
                Url = "/search?" + x.Search,
                SecondaryText = "Search",
                SearchString = DecodeSearchString(x.Search),
                CanReorder = canReorder,
                IsStarred = true,
                StarCount = _context.StarredSearches.Count(y => y.Search == x.Search),
                PlaceholderImageUrl = "/img/report_placeholder_128x128.png",
            })
            .ToListAsync(cancellationToken);
        return AttachFolderMetadata(items, folderLookup);
    }

    private async Task<bool> CanRunFavoriteReportAsync(
        ClaimsPrincipal user,
        ReportObject report
    )
    {
        if (_authorizationService == null)
        {
            return false;
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            user,
            report,
            "ReportRunPolicy"
        );
        return authorizationResult.Succeeded;
    }

    private static string BuildReportRunDisabledReason(
        ReportObject report,
        string runUrl,
        string editUrl
    )
    {
        if (!string.IsNullOrEmpty(runUrl))
        {
            return null;
        }

        if (report.EpicMasterFile != null && report.EpicMasterFile.Equals("IDB"))
        {
            return "Open a related dashboard that uses this.";
        }

        if (!string.IsNullOrEmpty(editUrl))
        {
            return "Open in report library.";
        }

        if (report.EpicMasterFile != null)
        {
            return "Run from the Hyperspace report library.";
        }

        return null;
    }

    private static List<UserFavoriteItemDto> AttachFolderMetadata(
        IReadOnlyList<UserFavoriteItemDto> items,
        IReadOnlyDictionary<int, UserFavoriteFolder> folderLookup
    )
    {
        return items.Select(x =>
        {
            folderLookup.TryGetValue(x.FolderId ?? 0, out var folder);
            return new UserFavoriteItemDto
            {
                StarId = x.StarId,
                Type = x.Type,
                TypeLabel = x.TypeLabel,
                FolderId = x.FolderId,
                Rank = x.Rank,
                ItemId = x.ItemId,
                Name = x.Name,
                Description = x.Description,
                Url = x.Url,
                SecondaryText = x.SecondaryText,
                FolderName = folder?.FolderName,
                FolderRank = folder?.FolderRank,
                SearchString = x.SearchString,
                CanReorder = x.CanReorder,
                IsStarred = x.IsStarred,
                StarCount = x.StarCount,
                BodyText = x.BodyText,
                PlaceholderImageUrl = x.PlaceholderImageUrl,
                ThumbnailUrl = x.ThumbnailUrl,
                FullImageUrl = x.FullImageUrl,
                IsCertified = x.IsCertified,
                IsApproved = x.IsApproved,
                CanOpenProfile = x.CanOpenProfile,
                ProfileTargetId = x.ProfileTargetId,
                CanShare = x.CanShare,
                ShareTargetId = x.ShareTargetId,
                ShareName = x.ShareName,
                ShareType = x.ShareType,
                CanRequestAccess = x.CanRequestAccess,
                RequestAccessTargetId = x.RequestAccessTargetId,
                CanRun = x.CanRun,
                RunUrl = x.RunUrl,
                OpensRunModal = x.OpensRunModal,
                RunModalTargetId = x.RunModalTargetId,
                RunDisabledReason = x.RunDisabledReason,
                CanEditInEditor = x.CanEditInEditor,
                EditUrl = x.EditUrl,
                CanManageInEditor = x.CanManageInEditor,
                ManageUrl = x.ManageUrl,
                ReportObjectUrl = x.ReportObjectUrl,
                ReportServerPath = x.ReportServerPath,
                SourceServer = x.SourceServer,
                EpicMasterFile = x.EpicMasterFile,
                EpicRecordId = x.EpicRecordId,
                EpicReportTemplateId = x.EpicReportTemplateId,
                EnabledForHyperspace = x.EnabledForHyperspace,
                Tags = x.Tags,
                RelatedCollectionNames = x.RelatedCollectionNames,
            };
        }).ToList();
    }
}
