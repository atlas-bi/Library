using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class TasksApiControllerTests
{
    [Fact]
    public void GetTasks_ExposesAuthenticatedCollectionEndpoint()
    {
        var method = typeof(TasksApiController).GetMethod(
            "GetTasks",
            BindingFlags.Instance | BindingFlags.Public
        );

        Assert.NotNull(method);
        Assert.NotNull(method.GetCustomAttributes<HttpGetAttribute>().Single());
        Assert.Equal(
            "api/tasks",
            typeof(TasksApiController).GetCustomAttributes<RouteAttribute>().Single().Template
        );
        Assert.Contains(
            typeof(TasksApiController).GetCustomAttributes<AuthorizeAttribute>(),
            attribute => attribute.AuthenticationSchemes == "Bearer"
        );
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyBucketsForEmptyDatabase()
    {
        await using var context = CreateContext("tasks-empty");
        var result = await new TasksApiService(context).GetTasksAsync(CancellationToken.None);

        Assert.Empty(result.CanMakeReports);
        Assert.Empty(result.RecommendRetire);
        Assert.Empty(result.Unused);
        Assert.Empty(result.MaintenanceRequired);
        Assert.Empty(result.Audit);
        Assert.Empty(result.MissingSchedule);
        Assert.Empty(result.NotInAnalytics);
        Assert.Empty(result.TopUndocumented);
        Assert.Empty(result.NewUndocumented);
    }

    [Fact]
    public async Task GetTasks_PlacesSeededRowsInExpectedBuckets()
    {
        await using var context = CreateContext("tasks-seeded");
        var lastModified = DateTime.Now.AddMonths(-4);
        var maintenanceDate = DateTime.Now.AddMonths(-3);

        var writer = new User
        {
            UserId = 1,
            Username = "writer",
            FullnameCalc = "Writer Name",
        };
        var group = new UserGroup
        {
            GroupId = 10,
            GroupName = "Report Authors",
            EpicId = "100623",
        };
        var reportType = new ReportObjectType
        {
            ReportObjectTypeId = 3,
            Name = "SSRS Report",
        };
        var unused = CreateReport(1, reportType, lastModified, "Unused Census");
        var retire = CreateReport(2, reportType, lastModified, "Retire Census");
        var undocumented = CreateReport(3, reportType, DateTime.Now.AddDays(-3), "New Undocumented");
        var status = new MaintenanceLogStatus { Id = 8, Name = "Recommend Retire" };

        context.Users.Add(writer);
        context.UserGroups.Add(group);
        context.UserGroupsMemberships.Add(
            new UserGroupsMembership
            {
                MembershipId = 1,
                UserId = 1,
                GroupId = 10,
            }
        );
        context.ReportObjectTypes.Add(reportType);
        context.ReportObjects.AddRange(unused, retire, undocumented);
        context.ReportObjectDocs.Add(
            new ReportObjectDoc
            {
                ReportObjectId = 2,
                Hidden = "N",
            }
        );
        context.MaintenanceLogStatuses.Add(status);
        context.MaintenanceLogs.Add(
            new MaintenanceLog
            {
                MaintenanceLogId = 1,
                MaintainerId = 1,
                ReportId = 2,
                MaintenanceLogStatusId = 8,
                MaintenanceDate = maintenanceDate,
                Comment = "Replace with command center",
            }
        );
        var run = new ReportObjectRunData
        {
            RunId = 1,
            RunDataId = "tasks-run-1",
            RunStartTime = DateTime.Now.AddDays(-1),
            RunStartTime_Hour = DateTime.Today,
            RunStartTime_Day = DateTime.Today,
            RunStartTime_Month = DateTime.Today,
            RunStartTime_Year = DateTime.Today,
            LastLoadDate = DateTime.Now,
            RunStatus = "Success",
        };
        context.ReportObjectRunDatas.Add(run);
        context.ReportObjectRunDataBridges.Add(
            new ReportObjectRunDataBridge
            {
                BridgeId = 1,
                ReportObjectId = 2,
                RunId = "tasks-run-1",
                Runs = 4,
            }
        );
        await context.SaveChangesAsync();

        var controller = new TasksApiController(new TasksApiService(context));
        var result = await controller.GetTasks();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<TasksResponseDto>(ok.Value);

        Assert.Equal("Writer Name", Assert.Single(payload.CanMakeReports).Name);
        var retireRow = Assert.Single(payload.RecommendRetire);
        Assert.Equal("Retire Census", retireRow.Name);
        Assert.Equal(ModelHelpers.RelativeDate(maintenanceDate), retireRow.MaintenanceDateString);
        var unusedRow = Assert.Single(payload.Unused);
        Assert.Equal("Unused Census", unusedRow.Name);
        Assert.Equal("/reports?id=1", unusedRow.ReportUrl);
        Assert.Equal(ModelHelpers.RelativeDate(lastModified), unusedRow.LastModified);
        Assert.Contains(payload.NewUndocumented, x => x.Name == "New Undocumented");
        Assert.Contains(payload.TopUndocumented, x => x.Name == "New Undocumented");
    }

    [Fact]
    public async Task GetTasks_SqlServer_FormatsUnusedLastModifiedWithoutTranslationFailure()
    {
        var connectionString = Environment.GetEnvironmentVariable("ATLAS_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var connection = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "AtlasTasksApiRegression",
        };
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseSqlServer(connection.ConnectionString)
            .Options;
        await using var context = new Atlas_WebContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Database.ExecuteSqlRawAsync(
            """
            SET IDENTITY_INSERT dbo.ReportObjectType ON;
            INSERT INTO dbo.ReportObjectType (ReportObjectTypeID, Name, Visible)
            VALUES (3, N'SSRS Report', N'Y');
            SET IDENTITY_INSERT dbo.ReportObjectType OFF;
            INSERT INTO dbo.ReportObject
                (ReportObjectBizKey, SourceServer, SourceDB, SourceTable, Name, DisplayTitle,
                 ReportObjectTypeID, DefaultVisibilityYN, OrphanedReportObjectYN, LastModifiedDate)
            VALUES
                (N'TASKS-UNUSED-1', N'TestServer', N'TestDatabase', N'TestTable', N'System Name',
                 N'Unused Census', 3, N'Y', N'N', '2026-01-15');
            """
        );

        var result = await new TasksApiService(context).GetTasksAsync(CancellationToken.None);

        var unused = Assert.Single(result.Unused);
        Assert.Equal("Unused Census", unused.Name);
        Assert.False(string.IsNullOrWhiteSpace(unused.LastModified));
    }

    private static Atlas_WebContext CreateContext(string name)
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new Atlas_WebContext(options);
    }

    private static ReportObject CreateReport(
        int id,
        ReportObjectType type,
        DateTime lastModified,
        string title
    )
    {
        return new ReportObject
        {
            ReportObjectId = id,
            Name = "System " + title,
            DisplayTitle = title,
            ReportObjectTypeId = type.ReportObjectTypeId,
            ReportObjectType = type,
            DefaultVisibilityYn = "Y",
            OrphanedReportObjectYn = "N",
            LastModifiedDate = lastModified,
            SourceServer = "TestServer",
            SourceDb = "TestDatabase",
            SourceTable = "TestTable",
        };
    }
}
