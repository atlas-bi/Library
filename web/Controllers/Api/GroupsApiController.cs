using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Groups;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/groups")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class GroupsApiController : ControllerBase
{
    private readonly IGroupsApiService _groupsApiService;

    public GroupsApiController(IGroupsApiService groupsApiService)
    {
        _groupsApiService = groupsApiService;
    }

    [HttpGet]
    public async Task<ActionResult<GroupListResponseDto>> GetGroups(
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("View Groups"))
        {
            return Forbid();
        }

        return Ok(await _groupsApiService.GetGroupsAsync(User, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GroupDetailDto>> GetGroup(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("View Groups"))
        {
            return Forbid();
        }

        var group = await _groupsApiService.GetGroupAsync(User, id, cancellationToken);
        if (group == null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    [HttpGet("{id:int}/users")]
    public async Task<ActionResult<IReadOnlyList<GroupUserDto>>> GetGroupUsers(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("View Groups"))
        {
            return Forbid();
        }

        var users = await _groupsApiService.GetGroupUsersAsync(User, id, cancellationToken);
        if (users == null)
        {
            return NotFound();
        }

        return Ok(users);
    }

    [HttpGet("{id:int}/reports")]
    public async Task<ActionResult<IReadOnlyList<GroupReportDto>>> GetGroupReports(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("View Groups"))
        {
            return Forbid();
        }

        var reports = await _groupsApiService.GetGroupReportsAsync(User, id, cancellationToken);
        if (reports == null)
        {
            return NotFound();
        }

        return Ok(reports);
    }
}
