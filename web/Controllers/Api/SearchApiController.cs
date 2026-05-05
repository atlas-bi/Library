using System.ComponentModel.DataAnnotations;
using Atlas_Web.Contracts.Api.Search;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/search")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class SearchApiController : ControllerBase
{
    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "q", "type", "page", "pageSize", "field", "advanced",
    };

    private readonly ISearchApiService _searchApiService;

    public SearchApiController(ISearchApiService searchApiService)
    {
        _searchApiService = searchApiService;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResponseDto>> Search(
        [FromQuery] string q = "",
        [FromQuery] string type = "query",
        [FromQuery]
        [Range(1, int.MaxValue)]
            int page = 1,
        [FromQuery]
        [Range(1, 100)]
            int pageSize = 20,
        [FromQuery] string field = null,
        [FromQuery] string advanced = null,
        CancellationToken cancellationToken = default
    )
    {
        var filters = Request.Query
            .Where(kv => !ReservedKeys.Contains(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        var response = await _searchApiService.SearchAsync(
            User,
            q ?? string.Empty,
            type,
            page,
            pageSize,
            field,
            advanced == "Y",
            filters,
            cancellationToken
        );

        return Ok(response);
    }
}
