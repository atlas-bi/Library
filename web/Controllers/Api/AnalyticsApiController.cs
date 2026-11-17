using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Analytics;
using Atlas_Web.Helpers;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/analytics")]
[Authorize(AuthenticationSchemes = "Bearer")]
public sealed class AnalyticsApiController : ControllerBase
{
    private readonly IAnalyticsApiService _analyticsApiService;

    public AnalyticsApiController(IAnalyticsApiService analyticsApiService)
    {
        _analyticsApiService = analyticsApiService;
    }

    [HttpGet("live-users")]
    public async Task<ActionResult<AnalyticsLiveUsersResponseDto>> GetLiveUsers(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetLiveUsersAsync(cancellationToken));
    }

    [HttpPost("beacon")]
    public async Task<IActionResult> RecordBeacon(
        [FromBody] AnalyticsBeaconRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await _analyticsApiService.RecordBeaconAsync(
            User.GetUserId(),
            HttpContext.IsHyperspace(),
            request,
            cancellationToken
        );
        return Ok(new { status = "ok" });
    }

    [HttpGet("visits")]
    public async Task<ActionResult<AnalyticsVisitsResponseDto>> GetVisits(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetVisitsAsync(request, cancellationToken));
    }

    [HttpGet("visits/browsers")]
    public async Task<ActionResult<IReadOnlyList<AnalyticsBarItemDto>>> GetBrowsers(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetBrowsersAsync(request, cancellationToken));
    }

    [HttpGet("visits/os")]
    public async Task<ActionResult<IReadOnlyList<AnalyticsBarItemDto>>> GetOs(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetOsAsync(request, cancellationToken));
    }

    [HttpGet("visits/resolution")]
    public async Task<ActionResult<IReadOnlyList<AnalyticsBarItemDto>>> GetResolution(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetResolutionAsync(request, cancellationToken));
    }

    [HttpGet("visits/users")]
    public async Task<ActionResult<IReadOnlyList<AnalyticsBarItemDto>>> GetUsers(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetUsersAsync(request, cancellationToken));
    }

    [HttpGet("visits/load-times")]
    public async Task<ActionResult<IReadOnlyList<AnalyticsBarItemDto>>> GetLoadTimes(
        [FromQuery] AnalyticsQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetLoadTimesAsync(request, cancellationToken));
    }

    [HttpGet("traces")]
    public async Task<ActionResult<AnalyticsTraceListResponseDto>> GetTraces(
        [FromQuery] AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetTracesAsync(request, cancellationToken));
    }

    [HttpPost("traces")]
    public async Task<IActionResult> RecordTraces(
        [FromBody] AnalyticsTraceIngestRequest request,
        [FromHeader(Name = "User-Agent")] string userAgent = null,
        [FromHeader(Name = "Referer")] string referer = null,
        CancellationToken cancellationToken = default
    )
    {
        await _analyticsApiService.RecordTracesAsync(
            User.GetUserId(),
            userAgent,
            referer,
            request,
            cancellationToken
        );
        return Ok(new { status = "ok" });
    }

    [HttpPost("traces/{id:int}/resolve")]
    public async Task<IActionResult> ResolveTrace(
        int id,
        [FromQuery] int type = 1,
        CancellationToken cancellationToken = default
    )
    {
        await _analyticsApiService.ResolveTraceAsync(id, type, cancellationToken);
        return Ok(new { status = "ok" });
    }

    [HttpGet("errors")]
    public async Task<ActionResult<AnalyticsErrorListResponseDto>> GetErrors(
        [FromQuery] AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _analyticsApiService.GetErrorsAsync(request, cancellationToken));
    }

    [HttpPost("errors/{id:int}/resolve")]
    public async Task<IActionResult> ResolveError(
        int id,
        [FromQuery] int type = 1,
        CancellationToken cancellationToken = default
    )
    {
        await _analyticsApiService.ResolveErrorAsync(id, type, cancellationToken);
        return Ok(new { status = "ok" });
    }
}
