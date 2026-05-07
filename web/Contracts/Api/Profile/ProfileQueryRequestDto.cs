using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Contracts.Api.Profile;

public sealed class ProfileQueryRequestDto
{
    [FromQuery(Name = "id")]
    public int Id { get; init; }

    [FromQuery(Name = "type")]
    public string Type { get; init; }

    [FromQuery(Name = "start_at")]
    public double StartAt { get; init; } = -31536000;

    [FromQuery(Name = "end_at")]
    public double EndAt { get; init; }

    [FromQuery(Name = "server")]
    public List<string> Server { get; init; }

    [FromQuery(Name = "database")]
    public List<string> Database { get; init; }

    [FromQuery(Name = "masterFile")]
    public List<string> MasterFile { get; init; }

    [FromQuery(Name = "visible")]
    public List<string> Visible { get; init; }

    [FromQuery(Name = "certification")]
    public List<string> Certification { get; init; }

    [FromQuery(Name = "availability")]
    public List<string> Availability { get; init; }

    [FromQuery(Name = "reportType")]
    public List<int> ReportType { get; init; }
}
