using Atlas_Web.Contracts.Api.Tasks;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public interface ITasksApiService
{
    Task<TasksResponseDto> GetTasksAsync(CancellationToken cancellationToken);
}

public sealed class TasksApiService : ITasksApiService
{
    private const string TaskDateFormat = "MM/dd/yyyy";
    private const string MissingUserName = "user not found";

    private static readonly string[] CanMakeReportGroupEpicIds =
    {
        "100623",
        "100624",
        "100612",
        "5087102001",
        "5087101001",
        "5087107002",
    };
    private static readonly int[] UnusedReportTypeIds = { 3, 17, 20, 28 };
    private static readonly int[] UndocumentedReportTypeIds = { 17, 28, 3, 20 };
    private static readonly int[] AnalyticsReportTypeIds = { 3, 17 };

    private enum MaintenanceBucket
    {
        Required,
        Audit,
        Missing,
    }

    private sealed class MaintenanceDocRow
    {
        public int ReportObjectId { get; init; }
        public int? MaintenanceScheduleId { get; init; }
        public DateTime? MaintenanceDate { get; init; }
        public DateTime? LastUpdateDateTime { get; init; }
        public DateTime? CreatedDateTime { get; init; }
        public string Name { get; init; }
        public string MaintainerName { get; init; }
        public string UpdatedByName { get; init; }
    }

    private readonly Atlas_WebContext _context;

    public TasksApiService(Atlas_WebContext context)
    {
        _context = context;
    }

    public async Task<TasksResponseDto> GetTasksAsync(CancellationToken cancellationToken)
    {
        return new TasksResponseDto
        {
            CanMakeReports = await GetCanMakeReportsAsync(cancellationToken),
            RecommendRetire = await GetRecommendRetireAsync(cancellationToken),
            Unused = await GetUnusedAsync(cancellationToken),
            MaintenanceRequired = await GetMaintenanceAsync(
                MaintenanceBucket.Required,
                cancellationToken
            ),
            Audit = await GetMaintenanceAsync(MaintenanceBucket.Audit, cancellationToken),
            MissingSchedule = await GetMissingScheduleAsync(cancellationToken),
            NotInAnalytics = await GetNotInAnalyticsAsync(cancellationToken),
            TopUndocumented = await GetUndocumentedAsync(false, cancellationToken),
            NewUndocumented = await GetUndocumentedAsync(true, cancellationToken),
        };
    }

    private async Task<List<TaskCanMakeReportDto>> GetCanMakeReportsAsync(
        CancellationToken cancellationToken
    )
    {
        return await _context
            .UserGroupsMemberships.AsNoTracking()
            .Where(x => CanMakeReportGroupEpicIds.Contains(x.Group.EpicId))
            .Select(x => new TaskCanMakeReportDto
            {
                Name = x.User.FullnameCalc,
                UserId = x.UserId,
                Role = x.Group.GroupName,
                RoleId = x.GroupId,
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<TaskRetireReportDto>> GetRecommendRetireAsync(
        CancellationToken cancellationToken
    )
    {
        var rows = await _context
            .MaintenanceLogs.AsNoTracking()
            .Where(x =>
                x.MaintenanceLogStatus.Name == "Recommend Retire"
                && x.ReportObjectDoc.ReportObject.DefaultVisibilityYn == "Y"
            )
            .Select(x => new
            {
                FullName = x.Maintainer.FullnameCalc,
                Name = x.ReportObjectDoc.ReportObject.DisplayTitle
                    ?? x.ReportObjectDoc.ReportObject.Name,
                x.MaintenanceDate,
                x.ReportId,
                x.Comment,
            })
            .ToListAsync(cancellationToken);

        return rows.Select(x => new TaskRetireReportDto
            {
                FullName = x.FullName,
                Name = x.Name,
                MaintenanceDate = x.MaintenanceDate,
                MaintenanceDateString = ModelHelpers.RelativeDate(x.MaintenanceDate),
                ReportId = x.ReportId,
                Comment = x.Comment,
            })
            .ToList();
    }

    private async Task<List<TaskUnusedReportDto>> GetUnusedAsync(
        CancellationToken cancellationToken
    )
    {
        var cutoff = DateTime.Now.AddMonths(-2);
        var rows = await (
            from report in _context.ReportObjects.AsNoTracking()
            where
                UnusedReportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.DefaultVisibilityYn == "Y"
                && report.OrphanedReportObjectYn == "N"
                && !report.ReportObjectRunDataBridges.Any()
                && !_context.ReportObjectDocs.Any(document =>
                    document.ReportObjectId == report.ReportObjectId && document.Hidden == "Y"
                )
                && (report.LastModifiedDate < cutoff || report.LastModifiedDate == null)
            orderby report.LastModifiedDate
            select new
            {
                report.ReportObjectId,
                Name = report.DisplayTitle ?? report.Name,
                Type = report.ReportObjectType.Name,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc,
                report.LastModifiedDate,
                report.SourceServer,
                report.EpicMasterFile,
                report.EpicRecordId,
            }
        )
            .Take(30)
            .ToListAsync(cancellationToken);

        return rows.Select(x => new TaskUnusedReportDto
            {
                ReportUrl = "/reports?id=" + x.ReportObjectId,
                Name = x.Name,
                Type = x.Type,
                ModifiedBy = x.ModifiedBy,
                LastModified = ModelHelpers.RelativeDate(x.LastModifiedDate),
                Server = x.SourceServer,
                MasterFile = x.EpicMasterFile,
                EpicId = x.EpicRecordId.ToString(),
            })
            .ToList();
    }

    private async Task<List<TaskMaintenanceReportDto>> GetMaintenanceAsync(
        MaintenanceBucket bucket,
        CancellationToken cancellationToken
    )
    {
        var today = DateTime.Now;
        var rows = await QueryVisibleDocsWithLatestMaintenanceAsync(bucket, cancellationToken);
        return ToUpcomingMaintenanceDtos(
            rows,
            today,
            x =>
                NextMaintenanceDate(
                    x.MaintenanceScheduleId,
                    x.MaintenanceDate,
                    x.LastUpdateDateTime,
                    x.CreatedDateTime,
                    today
                )
        );
    }

    private async Task<List<TaskMaintenanceReportDto>> GetMissingScheduleAsync(
        CancellationToken cancellationToken
    )
    {
        var today = DateTime.Now;
        var rows = await QueryVisibleDocsWithLatestMaintenanceAsync(
            MaintenanceBucket.Missing,
            cancellationToken
        );
        return ToUpcomingMaintenanceDtos(
            rows,
            today,
            x => x.MaintenanceDate ?? x.LastUpdateDateTime ?? today
        );
    }

    private async Task<List<MaintenanceDocRow>> QueryVisibleDocsWithLatestMaintenanceAsync(
        MaintenanceBucket bucket,
        CancellationToken cancellationToken
    )
    {
        return await (
            from document in _context.ReportObjectDocs.AsNoTracking()
            where
                (
                    bucket == MaintenanceBucket.Missing
                        ? document.MaintenanceScheduleId == null
                        : bucket == MaintenanceBucket.Audit
                            ? document.MaintenanceScheduleId == 5
                            : document.MaintenanceScheduleId != 5
                                && document.MaintenanceScheduleId != null
                )
                && document.ReportObject.DefaultVisibilityYn == "Y"
                && document.ReportObject.OrphanedReportObjectYn == "N"
            join latest in from log in _context.MaintenanceLogs
            group log by log.ReportId into grouped
            select new
            {
                ReportId = grouped.Key,
                MaintenanceLogId = grouped.Max(x => x.MaintenanceLogId),
            }
                on document.ReportObjectId equals latest.ReportId
                into latestLogs
            from latest in latestLogs.DefaultIfEmpty()
            join maintenance in _context.MaintenanceLogs
                on latest.MaintenanceLogId equals maintenance.MaintenanceLogId
                into maintenanceLogs
            from maintenance in maintenanceLogs.DefaultIfEmpty()
            select new MaintenanceDocRow
            {
                ReportObjectId = document.ReportObjectId,
                MaintenanceScheduleId = document.MaintenanceScheduleId,
                MaintenanceDate = maintenance == null ? null : maintenance.MaintenanceDate,
                LastUpdateDateTime = document.LastUpdateDateTime,
                CreatedDateTime = document.CreatedDateTime,
                Name = document.ReportObject.DisplayTitle ?? document.ReportObject.Name,
                MaintainerName = maintenance == null ? null : maintenance.Maintainer.FullnameCalc,
                UpdatedByName = document.UpdatedByNavigation.FullnameCalc,
            }
        ).ToListAsync(cancellationToken);
    }

    private static List<TaskMaintenanceReportDto> ToUpcomingMaintenanceDtos(
        IEnumerable<MaintenanceDocRow> rows,
        DateTime today,
        Func<MaintenanceDocRow, DateTime> nextDate
    )
    {
        return rows.Select(x => new
            {
                x.ReportObjectId,
                NextDate = nextDate(x),
                x.Name,
                User = ResolveMaintainer(x.MaintainerName, x.UpdatedByName),
            })
            .Where(x => x.NextDate < today.AddMonths(2))
            .OrderBy(x => x.NextDate)
            .Select(x => new TaskMaintenanceReportDto
            {
                ReportId = x.ReportObjectId,
                Date = FormatTaskDate(x.NextDate),
                Name = x.Name,
                User = x.User,
            })
            .ToList();
    }

    private async Task<List<TaskAnalyticsReportDto>> GetNotInAnalyticsAsync(
        CancellationToken cancellationToken
    )
    {
        var cutoff = DateTime.Today.AddMonths(-6);
        var rows = await (
            from report in _context.ReportObjects.AsNoTracking()
            join modifierRole in _context.UserRoleLinks
                on report.LastModifiedByUserId equals modifierRole.UserId
                into modifierRoles
            from modifierRole in modifierRoles.DefaultIfEmpty()
            where modifierRole.UserRolesId != 1
            join authorRole in _context.UserRoleLinks
                on report.AuthorUserId equals authorRole.UserId
                into authorRoles
            from authorRole in authorRoles.DefaultIfEmpty()
            where authorRole.UserRolesId != 1
            where
                report.LastModifiedDate > cutoff
                && report.DefaultVisibilityYn == "Y"
                && report.OrphanedReportObjectYn == "N"
                && AnalyticsReportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.ReportObjectRunDataBridges.Any()
            select new
            {
                report.ReportObjectId,
                report.LastModifiedDate,
                Author = report.AuthorUser.FullnameCalc,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc,
                Name = report.DisplayTitle ?? report.Name,
                ReportType = report.ReportObjectType.Name,
                report.EpicMasterFile,
                report.EpicRecordId,
                Runs = report.ReportObjectRunDataBridges.Sum(x => x.Runs),
            }
        ).ToListAsync(cancellationToken);

        return rows.Select(x => new TaskAnalyticsReportDto
            {
                ReportUrl = "/reports?id=" + x.ReportObjectId,
                LastModified = ModelHelpers.RelativeDate(x.LastModifiedDate),
                Author = x.Author,
                ModifiedBy = x.ModifiedBy,
                Name = x.Name,
                ReportType = x.ReportType,
                Epic = x.EpicMasterFile + " " + x.EpicRecordId,
                EditReportUrl = null,
                RecordViewerUrl = null,
                Runs = x.Runs,
                EpicRecordId = x.EpicRecordId.ToString(),
                EpicMasterFile = x.EpicMasterFile,
            })
            .ToList();
    }

    private async Task<List<TaskUndocumentedReportDto>> GetUndocumentedAsync(
        bool recentOnly,
        CancellationToken cancellationToken
    )
    {
        var recentCutoff = DateTime.Today.AddMonths(-1);
        var rows = await (
            from report in _context.ReportObjects.AsNoTracking()
            where
                UndocumentedReportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.DefaultVisibilityYn == "Y"
                && (!recentOnly || report.LastModifiedDate > recentCutoff)
                && !_context.ReportObjectDocs.Any(document =>
                    document.ReportObjectId == report.ReportObjectId
                    && document.DeveloperDescription != null
                )
            select new
            {
                report.ReportObjectId,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc != MissingUserName
                    ? report.LastModifiedByUser.FullnameCalc
                    : report.AuthorUser.FullnameCalc,
                Name = report.DisplayTitle ?? report.Name,
                ReportType = report.ReportObjectType.Name,
                Runs = report.ReportObjectRunDataBridges.Sum(x => x.Runs),
                Favorite = report.StarredReports.Any(),
                report.LastModifiedDate,
                LastRun = report
                    .ReportObjectRunDataBridges.Select(x => (DateTime?)x.RunData.RunStartTime_Day)
                    .Max(),
            }
        )
            .Take(60)
            .ToListAsync(cancellationToken);

        return rows.Select(x => new TaskUndocumentedReportDto
            {
                ReportObjectId = x.ReportObjectId,
                ModifiedBy = x.ModifiedBy,
                Name = x.Name,
                ReportType = FormatUndocumentedType(x.ReportType),
                Runs = x.Runs,
                Favorite = x.Favorite ? "Yes" : "",
                LastMaintained = FormatTaskDate(x.LastModifiedDate ?? DateTime.Today.AddYears(-1)),
                LastRun = x.LastRun == null ? "" : FormatTaskDate(x.LastRun.Value),
            })
            .ToList();
    }

    private static string FormatTaskDate(DateTime date) => date.ToString(TaskDateFormat);

    private static string ResolveMaintainer(string maintainerName, string updatedByName)
    {
        return string.IsNullOrEmpty(maintainerName) || maintainerName == MissingUserName
            ? updatedByName
            : maintainerName;
    }

    private static DateTime NextMaintenanceDate(
        int? scheduleId,
        DateTime? maintenanceDate,
        DateTime? lastUpdate,
        DateTime? created,
        DateTime today
    )
    {
        var basis = maintenanceDate ?? lastUpdate ?? today;
        return scheduleId switch
        {
            1 => basis.AddMonths(3),
            2 => basis.AddMonths(6),
            3 => basis.AddYears(1),
            4 => basis.AddYears(2),
            _ => maintenanceDate ?? lastUpdate ?? created ?? today,
        };
    }

    private static string FormatUndocumentedType(string name)
    {
        return name switch
        {
            "Reporting Workbench Report" => "Workbench",
            "Source Radar Dashboard" => "Dashboard",
            "Epic-Crystal Report" => "Crystal",
            _ => "SSRS",
        };
    }
}
