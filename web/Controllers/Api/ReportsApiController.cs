using System.ComponentModel.DataAnnotations;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Reports;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/reports")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ReportsApiController : ControllerBase
{
    private readonly IReportsApiService _reportsApiService;

    public ReportsApiController(IReportsApiService reportsApiService)
    {
        _reportsApiService = reportsApiService;
    }

    [HttpGet]
    public async Task<ActionResult<ReportListResponseDto>> GetReports(
        [FromQuery]
        [Range(1, int.MaxValue)]
            int page = 1,
        [FromQuery]
        [Range(1, 100)]
            int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _reportsApiService.GetReportsAsync(
            User,
            page,
            pageSize,
            cancellationToken
        );
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReportDetailDto>> GetReport(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var report = await _reportsApiService.GetReportAsync(User, id, cancellationToken);
        if (report == null)
        {
            return NotFound();
        }

        return Ok(report);
    }

    [HttpGet("{id:int}/terms")]
    public async Task<ActionResult<IReadOnlyList<TermSummaryDto>>> GetReportTerms(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var report = await _reportsApiService.GetReportAsync(User, id, cancellationToken);
        if (report == null)
        {
            return NotFound();
        }

        return Ok(await _reportsApiService.GetReportTermsAsync(User, id, cancellationToken));
    }

    [HttpGet("{id:int}/queries")]
    public async Task<ActionResult<ReportQueriesResponseDto>> GetReportQueries(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _reportsApiService.GetReportQueriesAsync(User, id, cancellationToken);
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/relationships")]
    public async Task<ActionResult<ReportRelationshipsResponseDto>> GetReportRelationships(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _reportsApiService.GetReportRelationshipsAsync(
            User,
            id,
            cancellationToken
        );
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpGet("{id:int}/maintenance-status")]
    public async Task<ActionResult<ReportMaintenanceStatusDto>> GetReportMaintenanceStatus(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _reportsApiService.GetReportMaintenanceStatusAsync(
            User,
            id,
            cancellationToken
        );
        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReportDetailDto>> UpdateReport(
        int id,
        [FromBody] UpdateReportDocumentRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Edit Report Documentation"))
        {
            return Forbid();
        }

        var report = await _reportsApiService.UpdateReportAsync(
            User,
            id,
            request,
            cancellationToken
        );
        if (report == null)
        {
            return NotFound();
        }

        return Ok(report);
    }

    [HttpPost("{id:int}/images")]
    [RequestSizeLimit(1024 * 1024)]
    public async Task<ActionResult<ReportImageDto>> AddImage(
        int id,
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Edit Report Documentation"))
        {
            return Forbid();
        }

        try
        {
            var image = await _reportsApiService.AddImageAsync(User, id, file, cancellationToken);
            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("lookups/{lookupArea}")]
    public async Task<ActionResult<IReadOnlyList<LookupDto>>> GetLookupValues(
        string lookupArea,
        CancellationToken cancellationToken = default
    )
    {
        var values = await _reportsApiService.GetLookupValuesAsync(lookupArea, cancellationToken);
        return Ok(values);
    }

    [HttpGet("search/terms")]
    public async Task<ActionResult<IReadOnlyList<ReportSearchResultDto>>> SearchTerms(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _reportsApiService.SearchTermsAsync(q, cancellationToken));
    }

    [HttpGet("search/collections")]
    public async Task<ActionResult<IReadOnlyList<ReportSearchResultDto>>> SearchCollections(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _reportsApiService.SearchCollectionsAsync(q, cancellationToken));
    }

    [HttpGet("search/users")]
    public async Task<ActionResult<IReadOnlyList<ReportSearchResultDto>>> SearchUsers(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _reportsApiService.SearchUsersAsync(q, cancellationToken));
    }
}
