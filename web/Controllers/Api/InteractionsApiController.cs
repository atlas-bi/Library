using Atlas_Web.Contracts.Api.Interactions;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/interactions")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class InteractionsApiController : ControllerBase
{
    private readonly IInteractionsApiService _interactionsApiService;

    public InteractionsApiController(IInteractionsApiService interactionsApiService)
    {
        _interactionsApiService = interactionsApiService;
    }

    [HttpPost("stars/toggle")]
    public async Task<ActionResult<ToggleStarResponseDto>> ToggleStar(
        [FromBody] ToggleStarRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await _interactionsApiService.ToggleStarAsync(
                User,
                request,
                cancellationToken
            );
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("share-mail")]
    public async Task<ActionResult<ShareMailResponseDto>> ShareMail(
        [FromBody] ShareMailRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok(
                await _interactionsApiService.SendShareMailAsync(User, request, cancellationToken)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("feedback")]
    public async Task<ActionResult> ShareFeedback(
        [FromBody] ShareFeedbackRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok(
                await _interactionsApiService.SendFeedbackAsync(User, request, cancellationToken)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("access-request")]
    public async Task<ActionResult> SubmitAccessRequest(
        [FromBody] AccessRequestRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok(
                await _interactionsApiService.SubmitAccessRequestAsync(
                    User,
                    request,
                    cancellationToken
                )
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { error = "Access request could not be submitted." });
        }
    }

    [HttpGet("search/recipients")]
    public async Task<ActionResult<IReadOnlyList<RecipientSearchResultDto>>> SearchRecipients(
        [FromQuery] string q,
        [FromQuery] bool includeGroups = true,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await _interactionsApiService.SearchRecipientsAsync(
                q,
                includeGroups,
                cancellationToken
            )
        );
    }
}
