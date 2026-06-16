namespace Atlas_Web.Contracts.Api.Search;

public sealed class SearchResponseDto
{
    public IReadOnlyList<SearchResultDto> Results { get; init; } = [];
    public IReadOnlyList<FacetDto> Facets { get; init; } = [];
    public IReadOnlyList<HighlightDto> Highlights { get; init; } = [];
    public IReadOnlyList<FilterFieldDto> FilterFields { get; init; } = [];
    public long Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int QTime { get; init; }
    public bool IsAdvancedSearch { get; init; }
}

public sealed class SearchResultDto
{
    public string Id { get; init; }
    public int AtlasId { get; init; }
    public string Type { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Url { get; init; }
    public string ReportType { get; init; }
    public string Email { get; init; }
    public string EpicMasterFile { get; init; }
    public string EpicRecordId { get; init; }
    public string EpicTemplateId { get; init; }
    public string ReportServerPath { get; init; }
    public string ExecutiveVisibility { get; init; }
    public string SourceServer { get; init; }
    public string GroupType { get; init; }
    public bool IsStarred { get; set; }
    public IReadOnlyList<string> Certifications { get; init; } = [];
    public string Documented { get; init; }
}

public sealed class FacetDto
{
    public string Key { get; init; }
    public IReadOnlyList<FacetValueDto> Values { get; init; } = [];
}

public sealed class FacetValueDto
{
    public string Value { get; init; }
    public int Count { get; init; }
}

public sealed class HighlightDto
{
    public string Id { get; init; }
    public IReadOnlyList<HighlightFieldDto> Fields { get; init; } = [];
}

public sealed class HighlightFieldDto
{
    public string Field { get; init; }
    public string Snippet { get; init; }
}

public sealed class FilterFieldDto
{
    public string Key { get; init; }
    public string Label { get; init; }
}
