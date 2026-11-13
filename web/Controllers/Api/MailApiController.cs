using Atlas_Web.Contracts.Api.Interactions;
using Atlas_Web.Contracts.Api.Mail;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/mail")]
[Authorize(AuthenticationSchemes = "Bearer")]
public sealed class MailApiController : ControllerBase
{
    private readonly IMailApiService _mailApiService;
    private readonly IInteractionsApiService _interactionsApiService;

    public MailApiController(
        IMailApiService mailApiService,
        IInteractionsApiService interactionsApiService
    )
    {
        _mailApiService = mailApiService;
        _interactionsApiService = interactionsApiService;
    }

    [HttpGet]
    public async Task<ActionResult<MailMailboxDto>> GetMailbox(
        [FromQuery] string folder = "inbox",
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _mailApiService.GetMailboxAsync(User, folder, cancellationToken));
    }

    [HttpGet("messages/{id:int}")]
    public async Task<ActionResult<MailMessageDetailDto>> GetMessage(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var message = await _mailApiService.GetMessageAsync(User, id, cancellationToken);
        return message == null ? NotFound() : Ok(message);
    }

    [HttpGet("drafts/{id:int}")]
    public async Task<ActionResult<MailDraftDetailDto>> GetDraft(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        var draft = await _mailApiService.GetDraftAsync(User, id, cancellationToken);
        return draft == null ? NotFound() : Ok(draft);
    }

    [HttpPost("check")]
    public async Task<ActionResult<MailCheckResponseDto>> CheckForMail(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _mailApiService.CheckForMailAsync(User, cancellationToken));
    }

    [HttpPost("messages/{id:int}/read")]
    public async Task<IActionResult> MarkMessageRead(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await _mailApiService.MarkMessageReadAsync(User, id, cancellationToken)
            ? Ok()
            : NotFound();
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MailSendResponseDto>> SendMessage(
        [FromBody] MailSendRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var sent = await _interactionsApiService.SendShareMailAsync(
                User,
                new ShareMailRequestDto
                {
                    DraftId = request?.DraftId,
                    To = (request?.To ?? Array.Empty<MailSendRecipientDto>())
                        .Select(x => new ShareRecipientDto { UserId = x.UserId, Type = x.Type })
                        .ToList(),
                    Subject = request?.Subject,
                    Message = request?.Message,
                    Text = request?.Text,
                    Share = request?.Share ?? false,
                    ShareName = request?.ShareName,
                    ShareUrl = request?.ShareUrl,
                },
                cancellationToken
            );
            return Ok(
                new MailSendResponseDto
                {
                    Message = sent.Message,
                    RecipientCount = sent.RecipientCount,
                    ShareCount = sent.ShareCount,
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("drafts")]
    public async Task<ActionResult<MailDraftSaveResponseDto>> SaveDraft(
        [FromBody] MailDraftSaveRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        var saved = await _mailApiService.SaveDraftAsync(User, request, cancellationToken);
        return saved == null ? NotFound() : Ok(saved);
    }

    [HttpDelete("messages/{id:int}")]
    public async Task<IActionResult> DeleteMessage(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await _mailApiService.DeleteMessageAsync(User, id, cancellationToken)
            ? Ok()
            : NotFound();
    }

    [HttpDelete("drafts/{id:int}")]
    public async Task<IActionResult> DeleteDraft(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await _mailApiService.DeleteDraftAsync(User, id, cancellationToken)
            ? Ok()
            : NotFound();
    }
}
