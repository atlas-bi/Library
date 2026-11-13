using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/reports")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ReportFilesApiController : ControllerBase
{
    private readonly IReportFileApiService _reportFileApiService;

    public ReportFilesApiController(IReportFileApiService reportFileApiService)
    {
        _reportFileApiService = reportFileApiService;
    }

    [HttpGet("{id:int}/cube")]
    public async Task<IActionResult> GetCube(int id, CancellationToken cancellationToken = default)
    {
        var result = await _reportFileApiService.GetCubeAsync(id, cancellationToken);
        if (result.Status == ReportFileStatus.NotFound)
        {
            return NotFound();
        }

        return File(result.Bytes, result.ContentType, result.FileName);
    }

    [HttpGet("attachments/{id:int}")]
    public async Task<IActionResult> GetAttachment(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _reportFileApiService.GetCrystalAttachmentAsync(
            User,
            id,
            cancellationToken
        );
        if (result.Status == ReportFileStatus.NotFound)
        {
            return NotFound();
        }

        if (result.Status == ReportFileStatus.Forbidden)
        {
            return Forbid();
        }

        Response.Headers.ContentDisposition = ReportFileApiService.BuildContentDisposition(result.FileName);
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(result.Bytes, result.ContentType);
    }
}
