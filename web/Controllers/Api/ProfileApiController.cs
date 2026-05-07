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
        [FromQuery] int id,
        [FromQuery] string type,
        [FromQuery] double start_at = -31536000,
        [FromQuery] double end_at = 0,
        [FromQuery] List<string> server = null,
        [FromQuery] List<string> database = null,
        [FromQuery] List<string> masterFile = null,
        [FromQuery] List<string> visible = null,
        [FromQuery] List<string> certification = null,
        [FromQuery] List<string> availability = null,
        [FromQuery] List<int> reportType = null,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await _profileApiService.GetChartAsync(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType,
                cancellationToken
            )
        );
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetUsers(
        [FromQuery] int id,
        [FromQuery] string type,
        [FromQuery] double start_at = -31536000,
        [FromQuery] double end_at = 0,
        [FromQuery] List<string> server = null,
        [FromQuery] List<string> database = null,
        [FromQuery] List<string> masterFile = null,
        [FromQuery] List<string> visible = null,
        [FromQuery] List<string> certification = null,
        [FromQuery] List<string> availability = null,
        [FromQuery] List<int> reportType = null,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await _profileApiService.GetUsersAsync(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType,
                cancellationToken
            )
        );
    }

    [HttpGet("reports")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetReports(
        [FromQuery] int id,
        [FromQuery] string type,
        [FromQuery] double start_at = -31536000,
        [FromQuery] double end_at = 0,
        [FromQuery] List<string> server = null,
        [FromQuery] List<string> database = null,
        [FromQuery] List<string> masterFile = null,
        [FromQuery] List<string> visible = null,
        [FromQuery] List<string> certification = null,
        [FromQuery] List<string> availability = null,
        [FromQuery] List<int> reportType = null,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await _profileApiService.GetReportsAsync(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType,
                cancellationToken
            )
        );
    }

    [HttpGet("fails")]
    public async Task<ActionResult<IReadOnlyList<ProfileBarItemDto>>> GetFails(
        [FromQuery] int id,
        [FromQuery] string type,
        [FromQuery] double start_at = -31536000,
        [FromQuery] double end_at = 0,
        [FromQuery] List<string> server = null,
        [FromQuery] List<string> database = null,
        [FromQuery] List<string> masterFile = null,
        [FromQuery] List<string> visible = null,
        [FromQuery] List<string> certification = null,
        [FromQuery] List<string> availability = null,
        [FromQuery] List<int> reportType = null,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await _profileApiService.GetFailsAsync(
                id,
                type,
                start_at,
                end_at,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType,
                cancellationToken
            )
        );
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
