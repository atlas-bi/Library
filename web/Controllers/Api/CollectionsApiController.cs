using System.ComponentModel.DataAnnotations;
using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Collections;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/collections")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CollectionsApiController : ControllerBase
{
    private readonly ICollectionsApiService _collectionsApiService;

    public CollectionsApiController(ICollectionsApiService collectionsApiService)
    {
        _collectionsApiService = collectionsApiService;
    }

    [HttpGet]
    public async Task<ActionResult<CollectionListResponseDto>> GetCollections(
        [FromQuery]
        [Range(1, int.MaxValue)]
            int page = 1,
        [FromQuery]
        [Range(1, 100)]
            int pageSize = 20,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _collectionsApiService.GetCollectionsAsync(
            User,
            page,
            pageSize,
            cancellationToken
        );
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CollectionDetailDto>> GetCollection(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var collection = await _collectionsApiService.GetCollectionAsync(
            User,
            id,
            cancellationToken
        );
        if (collection == null)
        {
            return NotFound();
        }

        return Ok(collection);
    }

    [HttpPost]
    public async Task<ActionResult<CollectionDetailDto>> CreateCollection(
        [FromBody] CreateCollectionRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Create Collection"))
        {
            return Forbid();
        }

        try
        {
            var collection = await _collectionsApiService.CreateCollectionAsync(
                User,
                request,
                cancellationToken
            );

            return CreatedAtAction(nameof(GetCollection), new { id = collection.Id }, collection);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CollectionDetailDto>> UpdateCollection(
        int id,
        [FromBody] UpdateCollectionRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Edit Collection"))
        {
            return Forbid();
        }

        try
        {
            var collection = await _collectionsApiService.UpdateCollectionAsync(
                User,
                id,
                request,
                cancellationToken
            );
            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCollection(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        if (!User.HasPermission("Delete Collection"))
        {
            return Forbid();
        }

        var deleted = await _collectionsApiService.DeleteCollectionAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("search/terms")]
    public async Task<ActionResult<IReadOnlyList<CollectionSearchResultDto>>> SearchTerms(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _collectionsApiService.SearchTermsAsync(q, cancellationToken));
    }

    [HttpGet("search/reports")]
    public async Task<ActionResult<IReadOnlyList<CollectionSearchResultDto>>> SearchReports(
        [FromQuery] string q,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _collectionsApiService.SearchReportsAsync(q, cancellationToken));
    }
}
