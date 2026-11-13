using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/reports")]
[Authorize(AuthenticationSchemes = "Bearer")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class ReportImagesApiController : ControllerBase
{
    private readonly IReportImageApiService _reportImageApiService;

    public ReportImagesApiController(IReportImageApiService reportImageApiService)
    {
        _reportImageApiService = reportImageApiService;
    }

    [HttpGet("{id:int}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(
        int id,
        [FromQuery] string size,
        [FromQuery] int? imgId,
        [FromQuery] string type,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _reportImageApiService.GetThumbnailAsync(
            id,
            size,
            imgId,
            type,
            cancellationToken
        );
        return ToFile(result);
    }

    [HttpGet("images/placeholder")]
    public async Task<IActionResult> GetPlaceholder(
        [FromQuery] string size,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _reportImageApiService.GetPlaceholderAsync(size, cancellationToken);
        return ToFile(result);
    }

    private IActionResult ToFile(ReportImageResult result)
    {
        if (result.Status == ReportImageStatus.NotFound)
        {
            return NotFound();
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(result.Bytes, result.ContentType);
    }
}
