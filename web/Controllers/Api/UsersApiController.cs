using Atlas_Web.Contracts.Api.Users;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/users")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UsersApiController : ControllerBase
{
    private readonly IUsersApiService _usersApiService;

    public UsersApiController(IUsersApiService usersApiService)
    {
        _usersApiService = usersApiService;
    }

    private int CurrentUserId => Int32.Parse(User.FindFirst("UserId")!.Value);

    private bool CanManageWorkspaceFor(int userId)
    {
        return userId == CurrentUserId || User.HasPermission("Edit Other Users");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserPageDto>> GetUserPage(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var userPage = await _usersApiService.GetUserPageAsync(User, id, cancellationToken);
        if (userPage == null)
        {
            return NotFound();
        }

        return Ok(userPage);
    }

    [HttpGet("{id:int}/stars")]
    public async Task<ActionResult<UserStarsDto>> GetStars(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetStarsAsync(User, id, cancellationToken));
    }

    [HttpGet("{id:int}/groups")]
    public async Task<ActionResult<IReadOnlyList<UserGroupDto>>> GetGroups(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetGroupsAsync(User, id, cancellationToken));
    }

    [HttpGet("{id:int}/subscriptions")]
    public async Task<ActionResult<IReadOnlyList<UserSubscriptionDto>>> GetSubscriptions(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetSubscriptionsAsync(User, id, cancellationToken));
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<UserHistorySectionDto>> GetHistory(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetHistoryAsync(User, id, cancellationToken));
    }

    [HttpGet("me/shared-objects")]
    public async Task<ActionResult<UserSharedObjectsDto>> GetSharedObjects(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetSharedObjectsAsync(User, cancellationToken));
    }

    [HttpGet("me/search-history")]
    public async Task<ActionResult<IReadOnlyList<UserSearchHistoryItemDto>>> GetSearchHistory(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _usersApiService.GetSearchHistoryAsync(User, cancellationToken));
    }

    [HttpPost("me/folders")]
    public async Task<ActionResult<UserFavoriteFolderDto>> CreateFolder(
        [FromBody] CreateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        var folder = await _usersApiService.CreateFolderAsync(CurrentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetSearchHistory), new { }, folder);
    }

    [HttpPost("{id:int}/folders")]
    public async Task<ActionResult<UserFavoriteFolderDto>> CreateFolderForUser(
        int id,
        [FromBody] CreateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!CanManageWorkspaceFor(id))
        {
            return Forbid();
        }

        var folder = await _usersApiService.CreateFolderAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetStars), new { id }, folder);
    }

    [HttpPut("me/folders/{id:int}")]
    public async Task<ActionResult<UserFavoriteFolderDto>> UpdateFolder(
        int id,
        [FromBody] UpdateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        var folder = await _usersApiService.UpdateFolderAsync(CurrentUserId, id, request, cancellationToken);
        if (folder == null)
        {
            return NotFound();
        }

        return Ok(folder);
    }

    [HttpPut("{userId:int}/folders/{id:int}")]
    public async Task<ActionResult<UserFavoriteFolderDto>> UpdateFolderForUser(
        int userId,
        int id,
        [FromBody] UpdateUserFavoriteFolderRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!CanManageWorkspaceFor(userId))
        {
            return Forbid();
        }

        var folder = await _usersApiService.UpdateFolderAsync(userId, id, request, cancellationToken);
        if (folder == null)
        {
            return NotFound();
        }

        return Ok(folder);
    }

    [HttpDelete("me/folders/{id:int}")]
    public async Task<IActionResult> DeleteFolder(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _usersApiService.DeleteFolderAsync(CurrentUserId, id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{userId:int}/folders/{id:int}")]
    public async Task<IActionResult> DeleteFolderForUser(
        int userId,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!CanManageWorkspaceFor(userId))
        {
            return Forbid();
        }

        var deleted = await _usersApiService.DeleteFolderAsync(userId, id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("me/folders/reorder")]
    public async Task<IActionResult> ReorderFolders(
        [FromBody] IReadOnlyList<ReorderUserFavoriteFolderItemDto> request,
        CancellationToken cancellationToken = default
    )
    {
        await _usersApiService.ReorderFoldersAsync(CurrentUserId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/folders/reorder")]
    public async Task<IActionResult> ReorderFoldersForUser(
        int id,
        [FromBody] IReadOnlyList<ReorderUserFavoriteFolderItemDto> request,
        CancellationToken cancellationToken = default
    )
    {
        if (id != CurrentUserId)
        {
            return Forbid();
        }

        await _usersApiService.ReorderFoldersAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("me/favorites/reorder")]
    public async Task<IActionResult> ReorderFavorites(
        [FromBody] IReadOnlyList<ReorderUserFavoriteItemDto> request,
        CancellationToken cancellationToken = default
    )
    {
        await _usersApiService.ReorderFavoritesAsync(CurrentUserId, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/favorites/reorder")]
    public async Task<IActionResult> ReorderFavoritesForUser(
        int id,
        [FromBody] IReadOnlyList<ReorderUserFavoriteItemDto> request,
        CancellationToken cancellationToken = default
    )
    {
        if (id != CurrentUserId)
        {
            return Forbid();
        }

        await _usersApiService.ReorderFavoritesAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("me/favorites/folder")]
    public async Task<IActionResult> UpdateFavoriteFolder(
        [FromBody] UpdateUserFavoriteFolderAssignmentRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        var updated = await _usersApiService.UpdateFavoriteFolderAsync(
            CurrentUserId,
            request,
            cancellationToken
        );
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id:int}/favorites/folder")]
    public async Task<IActionResult> UpdateFavoriteFolderForUser(
        int id,
        [FromBody] UpdateUserFavoriteFolderAssignmentRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!CanManageWorkspaceFor(id))
        {
            return Forbid();
        }

        var updated = await _usersApiService.UpdateFavoriteFolderAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("me/shared-objects/{id:int}")]
    public async Task<IActionResult> RemoveSharedObject(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var removed = await _usersApiService.RemoveSharedObjectAsync(User, id, cancellationToken);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("me/favorites/toggle")]
    public async Task<ActionResult<ToggleUserFavoriteResponseDto>> ToggleFavorite(
        [FromBody] ToggleUserFavoriteRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok(await _usersApiService.ToggleFavoriteAsync(CurrentUserId, request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:int}/favorites/toggle")]
    public async Task<ActionResult<ToggleUserFavoriteResponseDto>> ToggleFavoriteForUser(
        int id,
        [FromBody] ToggleUserFavoriteRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!CanManageWorkspaceFor(id))
        {
            return Forbid();
        }

        try
        {
            return Ok(await _usersApiService.ToggleFavoriteAsync(id, request, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("me/admin-mode/toggle")]
    public async Task<ActionResult<ToggleAdminModeResponseDto>> ToggleAdminMode(
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Administrator"))
        {
            return Forbid();
        }

        return Ok(await _usersApiService.ToggleAdminModeAsync(User, cancellationToken));
    }
}
