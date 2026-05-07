namespace Atlas_Web.Contracts.Api.Interactions;

public sealed class ToggleStarRequestDto
{
    public string Type { get; init; }
    public int Id { get; init; }
}

public sealed class ToggleStarResponseDto
{
    public string Type { get; init; }
    public int Id { get; init; }
    public bool IsStarred { get; init; }
    public int Count { get; init; }
}

public sealed class ShareMailRequestDto
{
    public int? DraftId { get; init; }
    public IReadOnlyList<ShareRecipientDto> To { get; init; } = Array.Empty<ShareRecipientDto>();
    public string Subject { get; init; }
    public string Message { get; init; }
    public string Text { get; init; }
    public bool Share { get; init; }
    public string ShareName { get; init; }
    public string ShareUrl { get; init; }
}

public sealed class ShareRecipientDto
{
    public int UserId { get; init; }
    public string Type { get; init; }
}

public sealed class ShareMailResponseDto
{
    public string Message { get; init; }
    public int RecipientCount { get; init; }
    public int ShareCount { get; init; }
}

public sealed class ShareFeedbackRequestDto
{
    public string ReportName { get; init; }
    public string ReportUrl { get; init; }
    public string Description { get; init; }
}

public sealed class RecipientSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string Email { get; init; }
}
