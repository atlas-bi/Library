using Atlas_Web.Contracts.Api.Profile;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/profile")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProfileApiController : ControllerBase
{
    private readonly IProfileApiService _profileApiService;

    public ProfileApiController(IProfileApiService profileApiService)
    {
        _profileApiService = profileApiService;
    }

    [HttpGet("chart")]
    public async Task<ActionResult<ProfileChartResponseDto>> GetChart(
        [FromQuery] ProfileQueryRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetChartAsync(request, cancellationToken));
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetUsers(
        [FromQuery] ProfileQueryRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetUsersAsync(request, cancellationToken));
    }

    [HttpGet("reports")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetReports(
        [FromQuery] ProfileQueryRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetReportsAsync(request, cancellationToken));
    }

    [HttpGet("fails")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetFails(
        [FromQuery] ProfileQueryRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetFailsAsync(request, cancellationToken));
    }

    [HttpGet("run-list")]
    public async Task<ActionResult<IReadOnlyList<ProfileRunListItemDto>>> GetRunList(
        [FromQuery] int id = -1,
        [FromQuery] string type = "user",
        [FromQuery] List<int> reportType = null,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetRunListAsync(id, type, reportType, cancellationToken));
    }

    [HttpGet("stars")]
    public async Task<ActionResult<IReadOnlyList<ProfileStarUserDto>>> GetStars(
        [FromQuery] int id,
        [FromQuery] string type,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetStarsAsync(id, type, cancellationToken));
    }

    [HttpGet("subscriptions")]
    public async Task<ActionResult<IReadOnlyList<ProfileSubscriptionDto>>> GetSubscriptions(
        [FromQuery] int id,
        [FromQuery] string type,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _profileApiService.GetSubscriptionsAsync(id, type, cancellationToken));
    }
}
