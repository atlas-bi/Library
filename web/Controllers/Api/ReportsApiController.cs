using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/reports")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ReportsApiController : ControllerBase
{
    private readonly Atlas_WebContext _context;

    public ReportsApiController(Atlas_WebContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReports([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var reports = await _context.ReportObjects
            .Where(x => x.DefaultVisibilityYn == "Y")
            .Include(x => x.ReportObjectType)
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                id = x.ReportObjectId,
                name = x.Name ?? x.DisplayTitle,
                description = x.Description,
                type = x.ReportObjectType != null ? x.ReportObjectType.ShortName : null,
                url = x.ReportObjectUrl,
                lastModified = x.LastModifiedDate,
            })
            .ToListAsync();

        var total = await _context.ReportObjects.CountAsync(x => x.DefaultVisibilityYn == "Y");

        return Ok(new { reports, total, page, pageSize });
    }
}
