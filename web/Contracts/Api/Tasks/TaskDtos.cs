namespace Atlas_Web.Contracts.Api.Tasks;

public sealed class TasksResponseDto
{
    public IReadOnlyList<TaskCanMakeReportDto> CanMakeReports { get; init; } = Array.Empty<TaskCanMakeReportDto>();
    public IReadOnlyList<TaskRetireReportDto> RecommendRetire { get; init; } = Array.Empty<TaskRetireReportDto>();
    public IReadOnlyList<TaskUnusedReportDto> Unused { get; init; } = Array.Empty<TaskUnusedReportDto>();
    public IReadOnlyList<TaskMaintenanceReportDto> MaintenanceRequired { get; init; } = Array.Empty<TaskMaintenanceReportDto>();
    public IReadOnlyList<TaskMaintenanceReportDto> Audit { get; init; } = Array.Empty<TaskMaintenanceReportDto>();
    public IReadOnlyList<TaskMaintenanceReportDto> MissingSchedule { get; init; } = Array.Empty<TaskMaintenanceReportDto>();
    public IReadOnlyList<TaskAnalyticsReportDto> NotInAnalytics { get; init; } = Array.Empty<TaskAnalyticsReportDto>();
    public IReadOnlyList<TaskUndocumentedReportDto> TopUndocumented { get; init; } = Array.Empty<TaskUndocumentedReportDto>();
    public IReadOnlyList<TaskUndocumentedReportDto> NewUndocumented { get; init; } = Array.Empty<TaskUndocumentedReportDto>();
}

public sealed class TaskCanMakeReportDto
{
    public string Name { get; init; }
    public int? UserId { get; init; }
    public string Role { get; init; }
    public int? RoleId { get; init; }
}

public sealed class TaskRetireReportDto
{
    public string Name { get; init; }
    public DateTime? MaintenanceDate { get; init; }
    public string MaintenanceDateString { get; init; }
    public int ReportId { get; init; }
    public string Comment { get; init; }
    public string FullName { get; init; }
}

public sealed class TaskUnusedReportDto
{
    public string ReportUrl { get; init; }
    public string Name { get; init; }
    public string Type { get; init; }
    public string ModifiedBy { get; init; }
    public string LastModified { get; init; }
    public string Server { get; init; }
    public string MasterFile { get; init; }
    public string EpicId { get; init; }
}

public sealed class TaskMaintenanceReportDto
{
    public int ReportId { get; init; }
    public string Date { get; init; }
    public string Name { get; init; }
    public string User { get; init; }
}

public sealed class TaskAnalyticsReportDto
{
    public string ReportUrl { get; init; }
    public string LastModified { get; init; }
    public string Author { get; init; }
    public string ModifiedBy { get; init; }
    public string Name { get; init; }
    public string ReportType { get; init; }
    public string Epic { get; init; }
    public string RunReportUrl { get; init; }
    public string EditReportUrl { get; init; }
    public string RecordViewerUrl { get; init; }
    public int Runs { get; init; }
    public string EpicMasterFile { get; init; }
    public string EpicRecordId { get; init; }
}

public sealed class TaskUndocumentedReportDto
{
    public int ReportObjectId { get; init; }
    public string ModifiedBy { get; init; }
    public string Name { get; init; }
    public string ReportType { get; init; }
    public int Runs { get; init; }
    public string LastMaintained { get; init; }
    public string LastRun { get; init; }
    public string Favorite { get; init; }
}
