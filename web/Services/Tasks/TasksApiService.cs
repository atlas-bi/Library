using Atlas_Web.Contracts.Api.Tasks;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Atlas_Web.Services;

public interface ITasksApiService
{
    Task<TasksResponseDto> GetTasksAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
}

public sealed class TasksApiService : ITasksApiService
{
    private readonly Atlas_WebContext _context;

    public TasksApiService(Atlas_WebContext context)
    {
        _context = context;
    }

    public async Task<TasksResponseDto> GetTasksAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken
    )
    {
        return new TasksResponseDto
        {
            CanMakeReports = await GetCanMakeReportsAsync(cancellationToken),
            RecommendRetire = await GetRecommendRetireAsync(cancellationToken),
            Unused = await GetUnusedAsync(cancellationToken),
            MaintenanceRequired = await GetMaintenanceAsync(includeAuditSchedule: false, cancellationToken),
            Audit = await GetMaintenanceAsync(includeAuditSchedule: true, cancellationToken),
            MissingSchedule = await GetMissingScheduleAsync(cancellationToken),
            NotInAnalytics = await GetNotInAnalyticsAsync(cancellationToken),
            TopUndocumented = await GetUndocumentedAsync(recentOnly: false, cancellationToken),
            NewUndocumented = await GetUndocumentedAsync(recentOnly: true, cancellationToken),
        };
    }

    private Task<List<TaskCanMakeReportDto>> GetCanMakeReportsAsync(CancellationToken cancellationToken)
    {
        var groupEpicIds = new[] { "100623", "100624", "100612", "5087102001", "5087101001", "5087107002" };
        return _context.UserGroupsMemberships
            .Where(x => groupEpicIds.Contains(x.Group.EpicId))
            .Select(x => new TaskCanMakeReportDto
            {
                Name = x.User.FullnameCalc,
                UserId = x.UserId,
                Role = x.Group.GroupName,
                RoleId = x.GroupId,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<TaskRetireReportDto>> GetRecommendRetireAsync(CancellationToken cancellationToken)
    {
        return _context.MaintenanceLogs
            .Where(x => x.MaintenanceLogStatus.Name == "Recommend Retire"
                && x.ReportObjectDoc.ReportObject.DefaultVisibilityYn == "Y")
            .Select(x => new TaskRetireReportDto
            {
                FullName = x.Maintainer.FullnameCalc,
                Name = x.ReportObjectDoc.ReportObject.DisplayName,
                MaintenanceDate = x.MaintenanceDate,
                MaintenanceDateString = x.MaintenanceDateDisplayString,
                ReportId = x.ReportId,
                Comment = x.Comment,
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<TaskUnusedReportDto>> GetUnusedAsync(CancellationToken cancellationToken)
    {
        var reportTypeIds = new[] { 3, 17, 20, 28 };
        return (
            from report in _context.ReportObjects
            where reportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.DefaultVisibilityYn == "Y"
                && report.OrphanedReportObjectYn == "N"
                && !report.ReportObjectRunDataBridges.Any()
                && (!_context.ReportObjectDocs.Any(document => document.ReportObjectId == report.ReportObjectId)
                    || _context.ReportObjectDocs.Any(document => document.ReportObjectId == report.ReportObjectId
                        && (document.Hidden ?? "N") == "N"))
                && (report.LastModifiedDate < DateTime.Now.AddMonths(-2) || report.LastModifiedDate == null)
            orderby report.LastModifiedDate ascending
            select new TaskUnusedReportDto
            {
                ReportUrl = "/reports?id=" + report.ReportObjectId,
                Name = report.DisplayName,
                Type = report.ReportObjectType.Name,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc,
                LastModified = report.LastUpdatedDateDisplayString,
                Server = report.SourceServer,
                MasterFile = report.EpicMasterFile,
                EpicId = report.EpicRecordId.ToString(),
            }
        ).Take(30).ToListAsync(cancellationToken);
    }

    private async Task<List<TaskMaintenanceReportDto>> GetMaintenanceAsync(
        bool includeAuditSchedule,
        CancellationToken cancellationToken
    )
    {
        var today = DateTime.Now;
        return await (
            from document in _context.ReportObjectDocs
            where (includeAuditSchedule
                    ? document.MaintenanceScheduleId == 5
                    : document.MaintenanceScheduleId != 5 && document.MaintenanceScheduleId != null)
                && document.ReportObject.DefaultVisibilityYn == "Y"
                && document.ReportObject.OrphanedReportObjectYn == "N"
            join latest in (
                from log in _context.MaintenanceLogs
                group log by log.ReportId into grouped
                select new { ReportId = grouped.Key, MaintenanceLogId = grouped.Max(x => x.MaintenanceLogId) }
            ) on document.ReportObjectId equals latest.ReportId into latestLogs
            from latest in latestLogs.DefaultIfEmpty()
            join maintenance in _context.MaintenanceLogs
                on latest.MaintenanceLogId equals maintenance.MaintenanceLogId into maintenanceLogs
            from maintenance in maintenanceLogs.DefaultIfEmpty()
            let nextDate = document.MaintenanceScheduleId == 1
                ? ((maintenance == null ? null : maintenance.MaintenanceDate) ?? document.LastUpdateDateTime ?? today).AddMonths(3)
                : document.MaintenanceScheduleId == 2
                    ? ((maintenance == null ? null : maintenance.MaintenanceDate) ?? document.LastUpdateDateTime ?? today).AddMonths(6)
                    : document.MaintenanceScheduleId == 3
                        ? ((maintenance == null ? null : maintenance.MaintenanceDate) ?? document.LastUpdateDateTime ?? today).AddYears(1)
                        : document.MaintenanceScheduleId == 4
                            ? ((maintenance == null ? null : maintenance.MaintenanceDate) ?? document.LastUpdateDateTime ?? today).AddYears(2)
                            : ((maintenance == null ? null : maintenance.MaintenanceDate) ?? document.LastUpdateDateTime ?? document.CreatedDateTime ?? today)
            where nextDate < today.AddMonths(2)
            orderby nextDate
            select new TaskMaintenanceReportDto
            {
                ReportId = document.ReportObjectId,
                Date = nextDate.ToString("MM/dd/yyyy"),
                Name = document.ReportObject.DisplayName,
                User = maintenance == null || maintenance.Maintainer == null
                    || maintenance.Maintainer.FullnameCalc == "user not found"
                    ? document.UpdatedByNavigation.FullnameCalc
                    : maintenance.Maintainer.FullnameCalc,
            }
        ).ToListAsync(cancellationToken);
    }

    private Task<List<TaskMaintenanceReportDto>> GetMissingScheduleAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.Now;
        return (
            from document in _context.ReportObjectDocs
            where document.MaintenanceScheduleId == null
                && document.ReportObject.DefaultVisibilityYn == "Y"
                && document.ReportObject.OrphanedReportObjectYn == "N"
            join latest in (
                from log in _context.MaintenanceLogs
                group log by log.ReportId into grouped
                select new { ReportId = grouped.Key, MaintenanceLogId = grouped.Max(x => x.MaintenanceLogId) }
            ) on document.ReportObjectId equals latest.ReportId into latestLogs
            from latest in latestLogs.DefaultIfEmpty()
            join maintenance in _context.MaintenanceLogs
                on latest.MaintenanceLogId equals maintenance.MaintenanceLogId into maintenanceLogs
            from maintenance in maintenanceLogs.DefaultIfEmpty()
            let nextDate = (maintenance == null ? null : maintenance.MaintenanceDate)
                ?? document.LastUpdateDateTime ?? today
            where nextDate < today.AddMonths(2)
            orderby nextDate
            select new TaskMaintenanceReportDto
            {
                ReportId = document.ReportObjectId,
                Date = nextDate.ToString("MM/dd/yyyy"),
                Name = document.ReportObject.DisplayName,
                User = maintenance == null || maintenance.Maintainer == null
                    || maintenance.Maintainer.FullnameCalc == "user not found"
                    ? document.UpdatedByNavigation.FullnameCalc
                    : maintenance.Maintainer.FullnameCalc,
            }
        ).ToListAsync(cancellationToken);
    }

    private Task<List<TaskAnalyticsReportDto>> GetNotInAnalyticsAsync(CancellationToken cancellationToken)
    {
        var reportTypeIds = new[] { 3, 17 };
        return (
            from report in _context.ReportObjects
            where report.LastModifiedDate > DateTime.Today.AddMonths(-6)
                && report.DefaultVisibilityYn == "Y"
                && report.OrphanedReportObjectYn == "N"
                && reportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.LastModifiedByUser.UserRoleLinks.Any(x => x.UserRolesId != 1)
                && report.AuthorUser.UserRoleLinks.Any(x => x.UserRolesId != 1)
                && report.ReportObjectRunDataBridges.Any()
            select new TaskAnalyticsReportDto
            {
                ReportUrl = "/reports?id=" + report.ReportObjectId,
                LastModified = report.LastUpdatedDateDisplayString,
                Author = report.AuthorUser.FullnameCalc,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc,
                Name = report.DisplayName,
                ReportType = report.ReportObjectType.Name,
                Epic = report.EpicMasterFile + " " + report.EpicRecordId,
                EditReportUrl = null,
                RecordViewerUrl = null,
                Runs = report.ReportObjectRunDataBridges.Sum(x => x.Runs),
                EpicRecordId = report.EpicRecordId.ToString(),
                EpicMasterFile = report.EpicMasterFile,
            }
        ).ToListAsync(cancellationToken);
    }

    private Task<List<TaskUndocumentedReportDto>> GetUndocumentedAsync(
        bool recentOnly,
        CancellationToken cancellationToken
    )
    {
        var reportTypeIds = new[] { 17, 28, 3, 20 };
        return (
            from report in _context.ReportObjects
            where reportTypeIds.Contains(report.ReportObjectTypeId ?? 0)
                && report.DefaultVisibilityYn == "Y"
                && (!recentOnly || report.LastModifiedDate > DateTime.Today.AddMonths(-1))
                && !_context.ReportObjectDocs.Any(x => x.ReportObjectId == report.ReportObjectId
                    && x.DeveloperDescription != null)
            select new TaskUndocumentedReportDto
            {
                ReportObjectId = report.ReportObjectId,
                ModifiedBy = report.LastModifiedByUser.FullnameCalc != "user not found"
                    ? report.LastModifiedByUser.FullnameCalc
                    : report.AuthorUser.FullnameCalc,
                Name = report.DisplayName,
                ReportType = report.ReportObjectType.Name == "Reporting Workbench Report" ? "Workbench"
                    : report.ReportObjectType.Name == "Source Radar Dashboard" ? "Dashboard"
                    : report.ReportObjectType.Name == "Epic-Crystal Report" ? "Crystal" : "SSRS",
                Runs = report.ReportObjectRunDataBridges.Sum(x => x.Runs),
                Favorite = report.StarredReports.Any() ? "Yes" : "",
                LastMaintained = (report.LastModifiedDate ?? DateTime.Today.AddYears(-1)).ToString("MM/dd/yyyy"),
                LastRun = report.ReportObjectRunDataBridges
                    .Max(x => x.RunData.RunStartTime_Day).ToString("MM/dd/yyyy"),
            }
        ).Take(60).ToListAsync(cancellationToken);
    }
}
