using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Terms;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/terms")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TermsApiController : ControllerBase
{
    private readonly ITermsApiService _termsApiService;

    public TermsApiController(ITermsApiService termsApiService)
    {
        _termsApiService = termsApiService;
    }

    [HttpGet]
    public async Task<ActionResult<TermsListDto>> GetTerms(CancellationToken cancellationToken = default)
    {
        return Ok(await _termsApiService.GetTermsAsync(User, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TermDetailDto>> GetTerm(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var term = await _termsApiService.GetTermAsync(User, id, cancellationToken);
        if (term == null)
        {
            return NotFound();
        }

        return Ok(term);
    }

    [HttpGet("{id:int}/reports")]
    public async Task<ActionResult<IReadOnlyList<TermRelatedReportDto>>> GetTermReports(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var term = await _termsApiService.GetTermAsync(User, id, cancellationToken);
        if (term == null)
        {
            return NotFound();
        }

        return Ok(await _termsApiService.GetTermReportsAsync(User, id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<TermDetailDto>> CreateTerm(
        [FromBody] CreateTermRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Create New Terms"))
        {
            return Forbid();
        }

        var term = await _termsApiService.CreateTermAsync(User, request, cancellationToken);
        return CreatedAtAction(nameof(GetTerm), new { id = term.Id }, term);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TermDetailDto>> UpdateTerm(
        int id,
        [FromBody] UpdateTermRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        var existing = await _termsApiService.GetTermAsync(User, id, cancellationToken);
        if (existing == null)
        {
            return NotFound();
        }

        if (!existing.Permissions.CanEditTerm)
        {
            return Forbid();
        }

        var term = await _termsApiService.UpdateTermAsync(User, id, request, cancellationToken);
        return Ok(term);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTerm(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _termsApiService.GetTermAsync(User, id, cancellationToken);
        if (existing == null)
        {
            return NotFound();
        }

        if (!existing.Permissions.CanDeleteTerm)
        {
            return Forbid();
        }

        await _termsApiService.DeleteTermAsync(id, cancellationToken);
        return NoContent();
    }
}
