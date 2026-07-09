using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Initiatives;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/initiatives")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class InitiativesApiController : ControllerBase
{
    private readonly IInitiativesApiService _initiativesApiService;

    public InitiativesApiController(IInitiativesApiService initiativesApiService)
    {
        _initiativesApiService = initiativesApiService;
    }

    [HttpGet]
    public async Task<ActionResult<InitiativeListResponseDto>> GetInitiatives(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _initiativesApiService.GetInitiativesAsync(User, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InitiativeDetailDto>> GetInitiative(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var initiative = await _initiativesApiService.GetInitiativeAsync(User, id, cancellationToken);
        if (initiative == null)
        {
            return NotFound();
        }

        return Ok(initiative);
    }

    [HttpPost]
    public async Task<ActionResult<InitiativeDetailDto>> CreateInitiative(
        [FromBody] CreateInitiativeRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Create Initiative"))
        {
            return Forbid();
        }

        try
        {
            var initiative = await _initiativesApiService.CreateInitiativeAsync(
                User,
                request,
                cancellationToken
            );
            return CreatedAtAction(nameof(GetInitiative), new { id = initiative.Id }, initiative);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InitiativeDetailDto>> UpdateInitiative(
        int id,
        [FromBody] UpdateInitiativeRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Edit Initiative"))
        {
            return Forbid();
        }

        try
        {
            var initiative = await _initiativesApiService.UpdateInitiativeAsync(
                User,
                id,
                request,
                cancellationToken
            );
            if (initiative == null)
            {
                return NotFound();
            }

            return Ok(initiative);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInitiative(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Delete Initiative"))
        {
            return Forbid();
        }

        var deleted = await _initiativesApiService.DeleteInitiativeAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("search/collections")]
    public async Task<ActionResult<IReadOnlyList<InitiativeCollectionSearchResultDto>>> SearchCollections(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _initiativesApiService.SearchCollectionsAsync(q, cancellationToken));
    }
}
