using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services.Seeding;

public sealed class DemoDataSeeder
{
    public const string SeedMarkerName = "demo_seed_version";

    private static readonly DateTime SeedTimestamp = new(
        2026,
        8,
        12,
        9,
        0,
        0,
        DateTimeKind.Utc
    );

    private static readonly string[] Departments =
    [
        "Enterprise Analytics",
        "Clinical Operations",
        "Revenue Cycle",
        "Quality and Safety",
        "Population Health",
        "Finance",
    ];

    private static readonly (string Name, string Description)[] ReportDefinitions =
    [
        ("Daily Emergency Department Census", "Hourly arrivals, departures, boarding, and current occupancy."),
        ("Inpatient Length of Stay", "Length-of-stay trends by service line, unit, and discharge disposition."),
        ("Readmission Risk Worklist", "Patients with elevated thirty-day readmission risk and active interventions."),
        ("Operating Room Utilization", "Room utilization, turnover time, first-case starts, and block performance."),
        ("Patient Experience Trends", "Survey response and domain scores by facility, unit, and provider."),
        ("Hospital-Acquired Infection Surveillance", "Current infection events, device days, and standardized ratios."),
        ("Medication Safety Events", "Reported medication events categorized by severity and contributing factor."),
        ("Sepsis Bundle Compliance", "Timeliness and completion of evidence-based sepsis bundle elements."),
        ("Falls With Injury", "Patient falls, injury severity, location, and prevention interventions."),
        ("Clinical Documentation Integrity", "Documentation opportunities and query response performance."),
        ("Net Revenue Forecast", "Monthly net revenue forecast compared with budget and prior year."),
        ("Accounts Receivable Aging", "Open receivables by aging bucket, payer, service area, and financial class."),
        ("Denial Prevention Dashboard", "Initial denials, overturn rates, root causes, and avoidable write-offs."),
        ("Point-of-Service Collections", "Collections performance by location, registrar, and encounter type."),
        ("Charge Capture Reconciliation", "Potential missing charges and late charge activity by department."),
        ("Labor Productivity", "Worked hours, productive hours, volume, and productivity variance."),
        ("Contract Performance", "Payer contract yield and reimbursement variance by service category."),
        ("Population Health Registry", "Care gaps and outreach status for attributed patient populations."),
        ("Diabetes Outcomes", "A1C control, screening compliance, and follow-up across primary care panels."),
        ("Hypertension Control", "Blood pressure control and follow-up measures by clinic and provider."),
        ("Preventive Screening Gaps", "Open breast, colorectal, and cervical cancer screening opportunities."),
        ("Ambulatory Access", "Third-next-available appointment and visit demand by specialty."),
        ("Referral Leakage", "Internal and external referral completion patterns by specialty."),
        ("Provider Panel Capacity", "Panel size, risk adjustment, visit demand, and available capacity."),
        ("Supply Expense Variance", "Supply expense and utilization variance by facility and cost center."),
        ("Case Mix Index", "Case mix trends by service line, payer, and attending provider."),
        ("Observation Utilization", "Observation stays, conversion rates, and hours by clinical service."),
        ("Discharge Before Noon", "Discharge timing and barriers by unit, provider, and disposition."),
        ("Bed Capacity Forecast", "Seven-day demand and staffed-bed capacity forecast by campus."),
        ("Executive Quality Scorecard", "Balanced view of safety, quality, experience, access, and finance measures."),
    ];

    private static readonly (string Name, string Summary)[] TermDefinitions =
    [
        ("Average Length of Stay", "Average inpatient days from admission through discharge."),
        ("Readmission", "An inpatient admission occurring within thirty days of a qualifying discharge."),
        ("Emergency Department Boarding", "Time a patient waits in the emergency department after admission decision."),
        ("Case Mix Index", "Relative clinical complexity and resource needs of discharged patients."),
        ("Net Revenue", "Expected revenue after contractual allowances and other adjustments."),
        ("Initial Denial Rate", "Percentage of submitted claims denied on the initial payer response."),
        ("Days in Accounts Receivable", "Average number of days required to collect patient service revenue."),
        ("Hospital-Acquired Infection", "An infection meeting surveillance criteria after healthcare exposure."),
        ("Patient Fall", "An unplanned descent to the floor with or without injury."),
        ("Sepsis Bundle Compliance", "Completion of required sepsis interventions within defined time windows."),
        ("Third Next Available Appointment", "Standard measure of appointment access excluding immediate openings."),
        ("Care Gap", "A recommended service that has not been completed within the expected interval."),
        ("Attributed Patient", "A patient assigned to an accountable clinician or care organization."),
        ("Productive Hours", "Paid hours associated directly with delivering patient care or operational output."),
        ("Operating Room Utilization", "Used operating room time divided by allocated room time."),
        ("Discharge Before Noon", "An inpatient discharge completed before 12:00 local time."),
        ("Observation Stay", "Outpatient hospital services used to evaluate the need for inpatient admission."),
        ("Staffed Bed", "A licensed bed supported by staff and available for patient placement."),
    ];

    private readonly Atlas_WebContext _context;
    private readonly ILogger<DemoDataSeeder> _logger;

    public DemoDataSeeder(Atlas_WebContext context, ILogger<DemoDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(
        string seedVersion,
        string adminUsername,
        bool resetExistingData = false,
        CancellationToken cancellationToken = default
    )
    {
        var appliedVersion = await _context
            .GlobalSiteSettings.Where(setting => setting.Name == SeedMarkerName)
            .Select(setting => setting.Value)
            .SingleOrDefaultAsync(cancellationToken);

        if (appliedVersion == seedVersion)
        {
            _logger.LogInformation("Demo seed version {SeedVersion} is already applied", seedVersion);
            return;
        }

        var containsApplicationData =
            await _context.Users.AnyAsync(cancellationToken)
            || await _context.UserGroups.AnyAsync(cancellationToken)
            || await _context.ReportObjects.AnyAsync(cancellationToken)
            || await _context.ReportObjectDocs.AnyAsync(cancellationToken)
            || await _context.ReportObjectRunDatas.AnyAsync(cancellationToken)
            || await _context.ReportObjectSubscriptions.AnyAsync(cancellationToken)
            || await _context.Terms.AnyAsync(cancellationToken)
            || await _context.Collections.AnyAsync(cancellationToken)
            || await _context.Initiatives.AnyAsync(cancellationToken)
            || await _context.UserFavoriteFolders.AnyAsync(cancellationToken)
            || await _context.StarredReports.AnyAsync(cancellationToken)
            || await _context.SharedItems.AnyAsync(cancellationToken);
        if (containsApplicationData && !resetExistingData)
        {
            throw new InvalidOperationException(
                "Demo data already exists. Set DEMO_SEED_RESET=true to replace it."
            );
        }

        _logger.LogWarning(
            "Replacing demo database data with seed version {SeedVersion}",
            seedVersion
        );

        if (resetExistingData)
        {
            _context.ChangeTracker.Clear();
            await _context.Database.EnsureDeletedAsync(cancellationToken);
            if (_context.Database.IsRelational())
            {
                await _context.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                await _context.Database.EnsureCreatedAsync(cancellationToken);
            }
        }
        else if (!_context.Database.IsRelational())
        {
            await _context.Database.EnsureCreatedAsync(cancellationToken);
        }

        var normalizedAdminUsername = string.IsNullOrWhiteSpace(adminUsername)
            ? "Default"
            : adminUsername.Trim();
        if (_context.Database.IsRelational())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                cancellationToken
            );
            try
            {
                await SeedDatabaseAsync(seedVersion, normalizedAdminUsername, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            await SeedDatabaseAsync(seedVersion, normalizedAdminUsername, cancellationToken);
        }

        _logger.LogInformation("Applied demo seed version {SeedVersion}", seedVersion);
    }

    private async Task SeedDatabaseAsync(
        string seedVersion,
        string adminUsername,
        CancellationToken cancellationToken
    )
    {
        var now = SeedTimestamp;
        var users = CreateUsers(adminUsername, now);
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync(cancellationToken);

        var admin = users[0];
        var roleNames = new[] { "Administrator", "Report Writer", "User" };
        var rolesByName = await _context
            .UserRoles.Where(role => roleNames.Contains(role.Name))
            .ToDictionaryAsync(role => role.Name, cancellationToken);
        UserRole GetOrCreateRole(string name, string description)
        {
            if (rolesByName.TryGetValue(name, out var existingRole))
            {
                return existingRole;
            }

            var role = new UserRole { Name = name, Description = description };
            _context.UserRoles.Add(role);
            rolesByName[name] = role;
            return role;
        }

        var roles = new[]
        {
            GetOrCreateRole(
                "Administrator",
                "Full administration access for the demo environment."
            ),
            GetOrCreateRole(
                "Report Writer",
                "Maintains report documentation and business terminology."
            ),
            GetOrCreateRole("User", "Searches, views, and personalizes library content."),
        };
        _context.UserRoleLinks.AddRange(
            new UserRoleLink { User = admin, UserRoles = roles[0] },
            new UserRoleLink { User = users[1], UserRoles = roles[1] },
            new UserRoleLink { User = users[2], UserRoles = roles[1] }
        );
        foreach (var user in users.Skip(3))
        {
            _context.UserRoleLinks.Add(new UserRoleLink { User = user, UserRoles = roles[2] });
        }

        var groups = CreateGroups();
        _context.UserGroups.AddRange(groups);
        for (var userIndex = 0; userIndex < users.Count; userIndex++)
        {
            _context.UserGroupsMemberships.Add(
                new UserGroupsMembership
                {
                    User = users[userIndex],
                    Group = groups[userIndex % groups.Count],
                    LastLoadDate = now,
                }
            );
            if (userIndex < 4)
            {
                _context.UserGroupsMemberships.Add(
                    new UserGroupsMembership
                    {
                        User = users[userIndex],
                        Group = groups[^1],
                        LastLoadDate = now,
                    }
                );
            }
        }

        var reportTypeDefinitions = new[]
        {
            ("Power BI Report", "Power BI"),
            ("SSRS Report", "SSRS"),
            ("Tableau Workbook", "Tableau"),
            ("Epic Dashboard", "Radar"),
        };
        var reportTypeNames = reportTypeDefinitions.Select(definition => definition.Item1).ToArray();
        var reportTypesByName = await _context
            .ReportObjectTypes.Where(type => reportTypeNames.Contains(type.Name))
            .ToDictionaryAsync(type => type.Name, cancellationToken);
        var reportTypes = reportTypeDefinitions
            .Select(definition =>
            {
                if (reportTypesByName.TryGetValue(definition.Item1, out var existingType))
                {
                    if (string.IsNullOrWhiteSpace(existingType.ShortName))
                    {
                        existingType.ShortName = definition.Item2;
                    }
                    if (string.IsNullOrWhiteSpace(existingType.Visible))
                    {
                        existingType.Visible = "Y";
                    }
                    return existingType;
                }

                var reportType = new ReportObjectType
                {
                    Name = definition.Item1,
                    ShortName = definition.Item2,
                    Visible = "Y",
                    LastLoadDate = now,
                };
                _context.ReportObjectTypes.Add(reportType);
                return reportType;
            })
            .ToArray();

        var scheduleNames = new[] { "Quarterly", "Yearly", "Audit Only" };
        var schedulesByName = await _context
            .MaintenanceSchedules.Where(schedule => scheduleNames.Contains(schedule.Name))
            .ToDictionaryAsync(schedule => schedule.Name, cancellationToken);
        var maintenanceSchedules = scheduleNames
            .Select(name =>
            {
                if (schedulesByName.TryGetValue(name, out var existingSchedule))
                {
                    return existingSchedule;
                }

                var schedule = new MaintenanceSchedule { Name = name };
                _context.MaintenanceSchedules.Add(schedule);
                return schedule;
            })
            .ToArray();

        var statusNames = new[]
        {
            "Approved - No Changes",
            "Approved - With Changes",
            "Recommend Retire",
        };
        var statusesByName = await _context
            .MaintenanceLogStatuses.Where(status => statusNames.Contains(status.Name))
            .ToDictionaryAsync(status => status.Name, cancellationToken);
        var maintenanceStatuses = statusNames
            .Select(name =>
            {
                if (statusesByName.TryGetValue(name, out var existingStatus))
                {
                    return existingStatus;
                }

                var status = new MaintenanceLogStatus { Name = name };
                _context.MaintenanceLogStatuses.Add(status);
                return status;
            })
            .ToArray();

        var initiatives = CreateInitiatives(users, now);
        _context.Initiatives.AddRange(initiatives);

        var collections = CreateCollections(initiatives, users, now);
        _context.Collections.AddRange(collections);

        var terms = CreateTerms(users, now);
        _context.Terms.AddRange(terms);

        var reports = CreateReports(users, reportTypes, now);
        _context.ReportObjects.AddRange(reports);
        await _context.SaveChangesAsync(cancellationToken);

        var reportTags = new[]
        {
            new ReportObjectTag { TagName = "Executive" },
            new ReportObjectTag { TagName = "Operational" },
            new ReportObjectTag { TagName = "Quality" },
            new ReportObjectTag { TagName = "Financial" },
            new ReportObjectTag { TagName = "Population Health" },
        };
        _context.ReportObjectTags.AddRange(reportTags);

        for (var index = 0; index < reports.Count; index++)
        {
            var report = reports[index];
            if (index < 26)
            {
                var reportDoc = new ReportObjectDoc
                {
                    ReportObject = report,
                    OperationalOwnerUser = users[index % users.Count],
                    RequesterNavigation = users[(index + 2) % users.Count],
                    DeveloperDescription =
                        $"Curated semantic model for {report.Name.ToLowerInvariant()}.",
                    KeyAssumptions =
                        "Data is refreshed nightly; incomplete encounters are excluded.",
                    ExecutiveVisibilityYn = index % 4 == 0 ? "Y" : "N",
                    MaintenanceSchedule = maintenanceSchedules[index % maintenanceSchedules.Length],
                    LastUpdateDateTime = now.AddDays(-(index + 1)),
                    CreatedDateTime = now.AddMonths(-8).AddDays(index),
                    CreatedBy = admin.UserId,
                    UpdatedByNavigation = users[index % users.Count],
                    EnabledForHyperspace = "Y",
                    DoNotPurge = "N",
                    Hidden = "N",
                    DeveloperNotes = "Validated against source-system control totals.",
                };
                _context.ReportObjectDocs.Add(reportDoc);
                _context.ReportObjectDocTerms.AddRange(
                    new ReportObjectDocTerm
                    {
                        ReportObject = reportDoc,
                        Term = terms[index % terms.Count],
                    },
                    new ReportObjectDocTerm
                    {
                        ReportObject = reportDoc,
                        Term = terms[(index + 3) % terms.Count],
                    }
                );
            }

            _context.ReportObjectQueries.Add(
                new ReportObjectQuery
                {
                    ReportObject = report,
                    Name = "Primary dataset",
                    Language = "SQL",
                    SourceServer = report.SourceServer,
                    Query =
                        $"SELECT * FROM analytics.fact_measure WHERE report_key = '{report.ReportObjectBizKey}'",
                    LastLoadDate = now,
                }
            );
            _context.ReportObjectParameters.AddRange(
                new ReportObjectParameter
                {
                    ReportObject = report,
                    ParameterName = "StartDate",
                    ParameterValue = now.AddDays(-30).ToString("yyyy-MM-dd"),
                },
                new ReportObjectParameter
                {
                    ReportObject = report,
                    ParameterName = "Facility",
                    ParameterValue = index % 2 == 0 ? "Central Hospital" : "North Medical Center",
                }
            );
            _context.ReportObjectTagMemberships.Add(
                new ReportObjectTagMembership
                {
                    ReportObject = report,
                    Tag = reportTags[index % reportTags.Length],
                    Line = 1,
                }
            );
            _context.ReportGroupsMemberships.Add(
                new ReportGroupsMembership
                {
                    Report = report,
                    Group = groups[index % groups.Count],
                    LastLoadDate = now,
                }
            );
        }

        for (var index = 0; index < reports.Count - 1; index += 5)
        {
            _context.ReportObjectHierarchies.Add(
                new ReportObjectHierarchy
                {
                    ParentReportObject = reports[index],
                    ChildReportObject = reports[index + 1],
                    Line = 1,
                    LastLoadDate = now,
                }
            );
        }

        for (var collectionIndex = 0; collectionIndex < collections.Count; collectionIndex++)
        {
            var collection = collections[collectionIndex];
            for (var offset = 0; offset < 4; offset++)
            {
                _context.CollectionReports.Add(
                    new CollectionReport
                    {
                        DataProject = collection,
                        Report = reports[(collectionIndex * 4 + offset) % reports.Count],
                        Rank = offset + 1,
                    }
                );
            }
            for (var offset = 0; offset < 3; offset++)
            {
                _context.CollectionTerms.Add(
                    new CollectionTerm
                    {
                        DataProject = collection,
                        Term = terms[(collectionIndex * 2 + offset) % terms.Count],
                        Rank = offset + 1,
                    }
                );
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var report in reports.Take(12))
        {
            _context.MaintenanceLogs.Add(
                new MaintenanceLog
                {
                    ReportObjectDoc = report.ReportObjectDoc,
                    Maintainer = users[reports.IndexOf(report) % users.Count],
                    MaintenanceDate = now.AddDays(-(reports.IndexOf(report) * 8 + 2)),
                    Comment = "Validated filters, totals, ownership, and refresh schedule.",
                    MaintenanceLogStatus = maintenanceStatuses[
                        reports.IndexOf(report) % maintenanceStatuses.Length
                    ],
                }
            );
        }

        CreateRunHistory(reports, users, now);

        var folders = new[]
        {
            new UserFavoriteFolder
            {
                FolderName = "Daily Operations",
                UserId = admin.UserId,
                FolderRank = 1,
            },
            new UserFavoriteFolder
            {
                FolderName = "Executive Review",
                UserId = admin.UserId,
                FolderRank = 2,
            },
        };
        _context.UserFavoriteFolders.AddRange(folders);
        await _context.SaveChangesAsync(cancellationToken);

        for (var index = 0; index < 8; index++)
        {
            _context.StarredReports.Add(
                new StarredReport
                {
                    Report = reports[index],
                    Owner = admin,
                    Folder = folders[index % folders.Length],
                    Rank = index + 1,
                }
            );
        }
        _context.StarredCollections.AddRange(
            new StarredCollection
            {
                Collection = collections[0],
                Owner = admin,
                Folder = folders[0],
                Rank = 1,
            },
            new StarredCollection
            {
                Collection = collections[1],
                Owner = admin,
                Folder = folders[1],
                Rank = 2,
            }
        );
        _context.StarredInitiatives.Add(
            new StarredInitiative
            {
                Initiative = initiatives[0],
                Owner = admin,
                Folder = folders[1],
                Rank = 1,
            }
        );
        _context.StarredTerms.AddRange(
            new StarredTerm
            {
                Term = terms[0],
                Owner = admin,
                Folder = folders[0],
                Rank = 1,
            },
            new StarredTerm
            {
                Term = terms[1],
                Owner = admin,
                Folder = folders[0],
                Rank = 2,
            }
        );
        _context.StarredGroups.Add(
            new StarredGroup
            {
                Group = groups[0],
                Owner = admin,
                Folder = folders[1],
                Rank = 1,
            }
        );

        for (var index = 0; index < 6; index++)
        {
            _context.ReportObjectSubscriptions.Add(
                new ReportObjectSubscription
                {
                    ReportObject = reports[index],
                    User = admin,
                    SubscriptionId = $"DEMO-SUB-{index + 1:000}",
                    InactiveFlags = index == 5 ? 1 : 0,
                    EmailList = admin.Email,
                    Description = $"Scheduled delivery of {reports[index].Name}",
                    LastStatus = index == 4 ? "Failed" : "Delivered",
                    LastRunTime = now.AddDays(-(index + 1)),
                    SubscriptionTo = index % 2 == 0 ? "Email" : "Library",
                    LastLoadDate = now,
                }
            );
        }

        for (var index = 0; index < 4; index++)
        {
            _context.SharedItems.Add(
                new SharedItem
                {
                    SharedFromUser = users[index + 1],
                    SharedToUser = admin,
                    Url = $"/reports?id={reports[index].ReportObjectId}",
                    Name = reports[index].Name,
                    ShareDate = now.AddDays(-(index + 1)),
                }
            );
        }

        _context.GlobalSiteSettings.Add(
            new GlobalSiteSetting
            {
                Name = SeedMarkerName,
                Description = "Applied demo data seed version",
                Value = seedVersion,
            }
        );

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static List<User> CreateUsers(string adminUsername, DateTime now)
    {
        var people = new[]
        {
            ("Local", "Admin", "Enterprise Analytics", "Analytics Administrator"),
            ("Maya", "Patel", "Enterprise Analytics", "Director of Analytics"),
            ("Daniel", "Kim", "Clinical Operations", "Clinical Informatics Manager"),
            ("Avery", "Johnson", "Revenue Cycle", "Revenue Integrity Director"),
            ("Sofia", "Martinez", "Quality and Safety", "Patient Safety Analyst"),
            ("Noah", "Williams", "Population Health", "Population Health Manager"),
            ("Emma", "Brown", "Finance", "Senior Financial Analyst"),
            ("Liam", "Davis", "Clinical Operations", "Nursing Operations Analyst"),
            ("Olivia", "Wilson", "Enterprise Analytics", "Business Intelligence Developer"),
            ("Ethan", "Anderson", "Revenue Cycle", "Denials Analytics Lead"),
            ("Isabella", "Thomas", "Quality and Safety", "Infection Preventionist"),
            ("Lucas", "Taylor", "Population Health", "Ambulatory Quality Specialist"),
        };

        return people
            .Select(
                (person, index) =>
                {
                    var username =
                        index == 0
                            ? adminUsername
                            : $"{person.Item1}.{person.Item2}".ToLowerInvariant();
                    return new User
                    {
                        Username = username,
                        EmployeeId = $"E{1000 + index}",
                        AccountName = username,
                        DisplayName = $"{person.Item1} {person.Item2}",
                        FullName = $"{person.Item1} {person.Item2}",
                        FirstName = person.Item1,
                        LastName = person.Item2,
                        Department = person.Item3,
                        Title = person.Item4,
                        Phone = $"555-010-{index + 1:00}",
                        Email = $"{username}@example.test",
                        Base = "Central Hospital",
                        LastLoadDate = now,
                        LastLogin = now.AddDays(-index),
                        FullnameCalc = $"{person.Item2}, {person.Item1}",
                        FirstnameCalc = person.Item1,
                    };
                }
            )
            .ToList();
    }

    private static List<UserGroup> CreateGroups()
    {
        return
        [
            new UserGroup
            {
                AccountName = "grp-enterprise-analytics",
                GroupName = "Enterprise Analytics",
                GroupEmail = "enterprise-analytics@example.test",
                GroupType = "Department",
                GroupSource = "Demo",
            },
            new UserGroup
            {
                AccountName = "grp-clinical-operations",
                GroupName = "Clinical Operations",
                GroupEmail = "clinical-operations@example.test",
                GroupType = "Department",
                GroupSource = "Demo",
            },
            new UserGroup
            {
                AccountName = "grp-revenue-cycle",
                GroupName = "Revenue Cycle Analytics",
                GroupEmail = "revenue-cycle@example.test",
                GroupType = "Department",
                GroupSource = "Demo",
            },
            new UserGroup
            {
                AccountName = "grp-data-governance",
                GroupName = "Data Governance Council",
                GroupEmail = "data-governance@example.test",
                GroupType = "Committee",
                GroupSource = "Demo",
            },
        ];
    }

    private static List<Initiative> CreateInitiatives(IReadOnlyList<User> users, DateTime now)
    {
        return
        [
            new Initiative
            {
                Name = "Improve Patient Flow",
                Description =
                    "Reduce avoidable delays from emergency arrival through inpatient discharge.",
                OperationOwner = users[2],
                ExecutiveOwner = users[1],
                LastUpdateUserNavigation = users[8],
                LastUpdateDate = now.AddDays(-5),
                Hidden = "N",
            },
            new Initiative
            {
                Name = "Strengthen Clinical Outcomes",
                Description =
                    "Improve reliability of evidence-based care and prevent avoidable harm.",
                OperationOwner = users[4],
                ExecutiveOwner = users[1],
                LastUpdateUserNavigation = users[10],
                LastUpdateDate = now.AddDays(-9),
                Hidden = "N",
            },
            new Initiative
            {
                Name = "Optimize Financial Performance",
                Description =
                    "Improve revenue integrity, cash acceleration, and operational productivity.",
                OperationOwner = users[3],
                ExecutiveOwner = users[6],
                LastUpdateUserNavigation = users[9],
                LastUpdateDate = now.AddDays(-12),
                Hidden = "N",
            },
            new Initiative
            {
                Name = "Advance Population Health",
                Description =
                    "Close preventive and chronic-care gaps for attributed populations.",
                OperationOwner = users[5],
                ExecutiveOwner = users[1],
                LastUpdateUserNavigation = users[11],
                LastUpdateDate = now.AddDays(-7),
                Hidden = "N",
            },
        ];
    }

    private static List<Collection> CreateCollections(
        IReadOnlyList<Initiative> initiatives,
        IReadOnlyList<User> users,
        DateTime now
    )
    {
        var definitions = new[]
        {
            ("Patient Flow Command Center", "Operational view of demand, capacity, and throughput."),
            ("Quality and Safety Scorecards", "Trusted measures for clinical quality and patient safety."),
            ("Revenue Cycle Performance", "Financial clearance, billing, denials, and collections performance."),
            ("Population Health Registries", "Care-gap and outcome views for attributed populations."),
            ("Executive Monthly Review", "Enterprise measures reviewed by senior leadership each month."),
            ("Nursing Operations", "Staffing, productivity, patient flow, and nursing-sensitive outcomes."),
            ("Ambulatory Access", "Access, panel capacity, referrals, and visit demand."),
            ("Finance and Productivity", "Revenue, expense, labor, and productivity management."),
        };

        return definitions
            .Select(
                (definition, index) =>
                    new Collection
                    {
                        Initiative = initiatives[index % initiatives.Count],
                        Name = definition.Item1,
                        Purpose = definition.Item2,
                        Description =
                            $"{definition.Item2} Content is curated and reviewed by the responsible analytics team.",
                        OperationOwner = users[(index + 2) % users.Count],
                        ExecutiveOwner = users[1],
                        AnalyticsOwner = users[8],
                        DataManager = users[(index + 4) % users.Count],
                        LastUpdateDate = now.AddDays(-(index + 1)),
                        LastUpdateUserNavigation = users[8],
                        Hidden = "N",
                    }
            )
            .ToList();
    }

    private static List<Term> CreateTerms(IReadOnlyList<User> users, DateTime now)
    {
        return TermDefinitions
            .Select(
                (definition, index) =>
                    new Term
                    {
                        Name = definition.Name,
                        Summary = definition.Summary,
                        TechnicalDefinition =
                            $"{definition.Summary} Calculated using governed source data and the approved enterprise measure specification.",
                        ApprovedYn = index % 5 == 0 ? "N" : "Y",
                        ApprovalDateTime = index % 5 == 0 ? null : now.AddMonths(-3).AddDays(index),
                        ApprovedByUser = index % 5 == 0 ? null : users[1],
                        HasExternalStandardYn = index % 3 == 0 ? "Y" : "N",
                        ExternalStandardUrl =
                            index % 3 == 0 ? "https://www.cms.gov/medicare/quality" : null,
                        ValidFromDateTime = now.AddYears(-1),
                        UpdatedByUser = users[index % users.Count],
                        LastUpdatedDateTime = now.AddDays(-(index + 2)),
                    }
            )
            .ToList();
    }

    private static List<ReportObject> CreateReports(
        IReadOnlyList<User> users,
        IReadOnlyList<ReportObjectType> reportTypes,
        DateTime now
    )
    {
        return ReportDefinitions
            .Select(
                (definition, index) =>
                    new ReportObject
                    {
                        ReportObjectBizKey = $"DEMO-REPORT-{index + 1:000}",
                        SourceServer = index % 2 == 0 ? "analytics-sql" : "enterprise-bi",
                        SourceDb = index < 10 ? "clinical" : index < 18 ? "finance" : "population",
                        SourceTable = $"report_{index + 1:000}",
                        Name = definition.Name,
                        DisplayTitle = definition.Name,
                        Description = definition.Description,
                        DetailedDescription =
                            $"{definition.Description} Includes governed definitions, drill-through details, and refresh metadata.",
                        ReportObjectType = reportTypes[index % reportTypes.Count],
                        AuthorUser = users[(index + 1) % users.Count],
                        LastModifiedByUser = users[index % users.Count],
                        LastModifiedDate = now.AddDays(-(index + 1)),
                        ReportObjectUrl = $"https://reports.example.test/demo/{index + 1}",
                        DefaultVisibilityYn = "Y",
                        OrphanedReportObjectYn = index == ReportDefinitions.Length - 1 ? "Y" : "N",
                        ReportServerPath = $"/Atlas Demo/{Departments[index % Departments.Length]}",
                        RepositoryDescription = "Curated demo content for migration testing.",
                        Availability = index % 7 == 0 ? "Business Hours" : "Always",
                        Runs = 0,
                        LastLoadDate = now,
                    }
            )
            .ToList();
    }

    private void CreateRunHistory(
        IReadOnlyList<ReportObject> reports,
        IReadOnlyList<User> users,
        DateTime now
    )
    {
        for (var index = 0; index < 72; index++)
        {
            var started = now.AddHours(-(index * 6 + 1));
            var report = reports[index % reports.Count];
            var run = new ReportObjectRunData
            {
                RunDataId = $"DEMO-RUN-{index + 1:0000}",
                RunUser = users[index % users.Count],
                RunStartTime = started,
                RunDurationSeconds = 12 + (index * 17) % 480,
                RunStatus = index % 11 == 0 ? "Failed" : "Success",
                LastLoadDate = now,
                RunStartTime_Hour = new DateTime(
                    started.Year,
                    started.Month,
                    started.Day,
                    started.Hour,
                    0,
                    0,
                    DateTimeKind.Utc
                ),
                RunStartTime_Day = started.Date,
                RunStartTime_Month = new DateTime(started.Year, started.Month, 1),
                RunStartTime_Year = new DateTime(started.Year, 1, 1),
            };
            _context.ReportObjectRunDatas.Add(run);
            _context.ReportObjectRunDataBridges.Add(
                new ReportObjectRunDataBridge
                {
                    ReportObject = report,
                    RunData = run,
                    Runs = 1,
                    Inherited = 0,
                }
            );
            report.Runs = (report.Runs ?? 0) + 1;
        }
    }
}
