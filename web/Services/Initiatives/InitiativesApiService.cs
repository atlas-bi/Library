using System.Security.Claims;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Initiatives;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Services;

public interface IInitiativesApiService
{
    Task<InitiativeListResponseDto> GetInitiativesAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
    Task<InitiativeDetailDto> GetInitiativeAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    );
    Task<InitiativeDetailDto> CreateInitiativeAsync(
        ClaimsPrincipal user,
        CreateInitiativeRequestDto request,
        CancellationToken cancellationToken
    );
    Task<InitiativeDetailDto> UpdateInitiativeAsync(
        ClaimsPrincipal user,
        int id,
        UpdateInitiativeRequestDto request,
        CancellationToken cancellationToken
    );
    Task<bool> DeleteInitiativeAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<InitiativeCollectionSearchResultDto>> SearchCollectionsAsync(
        string search,
        CancellationToken cancellationToken
    );
}

public sealed class InitiativesApiService : IInitiativesApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public InitiativesApiService(
        Atlas_WebContext context,
        IConfiguration configuration,
        IMemoryCache cache
    )
    {
        _context = context;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<InitiativeListResponseDto> GetInitiativesAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();

        var items = await _context.Initiatives.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new InitiativeListItemDto
            {
                Id = x.InitiativeId,
                Name = x.Name,
                Description = x.Description,
                IsStarred = x.StarredInitiatives.Any(y => y.Ownerid == currentUserId),
                StarCount = x.StarredInitiatives.Count,
            })
            .ToListAsync(cancellationToken);

        return new InitiativeListResponseDto
        {
            Features = BuildFeatures(),
            Permissions = BuildPermissions(user),
            Items = items,
        };
    }

    public async Task<InitiativeDetailDto> GetInitiativeAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = user.GetUserId();
        var canViewUserProfiles = user.HasPermission("View Other User") && IsUserProfileEnabled();
        var initiative = await _context.Initiatives.AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.StarredInitiatives)
            .Include(x => x.OperationOwner)
            .Include(x => x.ExecutiveOwner)
            .Include(x => x.LastUpdateUserNavigation)
            .Include(x => x.FinancialImpactNavigation)
            .Include(x => x.StrategicImportanceNavigation)
            .Include(x => x.Collections)
            .SingleOrDefaultAsync(x => x.InitiativeId == id, cancellationToken);

        if (initiative == null)
        {
            return null;
        }

        return new InitiativeDetailDto
        {
            Id = initiative.InitiativeId,
            Name = initiative.Name,
            Description = initiative.Description,
            Hidden = initiative.Hidden,
            LastModified = initiative.LastUpdateDate,
            LastModifiedDisplay = initiative.LastUpdatedDateDisplayString,
            IsStarred = initiative.StarredInitiatives.Any(y => y.Ownerid == currentUserId),
            StarCount = initiative.StarredInitiatives.Count,
            CanCreateInitiative = user.HasPermission("Create Initiative"),
            CanEditInitiative = user.HasPermission("Edit Initiative"),
            CanDeleteInitiative = user.HasPermission("Delete Initiative"),
            CanViewUserProfiles = canViewUserProfiles,
            Features = BuildFeatures(),
            OperationOwner = initiative.OperationOwner == null
                ? null
                : new InitiativeUserSummaryDto
                {
                    Id = initiative.OperationOwner.UserId,
                    Username = initiative.OperationOwner.Username,
                    FullName = initiative.OperationOwner.FullnameCalc,
                    Email = initiative.OperationOwner.Email,
                },
            ExecutiveOwner = initiative.ExecutiveOwner == null
                ? null
                : new InitiativeUserSummaryDto
                {
                    Id = initiative.ExecutiveOwner.UserId,
                    Username = initiative.ExecutiveOwner.Username,
                    FullName = initiative.ExecutiveOwner.FullnameCalc,
                    Email = initiative.ExecutiveOwner.Email,
                },
            LastUpdatedBy = initiative.LastUpdateUserNavigation == null
                ? null
                : new InitiativeUserSummaryDto
                {
                    Id = initiative.LastUpdateUserNavigation.UserId,
                    Username = initiative.LastUpdateUserNavigation.Username,
                    FullName = initiative.LastUpdateUserNavigation.FullnameCalc,
                    Email = initiative.LastUpdateUserNavigation.Email,
                },
            FinancialImpact = initiative.FinancialImpactNavigation == null
                ? null
                : new InitiativeLookupValueDto
                {
                    Id = initiative.FinancialImpactNavigation.Id,
                    Name = initiative.FinancialImpactNavigation.Name,
                },
            StrategicImportance = initiative.StrategicImportanceNavigation == null
                ? null
                : new InitiativeLookupValueDto
                {
                    Id = initiative.StrategicImportanceNavigation.Id,
                    Name = initiative.StrategicImportanceNavigation.Name,
                },
            Collections = initiative.Collections.OrderBy(y => y.Name)
                .Select(y => new InitiativeLinkedCollectionDto
                {
                    Id = y.CollectionId,
                    Name = y.Name,
                    Description = y.Description,
                })
                .ToList(),
        };
    }

    public async Task<InitiativeDetailDto> CreateInitiativeAsync(
        ClaimsPrincipal user,
        CreateInitiativeRequestDto request,
        CancellationToken cancellationToken
    )
    {
        await ValidateLinkedIdsAsync(request.CollectionIds, cancellationToken);

        var initiative = new Initiative
        {
            Name = request.Name,
            Description = request.Description,
            OperationOwnerId = request.OperationOwnerId,
            ExecutiveOwnerId = request.ExecutiveOwnerId,
            FinancialImpact = request.FinancialImpact,
            StrategicImportance = request.StrategicImportance,
            Hidden = NormalizeFlag(request.Hidden),
            LastUpdateUser = user.GetUserId(),
            LastUpdateDate = DateTime.Now,
        };

        await _context.Initiatives.AddAsync(initiative, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await SynchronizeCollectionsAsync(initiative.InitiativeId, request.CollectionIds, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        InvalidateInitiativeCaches(initiative.InitiativeId, request.CollectionIds);
        return await GetInitiativeAsync(user, initiative.InitiativeId, cancellationToken);
    }

    public async Task<InitiativeDetailDto> UpdateInitiativeAsync(
        ClaimsPrincipal user,
        int id,
        UpdateInitiativeRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var initiative = await _context.Initiatives.SingleOrDefaultAsync(
            x => x.InitiativeId == id,
            cancellationToken
        );
        if (initiative == null)
        {
            return null;
        }

        await ValidateLinkedIdsAsync(request.CollectionIds, cancellationToken);

        var existingCollectionIds = await _context.Collections.AsNoTracking()
            .Where(x => x.InitiativeId == id)
            .Select(x => x.CollectionId)
            .ToListAsync(cancellationToken);

        initiative.Name = request.Name;
        initiative.Description = request.Description;
        initiative.OperationOwnerId = request.OperationOwnerId;
        initiative.ExecutiveOwnerId = request.ExecutiveOwnerId;
        initiative.FinancialImpact = request.FinancialImpact;
        initiative.StrategicImportance = request.StrategicImportance;
        initiative.Hidden = NormalizeFlag(request.Hidden);
        initiative.LastUpdateUser = user.GetUserId();
        initiative.LastUpdateDate = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);
        await SynchronizeCollectionsAsync(id, request.CollectionIds, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        InvalidateInitiativeCaches(id, existingCollectionIds.Concat(request.CollectionIds).Distinct().ToArray());
        return await GetInitiativeAsync(user, id, cancellationToken);
    }

    public async Task<bool> DeleteInitiativeAsync(int id, CancellationToken cancellationToken)
    {
        var initiative = await _context.Initiatives.SingleOrDefaultAsync(
            x => x.InitiativeId == id,
            cancellationToken
        );
        if (initiative == null)
        {
            return false;
        }

        var linkedCollectionIds = await _context.Collections
            .Where(x => x.InitiativeId == id)
            .Select(x => x.CollectionId)
            .ToListAsync(cancellationToken);

        var collections = await _context.Collections.Where(x => x.InitiativeId == id).ToListAsync(cancellationToken);
        foreach (var collection in collections)
        {
            collection.InitiativeId = null;
        }

        _context.Initiatives.Remove(initiative);
        await _context.SaveChangesAsync(cancellationToken);

        InvalidateInitiativeCaches(id, linkedCollectionIds);
        return true;
    }

    public async Task<IReadOnlyList<InitiativeCollectionSearchResultDto>> SearchCollectionsAsync(
        string search,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Array.Empty<InitiativeCollectionSearchResultDto>();
        }

        var term = search.Trim();
        var lowered = term.ToLower();

        return await _context.Collections.AsNoTracking()
            .Where(x =>
                x.Name.ToLower().Contains(lowered)
                || (x.Description != null && x.Description.ToLower().Contains(lowered))
            )
            .OrderBy(x => x.Name)
            .Take(20)
            .Select(x => new InitiativeCollectionSearchResultDto
            {
                Id = x.CollectionId,
                Name = x.Name,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);
    }

    private InitiativeFeatureFlagsDto BuildFeatures()
    {
        return new InitiativeFeatureFlagsDto
        {
            UserProfilesEnabled = IsUserProfileEnabled(),
            FeedbackEnabled = IsFeatureEnabled("features:enable_feedback"),
            SharingEnabled = IsFeatureEnabled("features:enable_sharing"),
        };
    }

    private InitiativePermissionsDto BuildPermissions(ClaimsPrincipal user)
    {
        return new InitiativePermissionsDto
        {
            CanCreateInitiative = user.HasPermission("Create Initiative"),
            CanEditInitiative = user.HasPermission("Edit Initiative"),
            CanDeleteInitiative = user.HasPermission("Delete Initiative"),
            CanViewUserProfiles = user.HasPermission("View Other User"),
        };
    }

    private bool IsUserProfileEnabled()
    {
        return IsFeatureEnabled("features:enable_user_profile");
    }

    private bool IsFeatureEnabled(string key)
    {
        var value = _configuration[key];
        return string.IsNullOrWhiteSpace(value)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeFlag(string value)
    {
        return string.Equals(value, "Y", StringComparison.OrdinalIgnoreCase) ? "Y" : "N";
    }

    private async Task ValidateLinkedIdsAsync(
        IReadOnlyList<int> collectionIds,
        CancellationToken cancellationToken
    )
    {
        if (collectionIds.Count == 0)
        {
            return;
        }

        var validIds = await _context.Collections.AsNoTracking()
            .Where(x => collectionIds.Contains(x.CollectionId))
            .Select(x => x.CollectionId)
            .ToListAsync(cancellationToken);

        var missing = collectionIds.Except(validIds).ToArray();
        if (missing.Length > 0)
        {
            throw new InvalidOperationException("One or more linked collections do not exist.");
        }
    }

    private async Task SynchronizeCollectionsAsync(
        int initiativeId,
        IReadOnlyList<int> collectionIds,
        CancellationToken cancellationToken
    )
    {
        var existing = await _context.Collections.Where(x => x.InitiativeId == initiativeId)
            .ToListAsync(cancellationToken);

        foreach (var collection in existing.Where(x => !collectionIds.Contains(x.CollectionId)))
        {
            collection.InitiativeId = null;
        }

        if (collectionIds.Count == 0)
        {
            return;
        }

        var added = await _context.Collections.Where(x => collectionIds.Contains(x.CollectionId))
            .ToListAsync(cancellationToken);

        foreach (var collection in added)
        {
            collection.InitiativeId = initiativeId;
        }
    }

    private void InvalidateInitiativeCaches(int initiativeId, IEnumerable<int> collectionIds)
    {
        _cache.Remove("initiative-" + initiativeId);
        _cache.Remove("initiatives");
        _cache.Remove("collections");

        foreach (var collectionId in collectionIds.Distinct())
        {
            _cache.Remove("collection-" + collectionId);
        }
    }
}
