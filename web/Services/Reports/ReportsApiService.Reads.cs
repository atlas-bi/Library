using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Reports;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Atlas_Web.Services;

public sealed partial class ReportsApiService
{
    private async Task<bool> ReportExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.ReportObjects.AsNoTracking()
            .AnyAsync(x => x.ReportObjectId == id, cancellationToken);
    }

    private bool IsFeatureEnabled(string key)
    {
        var value = _configuration[key];
        return string.IsNullOrWhiteSpace(value)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private ReportFeatureFlagsDto BuildFeatureFlags()
    {
        return new ReportFeatureFlagsDto
        {
            TermsEnabled = IsFeatureEnabled("features:enable_terms"),
            UserProfilesEnabled = IsFeatureEnabled("features:enable_user_profile"),
            FeedbackEnabled = IsFeatureEnabled("features:enable_feedback"),
            RequestAccessEnabled = IsFeatureEnabled("features:enable_request_access"),
            SharingEnabled = IsFeatureEnabled("features:enable_sharing"),
        };
    }

    private async Task PopulateRunAuthorizationAsync(
        ClaimsPrincipal user,
        IReadOnlyList<ReportListItemDto> reports,
        CancellationToken cancellationToken
    )
    {
        if (reports.Count == 0)
        {
            return;
        }

        var reportIds = reports.Select(x => x.Id).ToArray();
        var authorizationReports = await LoadAuthorizationReportsAsync(reportIds, cancellationToken);
        var authorizationLookup = authorizationReports.ToDictionary(x => x.ReportObjectId);

        foreach (var report in reports)
        {
            report.CanRun =
                authorizationLookup.TryGetValue(report.Id, out var authorizationReport)
                && (
                    await _authorizationService.AuthorizeAsync(
                        user,
                        authorizationReport,
                        "ReportRunPolicy"
                    )
                ).Succeeded;
        }
    }

    private async Task<bool> CanRunReportAsync(
        ClaimsPrincipal user,
        int reportId,
        CancellationToken cancellationToken
    )
    {
        var authorizationReports = await LoadAuthorizationReportsAsync(
            new[] { reportId },
            cancellationToken
        );
        var authorizationReport = authorizationReports.SingleOrDefault();
        if (authorizationReport == null)
        {
            return false;
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(
            user,
            authorizationReport,
            "ReportRunPolicy"
        );
        return authorizationResult.Succeeded;
    }

    private async Task<List<ReportObject>> LoadAuthorizationReportsAsync(
        IReadOnlyCollection<int> reportIds,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .ReportObjects.AsNoTracking()
            .Where(x => reportIds.Contains(x.ReportObjectId))
            .Include(x => x.ReportObjectType)
            .Include(x => x.ReportGroupsMemberships)
            .Include(x => x.ReportObjectHierarchyChildReportObjects)
                .ThenInclude(x => x.ParentReportObject)
                    .ThenInclude(x => x.ReportGroupsMemberships)
            .ToListAsync(cancellationToken);
    }

    private async Task<ReportDetailDto> GetReportCoreAsync(
        ClaimsPrincipal user,
        int id,
        CancellationToken cancellationToken,
        bool visibleOnly
    )
    {
        var canEditDocumentation = user.HasPermission("Edit Report Documentation");
        var canViewGroups = user.HasPermission("View Groups");
        var canViewPurgeOption = user.HasPermission("Edit Report Purge Option");
        var canViewHiddenOption = user.HasPermission("Edit Report Hidden Option");
        var features = BuildFeatureFlags();
        var currentUserId = user.GetUserId();
        var query = _context.ReportObjects.AsNoTracking().Where(x => x.ReportObjectId == id);

        if (visibleOnly)
        {
            query = query
                .Where(x => x.DefaultVisibilityYn == "Y")
                .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N");
        }

        var report = await query
            .Select(x => new ReportDetailDto
            {
                Id = x.ReportObjectId,
                Name = x.Name,
                DisplayTitle = x.DisplayTitle,
                DisplayName = x.DisplayTitle ?? x.Name,
                Description = x.Description,
                DetailedDescription = x.DetailedDescription,
                TypeName = x.ReportObjectType != null ? x.ReportObjectType.Name : null,
                TypeShortName = x.ReportObjectType != null ? x.ReportObjectType.ShortName : null,
                Url = x.ReportObjectUrl,
                EpicMasterFile = x.EpicMasterFile,
                EpicRecordId = x.EpicRecordId,
                EpicReportTemplateId = x.EpicReportTemplateId,
                ReportServerPath = x.ReportServerPath,
                Availability = x.Availability,
                OrphanedReportObjectYn = x.OrphanedReportObjectYn,
                RepositoryDescription = x.RepositoryDescription,
                VisibleInSearch =
                    (x.OrphanedReportObjectYn ?? "N") == "N"
                    && x.ReportObjectType != null
                    && x.ReportObjectType.Visible == "Y"
                    && x.DefaultVisibilityYn == "Y"
                    && (x.ReportObjectDoc == null || (x.ReportObjectDoc.Hidden ?? "N") == "N"),
                Runs = x.Runs,
                LastModified = x.LastModifiedDate,
                LastLoadDate = x.LastLoadDate,
                CanEditDocumentation = canEditDocumentation,
                IsStarred = x.StarredReports.Any(y => y.Ownerid == currentUserId),
                Author = x.AuthorUser == null
                    ? null
                    : new UserSummaryDto
                    {
                        Id = x.AuthorUser.UserId,
                        Username = x.AuthorUser.Username,
                        FullName = x.AuthorUser.FullnameCalc ?? x.AuthorUser.DisplayName,
                        Email = x.AuthorUser.Email,
                    },
                LastModifiedBy = x.LastModifiedByUser == null
                    ? null
                    : new UserSummaryDto
                    {
                        Id = x.LastModifiedByUser.UserId,
                        Username = x.LastModifiedByUser.Username,
                        FullName = x.LastModifiedByUser.FullnameCalc ?? x.LastModifiedByUser.DisplayName,
                        Email = x.LastModifiedByUser.Email,
                    },
                Document = x.ReportObjectDoc == null
                    ? null
                    : new ReportDocumentDto
                    {
                        ReportObjectId = x.ReportObjectDoc.ReportObjectId,
                        GitLabProjectUrl = x.ReportObjectDoc.GitLabProjectUrl,
                        DeveloperDescription = x.ReportObjectDoc.DeveloperDescription,
                        KeyAssumptions = x.ReportObjectDoc.KeyAssumptions,
                        ExecutiveVisibilityYn = x.ReportObjectDoc.ExecutiveVisibilityYn,
                        LastUpdateDateTime = x.ReportObjectDoc.LastUpdateDateTime,
                        CreatedDateTime = x.ReportObjectDoc.CreatedDateTime,
                        EnabledForHyperspace = x.ReportObjectDoc.EnabledForHyperspace,
                        DoNotPurge = x.ReportObjectDoc.DoNotPurge,
                        Hidden = x.ReportObjectDoc.Hidden,
                        DeveloperNotes = x.ReportObjectDoc.DeveloperNotes,
                        OrganizationalValue = x.ReportObjectDoc.OrganizationalValue == null
                            ? null
                            : new LookupDto
                            {
                                Id = x.ReportObjectDoc.OrganizationalValue.Id,
                                Name = x.ReportObjectDoc.OrganizationalValue.Name,
                            },
                        EstimatedRunFrequency = x.ReportObjectDoc.EstimatedRunFrequency == null
                            ? null
                            : new LookupDto
                            {
                                Id = x.ReportObjectDoc.EstimatedRunFrequency.Id,
                                Name = x.ReportObjectDoc.EstimatedRunFrequency.Name,
                            },
                        Fragility = x.ReportObjectDoc.Fragility == null
                            ? null
                            : new LookupDto
                            {
                                Id = x.ReportObjectDoc.Fragility.Id,
                                Name = x.ReportObjectDoc.Fragility.Name,
                            },
                        MaintenanceSchedule = x.ReportObjectDoc.MaintenanceSchedule == null
                            ? null
                            : new LookupDto
                            {
                                Id = x.ReportObjectDoc.MaintenanceSchedule.Id,
                                Name = x.ReportObjectDoc.MaintenanceSchedule.Name,
                            },
                        OperationalOwner = x.ReportObjectDoc.OperationalOwnerUser == null
                            ? null
                            : new UserSummaryDto
                            {
                                Id = x.ReportObjectDoc.OperationalOwnerUser.UserId,
                                Username = x.ReportObjectDoc.OperationalOwnerUser.Username,
                                FullName = x.ReportObjectDoc.OperationalOwnerUser.FullnameCalc
                                    ?? x.ReportObjectDoc.OperationalOwnerUser.DisplayName,
                                Email = x.ReportObjectDoc.OperationalOwnerUser.Email,
                            },
                        Requester = x.ReportObjectDoc.RequesterNavigation == null
                            ? null
                            : new UserSummaryDto
                            {
                                Id = x.ReportObjectDoc.RequesterNavigation.UserId,
                                Username = x.ReportObjectDoc.RequesterNavigation.Username,
                                FullName = x.ReportObjectDoc.RequesterNavigation.FullnameCalc
                                    ?? x.ReportObjectDoc.RequesterNavigation.DisplayName,
                                Email = x.ReportObjectDoc.RequesterNavigation.Email,
                            },
                        UpdatedBy = x.ReportObjectDoc.UpdatedByNavigation == null
                            ? null
                            : new UserSummaryDto
                            {
                                Id = x.ReportObjectDoc.UpdatedByNavigation.UserId,
                                Username = x.ReportObjectDoc.UpdatedByNavigation.Username,
                                FullName = x.ReportObjectDoc.UpdatedByNavigation.FullnameCalc
                                    ?? x.ReportObjectDoc.UpdatedByNavigation.DisplayName,
                                Email = x.ReportObjectDoc.UpdatedByNavigation.Email,
                            },
                        FragilityTags = x.ReportObjectDoc.ReportObjectDocFragilityTags
                            .OrderBy(y => y.FragilityTag.Name)
                            .Select(y => new LookupDto
                            {
                                Id = y.FragilityTag.Id,
                                Name = y.FragilityTag.Name,
                            })
                            .ToList(),
                        MaintenanceLogs = x.ReportObjectDoc.MaintenanceLogs
                            .OrderByDescending(y => y.MaintenanceDate)
                            .Select(y => new ReportMaintenanceLogDto
                            {
                                Id = y.MaintenanceLogId,
                                MaintenanceDate = y.MaintenanceDate,
                                Comment = y.Comment,
                                Status = y.MaintenanceLogStatus == null
                                    ? null
                                    : new LookupDto
                                    {
                                        Id = y.MaintenanceLogStatus.Id,
                                        Name = y.MaintenanceLogStatus.Name,
                                    },
                                Maintainer = y.Maintainer == null
                                    ? null
                                    : new UserSummaryDto
                                    {
                                        Id = y.Maintainer.UserId,
                                        Username = y.Maintainer.Username,
                                        FullName = y.Maintainer.FullnameCalc ?? y.Maintainer.DisplayName,
                                        Email = y.Maintainer.Email,
                                    },
                            })
                            .ToList(),
                        ServiceRequests = x.ReportObjectDoc.ReportServiceRequests
                            .OrderByDescending(y => y.ServiceRequestId)
                            .Select(y => new ReportServiceRequestDto
                            {
                                Id = y.ServiceRequestId,
                                TicketNumber = y.TicketNumber,
                                Description = y.Description,
                                TicketUrl = y.TicketUrl,
                            })
                            .ToList(),
                    },
                HeaderTags = x.ReportTagLinks
                    .OrderBy(y => y.Tag.Priority)
                    .ThenBy(y => y.Tag.Name)
                    .Select(y => new ReportTagDto
                    {
                        Id = y.TagId,
                        Name = y.Tag.Name,
                        Description = y.Tag.Description,
                        Priority = y.Tag.Priority,
                        ShowInHeader = y.ShowInHeader,
                    })
                    .ToList(),
                ObjectTags = x.ReportObjectTagMemberships
                    .OrderBy(y => y.Line)
                    .ThenBy(y => y.Tag.TagName)
                    .Select(y => new ReportObjectTagDto
                    {
                        Id = y.TagId,
                        Name = y.Tag.TagName,
                        Line = y.Line,
                    })
                    .ToList(),
                Attachments = x.ReportObjectAttachments
                    .OrderBy(y => y.Name)
                    .Select(y => new ReportAttachmentDto
                    {
                        Id = y.ReportObjectAttachmentId,
                        Name = y.Name,
                        Path = y.Path,
                        Source = y.Source,
                        Type = y.Type,
                        CreationDate = y.CreationDate,
                    })
                    .ToList(),
                Images = x.ReportObjectImagesDocs
                    .OrderBy(y => y.ImageOrdinal)
                    .Select(y => new ReportImageDto
                    {
                        Id = y.ImageId,
                        Ordinal = y.ImageOrdinal,
                        Source = y.ImageSource,
                    })
                    .ToList(),
                Groups = x.ReportGroupsMemberships
                    .OrderBy(y => y.Group.GroupName)
                    .Select(y => new GroupSummaryDto
                    {
                        Id = y.GroupId,
                        Name = y.Group.GroupName,
                        Email = y.Group.GroupEmail,
                        Type = y.Group.GroupType,
                    })
                    .ToList(),
                Collections = x.CollectionReports
                    .OrderBy(y => y.Rank)
                    .ThenBy(y => y.DataProject.Name)
                    .Select(y => new CollectionSummaryDto
                    {
                        Id = y.CollectionId,
                        Name = y.DataProject.Name,
                        Rank = y.Rank,
                    })
                    .ToList(),
                Parameters = x.ReportObjectParameters
                    .OrderBy(y => y.ParameterName)
                    .Select(y => new ReportParameterDto
                    {
                        Id = y.ReportObjectParameterId,
                        Name = y.ParameterName,
                        Value = y.ParameterValue,
                    })
                    .ToList(),
                Queries = x.ReportObjectQueries
                    .OrderBy(y => y.Name)
                    .Select(y => new ReportQueryDto
                    {
                        Id = y.ReportObjectQueryId,
                        ReportObjectId = y.ReportObjectId,
                        Name = y.Name,
                        Language = y.Language,
                        SourceServer = y.SourceServer,
                        Query = y.Query,
                        LastLoadDate = y.LastLoadDate,
                    })
                    .ToList(),
                StarCount = x.StarredReports.Count,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (report == null)
        {
            return null;
        }

        report.Features = features;
        report.Terms = await GetTermsAsync(id, cancellationToken);
        report.ComponentQueries = await GetComponentQueriesAsync(id, cancellationToken);
        report.Children = await GetChildrenAsync(id, cancellationToken);
        report.Parents = await GetParentsAsync(id, cancellationToken);
        report.MaintenanceStatus = await GetMaintenanceStatusAsync(id, cancellationToken);
        report.CanRun = await CanRunReportAsync(user, id, cancellationToken);
        ApplyDetailVisibility(
            report,
            canEditDocumentation,
            canViewGroups,
            canViewPurgeOption,
            canViewHiddenOption
        );
        ApplyReportActions(report, user);

        return report;
    }

    private async Task<IReadOnlyList<TermSummaryDto>> GetTermsAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Terms.AsNoTracking()
            .Where(x => x.ReportObjectDocTerms.Any(y => y.ReportObjectId == id))
            .Union(
                _context.Terms.Where(x =>
                    x.ReportObjectDocTerms.Any(y =>
                        y.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                            z.ParentReportObjectId == id
                        )
                    )
                )
            )
            .Union(
                _context.Terms.Where(x =>
                    x.ReportObjectDocTerms.Any(y =>
                        y.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                            z.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(a =>
                                a.ParentReportObjectId == id
                            )
                        )
                    )
                )
            )
            .Union(
                _context.Terms.Where(x =>
                    x.ReportObjectDocTerms.Any(y =>
                        y.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                            z.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(a =>
                                a.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(b =>
                                    b.ParentReportObjectId == id
                                )
                            )
                        )
                    )
                )
            )
            .Union(
                _context.Terms.Where(x =>
                    x.ReportObjectDocTerms.Any(y =>
                        y.ReportObject.ReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                            z.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(a =>
                                a.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(b =>
                                    b.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(c =>
                                        c.ParentReportObjectId == id
                                    )
                                )
                            )
                        )
                    )
                )
            )
            .Distinct()
            .OrderBy(x => x.Name)
            .Select(x => new TermSummaryDto
            {
                Id = x.TermId,
                Name = x.Name,
                Summary = x.Summary,
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<ReportQueryDto>> GetComponentQueriesAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .ReportObjectQueries.AsNoTracking()
            .Where(x =>
                x.ReportObject.ReportObjectHierarchyChildReportObjects.Any(y =>
                    y.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                        z.ParentReportObjectId == id && z.ParentReportObject.EpicMasterFile == "IDB"
                    )
                )
            )
            .OrderBy(x => x.Name)
            .Select(x => new ReportQueryDto
            {
                Id = x.ReportObjectQueryId,
                ReportObjectId = x.ReportObjectId,
                Name = x.Name,
                Language = x.Language,
                SourceServer = x.SourceServer,
                Query = x.Query,
                LastLoadDate = x.LastLoadDate,
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<ReportLinkSummaryDto>> GetChildrenAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .ReportObjects.AsNoTracking()
            .Where(x =>
                x.ReportObjectHierarchyChildReportObjects.Any(y => y.ParentReportObjectId == id)
            )
            .Where(x => x.EpicMasterFile != "IDK")
            .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
            .Where(x => x.DefaultVisibilityYn == "Y")
            .Union(
                _context.ReportObjects.Where(x =>
                    x.ReportObjectHierarchyChildReportObjects.Any(y =>
                        y.ParentReportObject.ReportObjectHierarchyChildReportObjects.Any(z =>
                            z.ParentReportObjectId == id
                            && z.ParentReportObject.DefaultVisibilityYn == "Y"
                        )
                        && y.ParentReportObject.EpicMasterFile == "IDK"
                    )
                )
                .Where(x => x.EpicMasterFile == "IDN")
                .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
            )
            .OrderBy(x => x.DisplayTitle ?? x.Name)
            .Select(x => new ReportLinkSummaryDto
            {
                Id = x.ReportObjectId,
                Name = x.DisplayTitle ?? x.Name,
                Type = x.ReportObjectType != null ? x.ReportObjectType.ShortName : null,
                Url = x.ReportObjectUrl,
                LastModified = x.LastModifiedDate,
                AttachmentCount = x.ReportObjectAttachments.Count,
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<ReportLinkSummaryDto>> GetParentsAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .ReportObjects.AsNoTracking()
            .Where(x =>
                x.ReportObjectHierarchyParentReportObjects.Any(y => y.ChildReportObjectId == id)
            )
            .Where(x => x.ReportObjectTypeId != 12)
            .Where(x => x.EpicMasterFile != "IDK")
            .Where(x => x.DefaultVisibilityYn == "Y")
            .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
            .Union(
                _context.ReportObjects.Where(x =>
                    x.ReportObjectHierarchyParentReportObjects.Any(y =>
                        y.ChildReportObject.ReportObjectHierarchyParentReportObjects.Any(z =>
                            z.ChildReportObjectId == id
                        )
                        && y.ChildReportObject.EpicMasterFile == "IDK"
                    )
                )
                .Where(x => x.EpicMasterFile == "IDB")
                .Where(x => x.DefaultVisibilityYn == "Y")
                .Where(x => (x.ReportObjectDoc.Hidden ?? "N") == "N")
            )
            .OrderBy(x => x.DisplayTitle ?? x.Name)
            .Select(x => new ReportLinkSummaryDto
            {
                Id = x.ReportObjectId,
                Name = x.DisplayTitle ?? x.Name,
                Type = x.ReportObjectType != null ? x.ReportObjectType.ShortName : null,
                Url = x.ReportObjectUrl,
                LastModified = x.LastModifiedDate,
                AttachmentCount = x.ReportObjectAttachments.Count,
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<ReportMaintenanceStatusDto> GetMaintenanceStatusAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        var maintenanceData = await _context
            .ReportObjectDocs.AsNoTracking()
            .Where(x => x.ReportObjectId == id && (x.MaintenanceScheduleId ?? 1) != 5)
            .Select(x => new
            {
                ScheduleId = x.MaintenanceScheduleId ?? 1,
                ScheduleName = x.MaintenanceSchedule != null ? x.MaintenanceSchedule.Name : null,
                LastMaintenanceDate = x.MaintenanceLogs.Max(y => y.MaintenanceDate),
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (maintenanceData == null)
        {
            return null;
        }

        var today = DateTime.UtcNow;
        var baseDate = maintenanceData.LastMaintenanceDate ?? today;
        var nextMaintenanceDate = maintenanceData.ScheduleId switch
        {
            1 => baseDate.AddMonths(3),
            2 => baseDate.AddMonths(6),
            3 => baseDate.AddYears(1),
            4 => baseDate.AddYears(2),
            _ => baseDate,
        };

        var isRequired = nextMaintenanceDate < today;
        return new ReportMaintenanceStatusDto
        {
            IsRequired = isRequired,
            Message = isRequired ? "Report requires maintenance." : null,
            LastMaintenanceDate = maintenanceData.LastMaintenanceDate,
            NextMaintenanceDate = nextMaintenanceDate,
            Schedule = new LookupDto
            {
                Id = maintenanceData.ScheduleId,
                Name = maintenanceData.ScheduleName,
            },
        };
    }

    private void ApplyReportActions(
        ReportDetailDto report,
        ClaimsPrincipal user
    )
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        var actionReport = new ReportObject
        {
            ReportObjectId = report.Id,
            Name = report.Name,
            DisplayTitle = report.DisplayTitle,
            Description = report.Description,
            ReportObjectUrl = report.Url,
            EpicMasterFile = report.EpicMasterFile,
            EpicRecordId = report.EpicRecordId,
            EpicReportTemplateId = report.EpicReportTemplateId,
            ReportServerPath = report.ReportServerPath,
            Availability = report.Availability,
            OrphanedReportObjectYn = "N",
            ReportObjectType = new ReportObjectType
            {
                Name = report.TypeName,
                ShortName = report.TypeShortName,
            },
            ReportObjectDoc = report.Document == null
                ? null
                : new ReportObjectDoc
                {
                    EnabledForHyperspace = report.Document.EnabledForHyperspace,
                },
        };

        report.RunUrl = actionReport.RunReportUrl(
            httpContext,
            _configuration,
            report.CanRun
        );
        report.RecordViewerUrl = actionReport.RecordViewerUrl(httpContext);
        report.CanViewUserProfiles =
            report.Features?.UserProfilesEnabled == true
            && user.HasPermission("View Other User");
        if (user.HasPermission("Open In Editor"))
        {
            report.EditReportUrl = actionReport.EditReportUrl(httpContext, _configuration);
            report.ManageReportUrl = actionReport.ManageReportUrl(httpContext, _configuration);
        }

        var basePath = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        foreach (var attachment in report.Attachments)
        {
            attachment.RunUrl = $"{basePath}/Data/File?handler=CrystalRun&id={attachment.Id}";
        }
    }

    private static void ApplyDetailVisibility(
        ReportDetailDto report,
        bool canEditDocumentation,
        bool canViewGroups,
        bool canViewPurgeOption,
        bool canViewHiddenOption
    )
    {
        if (canEditDocumentation || report.Document == null)
        {
            if (report.Document != null)
            {
                report.Document = new ReportDocumentDto
                {
                    ReportObjectId = report.Document.ReportObjectId,
                    GitLabProjectUrl = report.Document.GitLabProjectUrl,
                    DeveloperDescription = report.Document.DeveloperDescription,
                    KeyAssumptions = report.Document.KeyAssumptions,
                    ExecutiveVisibilityYn = report.Document.ExecutiveVisibilityYn,
                    LastUpdateDateTime = report.Document.LastUpdateDateTime,
                    CreatedDateTime = report.Document.CreatedDateTime,
                    EnabledForHyperspace = report.Document.EnabledForHyperspace,
                    DoNotPurge = canViewPurgeOption ? report.Document.DoNotPurge : null,
                    Hidden = canViewHiddenOption ? report.Document.Hidden : null,
                    DeveloperNotes = report.Document.DeveloperNotes,
                    OrganizationalValue = report.Document.OrganizationalValue,
                    EstimatedRunFrequency = report.Document.EstimatedRunFrequency,
                    Fragility = report.Document.Fragility,
                    MaintenanceSchedule = report.Document.MaintenanceSchedule,
                    OperationalOwner = report.Document.OperationalOwner,
                    Requester = report.Document.Requester,
                    UpdatedBy = report.Document.UpdatedBy,
                    FragilityTags = report.Document.FragilityTags,
                    MaintenanceLogs = report.Document.MaintenanceLogs,
                    ServiceRequests = report.Document.ServiceRequests,
                };
            }
            if (!canViewGroups)
            {
                report.Groups = Array.Empty<GroupSummaryDto>();
            }
            if (report.Features?.TermsEnabled != true)
            {
                report.Terms = Array.Empty<TermSummaryDto>();
            }
            report.CanViewGroups = canViewGroups;
            return;
        }

        report.Document = new ReportDocumentDto
        {
            ReportObjectId = report.Document.ReportObjectId,
            DeveloperDescription = report.Document.DeveloperDescription,
            KeyAssumptions = report.Document.KeyAssumptions,
            ExecutiveVisibilityYn = report.Document.ExecutiveVisibilityYn,
            LastUpdateDateTime = report.Document.LastUpdateDateTime,
            CreatedDateTime = report.Document.CreatedDateTime,
            OrganizationalValue = report.Document.OrganizationalValue,
            EstimatedRunFrequency = report.Document.EstimatedRunFrequency,
            Fragility = report.Document.Fragility,
            MaintenanceSchedule = report.Document.MaintenanceSchedule,
            OperationalOwner = report.Document.OperationalOwner,
            Requester = report.Document.Requester,
            UpdatedBy = report.Document.UpdatedBy,
            DoNotPurge = canViewPurgeOption ? report.Document.DoNotPurge : null,
            Hidden = canViewHiddenOption ? report.Document.Hidden : null,
            MaintenanceLogs = report.Document.MaintenanceLogs,
            FragilityTags = report.Document.FragilityTags,
            ServiceRequests = report.Document.ServiceRequests,
        };

        if (report.Features?.TermsEnabled != true)
        {
            report.Terms = Array.Empty<TermSummaryDto>();
        }
        report.CanViewGroups = canViewGroups;
        if (!canViewGroups)
        {
            report.Groups = Array.Empty<GroupSummaryDto>();
        }
    }
}
