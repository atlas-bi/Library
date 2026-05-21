using System.Security.Claims;
using System.Text.RegularExpressions;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Users;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Services;

public interface IUsersApiService
{
    Task<UserPageDto> GetUserPageAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    );
    Task<UserStarsDto> GetStarsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<UserGroupDto>> GetGroupsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<UserSubscriptionDto>> GetSubscriptionsAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    );
    Task<UserHistorySectionDto> GetHistoryAsync(
        ClaimsPrincipal user,
        int requestedId,
        CancellationToken cancellationToken
    );
    Task<UserSharedObjectsDto> GetSharedObjectsAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<UserSearchHistoryItemDto>> GetSearchHistoryAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
    Task<UserFavoriteFolderDto> CreateFolderAsync(
        int workspaceUserId,
        CreateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken
    );
    Task<UserFavoriteFolderDto> UpdateFolderAsync(
        int workspaceUserId,
        int id,
        UpdateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken
    );
    Task<bool> DeleteFolderAsync(
        int workspaceUserId,
        int id,
        CancellationToken cancellationToken
    );
    Task ReorderFoldersAsync(
        int workspaceUserId,
        IReadOnlyList<ReorderUserFavoriteFolderItemDto> request,
        CancellationToken cancellationToken
    );
    Task ReorderFavoritesAsync(
        int workspaceUserId,
        IReadOnlyList<ReorderUserFavoriteItemDto> request,
        CancellationToken cancellationToken
    );
    Task<bool> UpdateFavoriteFolderAsync(
        int workspaceUserId,
        UpdateUserFavoriteFolderAssignmentRequestDto request,
        CancellationToken cancellationToken
    );
    Task<bool> RemoveSharedObjectAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<ToggleUserFavoriteResponseDto> ToggleFavoriteAsync(
        int workspaceUserId,
        ToggleUserFavoriteRequestDto request,
        CancellationToken cancellationToken
    );
    Task<ToggleAdminModeResponseDto> ToggleAdminModeAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
}

public sealed partial class UsersApiService : IUsersApiService
{
    private static readonly Regex SearchRegex = new(
        @"Query=(.*?)[&|?|\s]",
        RegexOptions.None,
        TimeSpan.FromSeconds(1)
    );

    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsersApiService(
        Atlas_WebContext context,
        IConfiguration configuration,
        IMemoryCache cache,
        IAuthorizationService authorizationService = null,
        IHttpContextAccessor httpContextAccessor = null
    )
    {
        _context = context;
        _configuration = configuration;
        _cache = cache;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    private bool IsUserProfileEnabled()
    {
        var value = _configuration["features:enable_user_profile"];
        return string.IsNullOrEmpty(value) || value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsFeatureEnabled(string key)
    {
        var value = _configuration[key];
        return string.IsNullOrWhiteSpace(value)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private HttpContext GetCurrentHttpContext()
    {
        return _httpContextAccessor?.HttpContext ?? new DefaultHttpContext();
    }

    private static string TruncateWithReadMore(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var trimmed = text.Trim();
        return trimmed.Substring(0, Math.Min(160, trimmed.Length)) + "... ";
    }

    private static int ResolveTargetUserId(ClaimsPrincipal user, int requestedId)
    {
        var currentUserId = user.GetUserId();
        return user.HasPermission("View Other User") ? requestedId : currentUserId;
    }

    private static string DecodeSearchString(string search)
    {
        return SearchRegex.Match((search ?? string.Empty) + " ").Groups[1].Value
            .Replace("%25", "%")
            .Replace("%20", " ")
            .Replace("%2C", ",");
    }

    private void InvalidateWorkspaceCaches(int userId)
    {
        _cache.Remove("FavoriteFolders-" + userId);
        _cache.Remove("FavoriteReports-" + userId);
    }

    private static UserFavoriteFolderDto ToFolderDto(UserFavoriteFolder folder, int itemCount)
    {
        return new UserFavoriteFolderDto
        {
            Id = folder.UserFavoriteFolderId,
            Name = folder.FolderName,
            Rank = folder.FolderRank,
            ItemCount = itemCount,
        };
    }

    private async Task<int> CountFolderItemsAsync(int folderId, CancellationToken cancellationToken)
    {
        return await _context.StarredCollections.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredGroups.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredInitiatives.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredReports.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredSearches.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredTerms.CountAsync(x => x.Folderid == folderId, cancellationToken)
            + await _context.StarredUsers.CountAsync(x => x.Folderid == folderId, cancellationToken);
    }

    private async Task ClearFolderAssignmentsAsync(
        int currentUserId,
        int folderId,
        CancellationToken cancellationToken
    )
    {
        await _context.StarredCollections.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredReports.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredInitiatives.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredTerms.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredUsers.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredGroups.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
        await _context.StarredSearches.Where(x => x.Folderid == folderId && x.Ownerid == currentUserId)
            .ForEachAsync(x => x.Folderid = null, cancellationToken);
    }

    private static async Task SetFavoriteRankAsync<T>(
        DbSet<T> dbSet,
        int currentUserId,
        int favoriteId,
        int favoriteRank,
        CancellationToken cancellationToken
    ) where T : class
    {
        dynamic entity = await dbSet.FindAsync([favoriteId], cancellationToken);
        if (entity != null && entity.Ownerid == currentUserId)
        {
            entity.Rank = favoriteRank;
        }
    }

    private async Task<bool> SetFavoriteFolderAsync<T>(
        DbSet<T> dbSet,
        int currentUserId,
        int favoriteId,
        int? folderId,
        CancellationToken cancellationToken
    ) where T : class
    {
        dynamic entity = await dbSet.FindAsync([favoriteId], cancellationToken);
        if (entity == null || entity.Ownerid != currentUserId)
        {
            return false;
        }

        entity.Folderid = folderId;
        await _context.SaveChangesAsync(cancellationToken);
        InvalidateWorkspaceCaches(currentUserId);
        return true;
    }
}
