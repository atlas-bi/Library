namespace Atlas_Web.Contracts.Api.Mail;

public sealed class MailMailboxDto
{
    public IReadOnlyList<MailMessagePreviewDto> Messages { get; init; } =
        Array.Empty<MailMessagePreviewDto>();
    public IReadOnlyList<MailDraftPreviewDto> Drafts { get; init; } =
        Array.Empty<MailDraftPreviewDto>();
    public int UnreadCount { get; init; }
    public int DraftCount { get; init; }
}

public sealed class MailMessagePreviewDto
{
    public int MessageId { get; init; }
    public int RecipientId { get; init; }
    public string From { get; init; }
    public int FromId { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string SentPreview { get; init; }
    public DateTime? Sent { get; init; }
    public int Read { get; init; }
    public string Type { get; init; }
    public int? TypeId { get; init; }
    public int ConversationId { get; init; }
}

public sealed class MailMessageDetailDto
{
    public int MessageId { get; init; }
    public int RecipientId { get; init; }
    public string From { get; init; }
    public int FromId { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string SentRead { get; init; }
    public IReadOnlyList<MailRecipientDto> To { get; init; } = Array.Empty<MailRecipientDto>();
    public string Type { get; init; }
    public int? TypeId { get; init; }
    public int ConversationId { get; init; }
}

public sealed class MailRecipientDto
{
    public int Id { get; init; }
    public string Fullname { get; init; }
    public string Type { get; init; }
}

public sealed class MailDraftPreviewDto
{
    public int DraftId { get; init; }
    public string From { get; init; }
    public string To { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string Edited { get; init; }
    public int? ReplyToMessageId { get; init; }
    public int? ReplyToConversationId { get; init; }
}

public sealed class MailDraftDetailDto
{
    public int DraftId { get; init; }
    public string From { get; init; }
    public int FromId { get; init; }
    public string To { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string Edited { get; init; }
    public int? ReplyToMessageId { get; init; }
    public int? ReplyToConversationId { get; init; }
}

public sealed class MailCheckResponseDto
{
    public int UnreadCount { get; init; }
    public int MessageCount { get; init; }
    public int DraftCount { get; init; }
    public IReadOnlyList<MailAlertDto> Alerts { get; init; } = Array.Empty<MailAlertDto>();
    public IReadOnlyList<MailMessagePreviewDto> Messages { get; init; } =
        Array.Empty<MailMessagePreviewDto>();
}

public sealed class MailAlertDto
{
    public string From { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
}

public sealed class MailDraftSaveRequestDto
{
    public int? DraftId { get; init; }
    public string To { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string Text { get; init; }
    public int? MsgId { get; init; }
    public int? ConvId { get; init; }
}

public sealed class MailDraftSaveResponseDto
{
    public int DraftId { get; init; }
}
