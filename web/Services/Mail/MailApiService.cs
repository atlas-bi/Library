using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Mail;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface IMailApiService
{
    Task<MailMailboxDto> GetMailboxAsync(
        ClaimsPrincipal user,
        string folder,
        CancellationToken cancellationToken
    );
    Task<MailMessageDetailDto> GetMessageAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    );
    Task<MailDraftDetailDto> GetDraftAsync(
        ClaimsPrincipal user,
        int draftId,
        CancellationToken cancellationToken
    );
    Task<MailCheckResponseDto> CheckForMailAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    );
    Task<bool> MarkMessageReadAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    );
    Task<MailDraftSaveResponseDto> SaveDraftAsync(
        ClaimsPrincipal user,
        MailDraftSaveRequestDto request,
        CancellationToken cancellationToken
    );
    Task<bool> DeleteMessageAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    );
    Task<bool> DeleteDraftAsync(
        ClaimsPrincipal user,
        int draftId,
        CancellationToken cancellationToken
    );
}

public sealed class MailApiService : IMailApiService
{
    private readonly Atlas_WebContext _context;

    public MailApiService(Atlas_WebContext context)
    {
        _context = context;
    }

    public async Task<MailMailboxDto> GetMailboxAsync(
        ClaimsPrincipal user,
        string folder,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var draftCount = await _context.MailDrafts.CountAsync(
            x => x.FromUserId == userId,
            cancellationToken
        );
        var messages = await LoadInboxAsync(userId, cancellationToken);

        if (string.Equals(folder, "drafts", StringComparison.OrdinalIgnoreCase))
        {
            return new MailMailboxDto
            {
                Messages = Array.Empty<MailMessagePreviewDto>(),
                Drafts = await LoadDraftsAsync(userId, cancellationToken),
                UnreadCount = messages.Count(x => x.Read == 0),
                DraftCount = draftCount,
            };
        }

        return new MailMailboxDto
        {
            Messages = messages,
            Drafts = Array.Empty<MailDraftPreviewDto>(),
            UnreadCount = messages.Count(x => x.Read == 0),
            DraftCount = draftCount,
        };
    }

    public async Task<MailMessageDetailDto> GetMessageAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var row = await _context
            .MailRecipients.AsNoTracking()
            .Where(x => x.Id == recipientId && x.ToUserId == userId)
            .Select(x => new
            {
                x.Id,
                x.MessageId,
                From = x.Message.FromUser.FullnameCalc,
                FromId = x.Message.FromUserId,
                x.Message.Subject,
                x.Message.Message,
                x.Message.SendDate,
                Type = x.Message.MessageType.Name,
                x.Message.MessageTypeId,
                ConversationId = x
                    .Message.MailConversations.Select(c => c.ConversationId)
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row == null)
        {
            return null;
        }

        var to = await _context
            .MailRecipients.AsNoTracking()
            .Where(x => x.MessageId == row.MessageId)
            .Select(x => new MailRecipientDto
            {
                Id = x.ToGroupId ?? x.ToUserId ?? 0,
                Fullname = x.ToGroupId == null ? x.ToUser.FullnameCalc : x.ToGroup.GroupName,
                Type = x.ToGroupId == null ? "" : "group",
            })
            .ToListAsync(cancellationToken);

        return new MailMessageDetailDto
        {
            MessageId = row.MessageId ?? 0,
            RecipientId = row.Id,
            From = row.From,
            FromId = row.FromId ?? 0,
            Subject = row.Subject,
            Message = row.Message,
            SentRead = FormatSendDateReader(row.SendDate),
            To = to,
            Type = row.Type,
            TypeId = row.MessageTypeId,
            ConversationId = row.ConversationId,
        };
    }

    public async Task<MailDraftDetailDto> GetDraftAsync(
        ClaimsPrincipal user,
        int draftId,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var row = await _context
            .MailDrafts.AsNoTracking()
            .Where(x => x.DraftId == draftId && x.FromUserId == userId)
            .Select(x => new
            {
                x.DraftId,
                From = x.FromUser.FullnameCalc,
                FromId = x.FromUserId,
                x.Recipients,
                x.Subject,
                x.Message,
                x.EditDate,
                x.ReplyToMessageId,
                x.ReplyToConvId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row == null)
        {
            return null;
        }

        return new MailDraftDetailDto
        {
            DraftId = row.DraftId,
            From = row.From,
            FromId = row.FromId ?? 0,
            To = row.Recipients,
            Subject = row.Subject,
            Message = row.Message,
            Edited = FormatSendDatePreview(row.EditDate),
            ReplyToMessageId = row.ReplyToMessageId,
            ReplyToConversationId = row.ReplyToConvId,
        };
    }

    public async Task<MailCheckResponseDto> CheckForMailAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var messages = await LoadInboxAsync(userId, cancellationToken);
        var unseen = await _context
            .MailRecipients.Where(x =>
                x.ToUserId == userId && (x.AlertDisplayed == 0 || x.AlertDisplayed == null)
            )
            .ToListAsync(cancellationToken);
        var unseenIds = unseen.Select(x => x.Id).ToHashSet();
        foreach (var recipient in unseen)
        {
            recipient.AlertDisplayed = 1;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new MailCheckResponseDto
        {
            UnreadCount = messages.Count(x => x.Read == 0),
            MessageCount = messages.Count,
            DraftCount = await _context.MailDrafts.CountAsync(
                x => x.FromUserId == userId,
                cancellationToken
            ),
            Alerts = messages
                .Where(x => unseenIds.Contains(x.RecipientId))
                .Select(x => new MailAlertDto
                {
                    From = x.From,
                    Subject = x.Subject,
                    Message = x.Message,
                })
                .ToList(),
            Messages = messages,
        };
    }

    public async Task<bool> MarkMessageReadAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var recipient = await _context.MailRecipients.FirstOrDefaultAsync(
            x => x.Id == recipientId && x.ToUserId == userId,
            cancellationToken
        );
        if (recipient == null)
        {
            return false;
        }

        recipient.ReadDate = DateTime.Now;
        recipient.AlertDisplayed = 1;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<MailDraftSaveResponseDto> SaveDraftAsync(
        ClaimsPrincipal user,
        MailDraftSaveRequestDto request,
        CancellationToken cancellationToken
    )
    {
        request ??= new MailDraftSaveRequestDto();
        var userId = user.GetUserId();
        if (request.DraftId.GetValueOrDefault() >= 0)
        {
            var draft = await _context.MailDrafts.FirstOrDefaultAsync(
                x => x.DraftId == request.DraftId && x.FromUserId == userId,
                cancellationToken
            );
            if (draft == null)
            {
                return null;
            }

            draft.EditDate = DateTime.Now;
            draft.Message = request.Message;
            draft.MessagePlainText = request.Text;
            draft.Recipients = request.To;
            draft.Subject = request.Subject;
            draft.ReplyToConvId = request.ConvId;
            draft.ReplyToMessageId = request.MsgId;
            await _context.SaveChangesAsync(cancellationToken);
            return new MailDraftSaveResponseDto { DraftId = draft.DraftId };
        }

        var created = new MailDraft
        {
            EditDate = DateTime.Now,
            Message = request.Message,
            MessagePlainText = request.Text,
            Recipients = request.To,
            Subject = request.Subject,
            ReplyToConvId = request.ConvId,
            ReplyToMessageId = request.MsgId,
            FromUserId = userId,
        };
        await _context.MailDrafts.AddAsync(created, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new MailDraftSaveResponseDto { DraftId = created.DraftId };
    }

    public async Task<bool> DeleteMessageAsync(
        ClaimsPrincipal user,
        int recipientId,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var recipient = await _context.MailRecipients.FirstOrDefaultAsync(
            x => x.Id == recipientId && x.ToUserId == userId,
            cancellationToken
        );
        if (recipient == null)
        {
            return false;
        }

        _context.MailRecipientsDeleteds.Add(
            new MailRecipientsDeleted
            {
                AlertDisplayed = recipient.AlertDisplayed,
                MessageId = recipient.MessageId,
                ReadDate = recipient.ReadDate,
                ToUserId = recipient.ToUserId,
                ToGroupId = recipient.ToGroupId,
            }
        );
        _context.MailRecipients.Remove(recipient);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteDraftAsync(
        ClaimsPrincipal user,
        int draftId,
        CancellationToken cancellationToken
    )
    {
        var userId = user.GetUserId();
        var draft = await _context.MailDrafts.FirstOrDefaultAsync(
            x => x.DraftId == draftId && x.FromUserId == userId,
            cancellationToken
        );
        if (draft == null)
        {
            return false;
        }

        _context.MailDrafts.Remove(draft);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<List<MailMessagePreviewDto>> LoadInboxAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var rows = await _context
            .MailRecipients.AsNoTracking()
            .Where(x => x.ToUserId == userId)
            .OrderByDescending(x => x.Message.SendDate)
            .Select(x => new
            {
                x.Id,
                x.MessageId,
                From = x.Message.FromUser.FullnameCalc,
                FromId = x.Message.FromUserId,
                x.Message.Subject,
                x.Message.MessagePlainText,
                x.Message.SendDate,
                x.ReadDate,
                Type = x.Message.MessageType.Name,
                x.Message.MessageTypeId,
                ConversationId = x
                    .Message.MailConversations.Select(c => c.ConversationId)
                    .FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);

        return rows.Select(x => new MailMessagePreviewDto
            {
                MessageId = x.MessageId ?? 0,
                RecipientId = x.Id,
                From = x.From,
                FromId = x.FromId ?? 0,
                Subject = TakeWords(x.Subject, 5),
                Message = TakeWords(x.MessagePlainText, 10),
                SentPreview = FormatSendDatePreview(x.SendDate),
                Sent = x.SendDate,
                Read = x.ReadDate != null ? 1 : 0,
                Type = x.Type,
                TypeId = x.MessageTypeId,
                ConversationId = x.ConversationId,
            })
            .ToList();
    }

    private async Task<List<MailDraftPreviewDto>> LoadDraftsAsync(
        int userId,
        CancellationToken cancellationToken
    )
    {
        var rows = await _context
            .MailDrafts.AsNoTracking()
            .Where(x => x.FromUserId == userId)
            .OrderByDescending(x => x.EditDate)
            .Select(x => new
            {
                x.DraftId,
                From = x.FromUser.FullnameCalc,
                x.Recipients,
                x.Subject,
                x.MessagePlainText,
                x.EditDate,
                x.ReplyToMessageId,
                x.ReplyToConvId,
            })
            .ToListAsync(cancellationToken);

        return rows.Select(x => new MailDraftPreviewDto
            {
                DraftId = x.DraftId,
                From = x.From,
                To = x.Recipients,
                Subject = TakeWords(x.Subject, 5),
                Message = TakeWords(x.MessagePlainText, 10),
                Edited = FormatSendDatePreview(x.EditDate),
                ReplyToMessageId = x.ReplyToMessageId,
                ReplyToConversationId = x.ReplyToConvId,
            })
            .ToList();
    }

    private static string TakeWords(string value, int count)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        return string.Join(" ", value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(count));
    }

    private static string FormatSendDatePreview(DateTime? value)
    {
        if (value == null)
        {
            return "";
        }

        var sent = value.Value;
        var timeAgo = DateTime.Now.Subtract(sent);
        if (timeAgo.TotalDays < 1)
        {
            return sent.ToString("h:mm tt");
        }

        if (timeAgo.TotalHours < 2)
        {
            return "Yesterday";
        }

        return sent.ToString("M/d/yy");
    }

    private static string FormatSendDateReader(DateTime? value)
    {
        if (value == null)
        {
            return "";
        }

        var sent = value.Value;
        var timeAgo = DateTime.Now.Subtract(sent);
        if (timeAgo.TotalDays < 1)
        {
            return sent.ToString("h:mm tt");
        }

        if (timeAgo.TotalHours < 2)
        {
            return "Yesterday at " + sent.ToString("h:mm tt");
        }

        return sent.ToString("M/d/yy") + " at " + sent.ToString("h:mm tt");
    }
}
