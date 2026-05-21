using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Users;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public sealed partial class UsersApiService
{
    public async Task<UserSharedObjectsDto> GetSharedObjectsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();

        var sharedToMe = await _context.SharedItems.AsNoTracking()
            .Where(x => x.SharedToUserId == currentUserId)
            .OrderByDescending(x => x.ShareDate)
            .Select(x => new UserSharedObjectDto
            {
                Id = x.Id,
                Name = x.Name,
                ShareDate = x.ShareDate == null ? null : (x.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                SharedFrom = x.SharedFromUser.FullnameCalc,
                Url = x.Url,
            })
            .ToListAsync(cancellationToken);

        var sharedFromMe = await _context.SharedItems.AsNoTracking()
            .Where(x => x.SharedFromUserId == currentUserId)
            .Select(x => new UserSharedObjectDto
            {
                Id = x.Id,
                Name = x.Name,
                ShareDate = x.ShareDate == null ? null : (x.ShareDate ?? DateTime.Now).ToString("M/d/yyyy"),
                SharedFrom = x.SharedToUser.FullnameCalc,
                Url = x.Url,
            })
            .ToListAsync(cancellationToken);

        return new UserSharedObjectsDto
        {
            SharedToMe = sharedToMe,
            SharedFromMe = sharedFromMe,
        };
    }

    public async Task<IReadOnlyList<UserSearchHistoryItemDto>> GetSearchHistoryAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        return await _context.Analytics.AsNoTracking()
            .Where(x => x.Pathname.ToLower() == "/search" && x.UserId == currentUserId)
            .OrderByDescending(x => x.AccessDateTime)
            .Take(7)
            .Select(x => new UserSearchHistoryItemDto
            {
                SearchUrl = x.Search.Replace("%25", "%"),
                SearchString = DecodeSearchString(x.Search),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserFavoriteFolderDto> CreateFolderAsync(
        int workspaceUserId,
        CreateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var folder = new UserFavoriteFolder
        {
            UserId = workspaceUserId,
            FolderName = request.Name.Trim(),
        };

        await _context.UserFavoriteFolders.AddAsync(folder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(workspaceUserId);

        return ToFolderDto(folder, 0);
    }

    public async Task<UserFavoriteFolderDto> UpdateFolderAsync(
        int workspaceUserId,
        int id,
        UpdateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var folder = await _context.UserFavoriteFolders.SingleOrDefaultAsync(
            x => x.UserFavoriteFolderId == id && x.UserId == workspaceUserId,
            cancellationToken
        );
        if (folder == null)
        {
            return null;
        }

        folder.FolderName = request.Name.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(workspaceUserId);

        return ToFolderDto(folder, await CountFolderItemsAsync(id, cancellationToken));
    }

    public async Task<bool> DeleteFolderAsync(
        int workspaceUserId,
        int id,
        CancellationToken cancellationToken
    )
    {
        var folder = await _context.UserFavoriteFolders.SingleOrDefaultAsync(
            x => x.UserFavoriteFolderId == id && x.UserId == workspaceUserId,
            cancellationToken
        );
        if (folder == null)
        {
            return false;
        }

        await ClearFolderAssignmentsAsync(workspaceUserId, id, cancellationToken);
        _context.UserFavoriteFolders.Remove(folder);
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(workspaceUserId);
        return true;
    }

    public async Task ReorderFoldersAsync(
        int workspaceUserId,
        IReadOnlyList<ReorderUserFavoriteFolderItemDto> request,
        CancellationToken cancellationToken
    )
    {
        foreach (var item in request)
        {
            if (!Int32.TryParse(item.FolderId, out var folderId))
            {
                continue;
            }

            var folder = await _context.UserFavoriteFolders.SingleOrDefaultAsync(
                x => x.UserFavoriteFolderId == folderId && x.UserId == workspaceUserId,
                cancellationToken
            );
            if (folder != null)
            {
                folder.FolderRank = item.FolderRank;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(workspaceUserId);
    }

    public async Task ReorderFavoritesAsync(
        int workspaceUserId,
        IReadOnlyList<ReorderUserFavoriteItemDto> request,
        CancellationToken cancellationToken
    )
    {
        foreach (var item in request)
        {
            if (!Int32.TryParse(item.FavoriteId, out var favoriteId))
            {
                continue;
            }

            switch (item.FavoriteType)
            {
                case "report":
                    await SetFavoriteRankAsync(_context.StarredReports, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "collection":
                    await SetFavoriteRankAsync(_context.StarredCollections, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "initiative":
                    await SetFavoriteRankAsync(_context.StarredInitiatives, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "term":
                    await SetFavoriteRankAsync(_context.StarredTerms, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "user":
                    await SetFavoriteRankAsync(_context.StarredUsers, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "group":
                    await SetFavoriteRankAsync(_context.StarredGroups, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                case "search":
                    await SetFavoriteRankAsync(_context.StarredSearches, workspaceUserId, favoriteId, item.FavoriteRank, cancellationToken);
                    break;
                default:
                    continue;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(workspaceUserId);
    }

    public async Task<bool> UpdateFavoriteFolderAsync(
        int workspaceUserId,
        UpdateUserFavoriteFolderAssignmentRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var folderId = request.FolderId == 0 ? null : request.FolderId;

        return request.FavoriteType switch
        {
            "report" => await SetFavoriteFolderAsync(_context.StarredReports, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "collection" => await SetFavoriteFolderAsync(_context.StarredCollections, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "initiative" => await SetFavoriteFolderAsync(_context.StarredInitiatives, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "term" => await SetFavoriteFolderAsync(_context.StarredTerms, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "user" => await SetFavoriteFolderAsync(_context.StarredUsers, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "group" => await SetFavoriteFolderAsync(_context.StarredGroups, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            "search" => await SetFavoriteFolderAsync(_context.StarredSearches, workspaceUserId, request.FavoriteId, folderId, cancellationToken),
            _ => false,
        };
    }

    public async Task<bool> RemoveSharedObjectAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var sharedItem = await _context.SharedItems.SingleOrDefaultAsync(
            x =>
                x.Id == id
                && (x.SharedFromUserId == currentUserId || x.SharedToUserId == currentUserId),
            cancellationToken
        );
        if (sharedItem == null)
        {
            return false;
        }

        _context.SharedItems.Remove(sharedItem);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ToggleUserFavoriteResponseDto> ToggleFavoriteAsync(
        int workspaceUserId,
        ToggleUserFavoriteRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var type = (request.Type ?? string.Empty).Trim().ToLowerInvariant();

        return type switch
        {
            "report" => await ToggleReportFavoriteAsync(workspaceUserId, request.Id, cancellationToken),
            "collection" => await ToggleCollectionFavoriteAsync(workspaceUserId, request.Id, cancellationToken),
            "initiative" => await ToggleInitiativeFavoriteAsync(workspaceUserId, request.Id, cancellationToken),
            "term" => await ToggleTermFavoriteAsync(workspaceUserId, request.Id, cancellationToken),
            "user" => await ToggleUserFavoriteEntityAsync(workspaceUserId, request.Id, cancellationToken),
            "group" => await ToggleGroupFavoriteAsync(workspaceUserId, request.Id, cancellationToken),
            "search" => await ToggleSearchFavoriteAsync(workspaceUserId, request.Search, cancellationToken),
            _ => throw new InvalidOperationException("Unsupported favorite type."),
        };
    }

    public async Task<ToggleAdminModeResponseDto> ToggleAdminModeAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var adminDisabled = await _context.UserPreferences.SingleOrDefaultAsync(
            x => x.UserId == currentUserId && x.ItemType == "AdminDisabled",
            cancellationToken
        );

        if (adminDisabled == null)
        {
            await _context.UserPreferences.AddAsync(
                new UserPreference { UserId = currentUserId, ItemType = "AdminDisabled" },
                cancellationToken
            );
            await _context.SaveChangesAsync(cancellationToken);
            return new ToggleAdminModeResponseDto { AdminEnabled = "N" };
        }

        _context.UserPreferences.RemoveRange(
            _context.UserPreferences.Where(x => x.UserId == currentUserId && x.ItemType == "AdminDisabled")
        );
        await _context.SaveChangesAsync(cancellationToken);
        return new ToggleAdminModeResponseDto { AdminEnabled = "Y" };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleReportFavoriteAsync(
        int currentUserId,
        int? reportId,
        CancellationToken cancellationToken
    )
    {
        if (reportId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredReports
            .Where(x => x.Ownerid == currentUserId && x.Reportid == reportId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredReports.AddAsync(
                new StarredReport { Ownerid = currentUserId, Reportid = reportId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredReports.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("report-" + reportId.Value);

        return new ToggleUserFavoriteResponseDto
        {
            Type = "report",
            Id = reportId,
            IsStarred = isStarred,
            StarCount = await _context.StarredReports.CountAsync(x => x.Reportid == reportId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleCollectionFavoriteAsync(
        int currentUserId,
        int? collectionId,
        CancellationToken cancellationToken
    )
    {
        if (collectionId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredCollections
            .Where(x => x.Ownerid == currentUserId && x.Collectionid == collectionId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredCollections.AddAsync(
                new StarredCollection { Ownerid = currentUserId, Collectionid = collectionId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredCollections.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("collection-" + collectionId.Value);
        _cache.Remove("collections");

        return new ToggleUserFavoriteResponseDto
        {
            Type = "collection",
            Id = collectionId,
            IsStarred = isStarred,
            StarCount = await _context.StarredCollections.CountAsync(x => x.Collectionid == collectionId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleInitiativeFavoriteAsync(
        int currentUserId,
        int? initiativeId,
        CancellationToken cancellationToken
    )
    {
        if (initiativeId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredInitiatives
            .Where(x => x.Ownerid == currentUserId && x.Initiativeid == initiativeId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredInitiatives.AddAsync(
                new StarredInitiative { Ownerid = currentUserId, Initiativeid = initiativeId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredInitiatives.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("initiative-" + initiativeId.Value);
        _cache.Remove("initatives");

        return new ToggleUserFavoriteResponseDto
        {
            Type = "initiative",
            Id = initiativeId,
            IsStarred = isStarred,
            StarCount = await _context.StarredInitiatives.CountAsync(x => x.Initiativeid == initiativeId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleTermFavoriteAsync(
        int currentUserId,
        int? termId,
        CancellationToken cancellationToken
    )
    {
        if (termId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredTerms
            .Where(x => x.Ownerid == currentUserId && x.Termid == termId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredTerms.AddAsync(
                new StarredTerm { Ownerid = currentUserId, Termid = termId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredTerms.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("term-" + termId.Value);
        _cache.Remove("terms");

        return new ToggleUserFavoriteResponseDto
        {
            Type = "term",
            Id = termId,
            IsStarred = isStarred,
            StarCount = await _context.StarredTerms.CountAsync(x => x.Termid == termId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleUserFavoriteEntityAsync(
        int currentUserId,
        int? userId,
        CancellationToken cancellationToken
    )
    {
        if (userId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredUsers
            .Where(x => x.Ownerid == currentUserId && x.Userid == userId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredUsers.AddAsync(
                new StarredUser { Ownerid = currentUserId, Userid = userId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredUsers.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("user-" + userId.Value);

        return new ToggleUserFavoriteResponseDto
        {
            Type = "user",
            Id = userId,
            IsStarred = isStarred,
            StarCount = await _context.StarredUsers.CountAsync(x => x.Userid == userId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleGroupFavoriteAsync(
        int currentUserId,
        int? groupId,
        CancellationToken cancellationToken
    )
    {
        if (groupId == null)
        {
            throw new InvalidOperationException("Favorite id is required.");
        }

        var existing = await _context.StarredGroups
            .Where(x => x.Ownerid == currentUserId && x.Groupid == groupId.Value)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredGroups.AddAsync(
                new StarredGroup { Ownerid = currentUserId, Groupid = groupId.Value },
                cancellationToken
            );
        }
        else
        {
            _context.StarredGroups.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _cache.Remove("group-" + groupId.Value);

        return new ToggleUserFavoriteResponseDto
        {
            Type = "group",
            Id = groupId,
            IsStarred = isStarred,
            StarCount = await _context.StarredGroups.CountAsync(x => x.Groupid == groupId.Value, cancellationToken),
        };
    }

    private async Task<ToggleUserFavoriteResponseDto> ToggleSearchFavoriteAsync(
        int currentUserId,
        string search,
        CancellationToken cancellationToken
    )
    {
        var normalizedSearch = (search ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(normalizedSearch))
        {
            throw new InvalidOperationException("Search is required.");
        }

        var existing = await _context.StarredSearches
            .Where(x => x.Ownerid == currentUserId && x.Search == normalizedSearch)
            .ToListAsync(cancellationToken);
        var isStarred = existing.Count == 0;
        if (isStarred)
        {
            await _context.StarredSearches.AddAsync(
                new StarredSearch { Ownerid = currentUserId, Search = normalizedSearch },
                cancellationToken
            );
        }
        else
        {
            _context.StarredSearches.RemoveRange(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ToggleUserFavoriteResponseDto
        {
            Type = "search",
            Search = normalizedSearch,
            IsStarred = isStarred,
            StarCount = await _context.StarredSearches.CountAsync(x => x.Search == normalizedSearch, cancellationToken),
        };
    }
}
