using System;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Profile;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Services;

public class ProfileApiServiceTests
{
    [Fact]
    public async Task Analytics_SqlServer_ReturnsSeededRunData()
    {
        var connectionString = System.Environment.GetEnvironmentVariable("ATLAS_TEST_SQLSERVER");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        var connection = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "AtlasProfileApiRegression",
        };
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseSqlServer(connection.ConnectionString)
            .Options;
        await using var context = new Atlas_WebContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var now = DateTime.Now.AddHours(-1);
        var user = new User { Username = "runner", FullnameCalc = "Run User" };
        var run = new ReportObjectRunData
        {
            RunDataId = "PROFILE-RUN-1",
            RunUser = user,
            RunStartTime = now,
            RunStartTime_Hour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0),
            RunStartTime_Day = now.Date,
            RunStartTime_Month = new DateTime(now.Year, now.Month, 1),
            RunStartTime_Year = new DateTime(now.Year, 1, 1),
            RunDurationSeconds = 42,
            RunStatus = "Success",
            LastLoadDate = now,
        };
        context.ReportObjectRunDatas.Add(run);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO dbo.ReportObject
                (ReportObjectBizKey, SourceServer, SourceDB, SourceTable, Name)
            VALUES
                ('PROFILE-REPORT-1', 'TestServer', 'TestDatabase', 'TestTable', 'Patient Flow')
            """
        );
        var report = await context.ReportObjects.SingleAsync(x => x.Name == "Patient Flow");
        context.ReportObjectRunDataBridges.Add(new ReportObjectRunDataBridge
        {
            ReportObject = report,
            RunData = run,
            Runs = 3,
        });
        await context.SaveChangesAsync();

        var service = new ProfileApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build()
        );

        var request = new ProfileQueryRequestDto
        {
            Id = user.UserId,
            Type = "user",
            StartAt = -86400,
        };

        var chart = await service.GetChartAsync(request, default);
        var users = await service.GetUsersAsync(request, default);
        var reports = await service.GetReportsAsync(request, default);
        var fails = await service.GetFailsAsync(request, default);

        Assert.Equal(3, chart.Runs);
        Assert.Equal(1, chart.Users);
        Assert.Equal(42, chart.RunTime);
        Assert.Single(chart.History);
        Assert.Equal("Run User", Assert.Single(users).Key);
        Assert.Equal("Patient Flow", Assert.Single(reports).Key);
        Assert.Empty(fails);
    }
}
