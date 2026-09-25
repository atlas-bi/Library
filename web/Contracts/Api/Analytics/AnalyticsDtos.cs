using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Contracts.Api.Analytics;

public class AnalyticsQueryRequest
{
    [FromQuery(Name = "start_at")]
    public double StartAt { get; init; } = -86400;

    [FromQuery(Name = "end_at")]
    public double EndAt { get; init; }

    [FromQuery(Name = "userId")]
    public int? UserId { get; init; } = -1;

    [FromQuery(Name = "groupId")]
    public int? GroupId { get; init; } = -1;
}

public sealed class AnalyticsLogQueryRequest : AnalyticsQueryRequest
{
    [FromQuery(Name = "p")]
    public int Page { get; init; }
}

public sealed class AnalyticsBeaconRequest
{
    public string Language { get; init; }
    public string UserAgent { get; init; }
    public string Hostname { get; init; }
    public string Href { get; init; }
    public string Protocol { get; init; }
    public string Search { get; init; }
    public string Pathname { get; init; }
    public string ScreenHeight { get; init; }
    public string ScreenWidth { get; init; }
    public string Origin { get; init; }
    public string LoadTime { get; init; }
    public string Referrer { get; init; }
    public double? Zoom { get; init; }
    public string SessionId { get; init; }
    public string PageId { get; init; }
    public int? PageTime { get; init; }
}

public sealed class AnalyticsBarItemDto
{
    public string Key { get; init; }
    public string Href { get; init; }
    public string TitleOne { get; init; }
    public string TitleTwo { get; init; }
    public double Count { get; init; }
    public double? Percent { get; init; }
}

public sealed class AnalyticsAccessHistoryDto
{
    public string Date { get; init; }
    public int Pages { get; init; }
    public int Sessions { get; init; }
    public double LoadTime { get; init; }
}

public sealed class AnalyticsVisitsResponseDto
{
    public int Views { get; init; }
    public int Visitors { get; init; }
    public double LoadTime { get; init; }
    public IReadOnlyList<AnalyticsAccessHistoryDto> AccessHistory { get; init; } =
        Array.Empty<AnalyticsAccessHistoryDto>();
}

public sealed class AnalyticsLiveUserDto
{
    public string Fullname { get; init; }
    public int UserId { get; init; }
    public string SessionTime { get; init; }
    public string PageTime { get; init; }
    public string Href { get; init; }
    public string AccessDateTime { get; init; }
    public string UpdateTime { get; init; }
    public int Pages { get; init; }
    public string SessionId { get; init; }
}

public sealed class AnalyticsLiveUsersResponseDto
{
    public int ActiveUsers { get; init; }
    public IReadOnlyList<AnalyticsLiveUserDto> Items { get; init; } =
        Array.Empty<AnalyticsLiveUserDto>();
}

public sealed class AnalyticsTraceEntryRequest
{
    [JsonPropertyName("l")]
    public int? Level { get; init; }

    [JsonPropertyName("m")]
    public string Message { get; init; }

    [JsonPropertyName("n")]
    public string Logger { get; init; }
}

public sealed class AnalyticsTraceIngestRequest
{
    [JsonPropertyName("lg")]
    public IReadOnlyList<AnalyticsTraceEntryRequest> Logs { get; init; } =
        Array.Empty<AnalyticsTraceEntryRequest>();
}

public sealed class AnalyticsTraceDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; }
    public int? Level { get; init; }
    public string Message { get; init; }
    public string Logger { get; init; }
    public DateTime? LogDateTime { get; init; }
    public int? Handled { get; init; }
    public string UserAgent { get; init; }
    public string Referer { get; init; }
}

public sealed class AnalyticsErrorDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; }
    public int? StatusCode { get; init; }
    public string Message { get; init; }
    public string Trace { get; init; }
    public DateTime? LogDateTime { get; init; }
    public int? Handled { get; init; }
    public string UserAgent { get; init; }
    public string Referrer { get; init; }
}

public sealed class AnalyticsTraceListResponseDto
{
    public int Pages { get; init; }
    public int CurrentPage { get; init; }
    public int TotalCount { get; init; }
    public int UnresolvedCount { get; init; }
    public IReadOnlyList<AnalyticsTraceDto> Items { get; init; } =
        Array.Empty<AnalyticsTraceDto>();
}

public sealed class AnalyticsErrorListResponseDto
{
    public int Pages { get; init; }
    public int CurrentPage { get; init; }
    public int TotalCount { get; init; }
    public int UnresolvedCount { get; init; }
    public IReadOnlyList<AnalyticsErrorDto> Items { get; init; } =
        Array.Empty<AnalyticsErrorDto>();
}
