using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Analytics;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class AnalyticsApiControllerTests
{
    [Fact]
    public void ExposesAllBearerProtectedAnalyticsEndpoints()
    {
        var controllerType = typeof(AnalyticsApiController);

        Assert.Equal("api/analytics", controllerType.GetCustomAttribute<RouteAttribute>()!.Template);
        Assert.Contains(
            controllerType.GetCustomAttributes<AuthorizeAttribute>(),
            attribute => attribute.AuthenticationSchemes == "Bearer"
        );

        var expectedMethods = new[]
        {
            "GetLiveUsers",
            "RecordBeacon",
            "GetVisits",
            "GetBrowsers",
            "GetOs",
            "GetResolution",
            "GetUsers",
            "GetLoadTimes",
            "GetTraces",
            "RecordTraces",
            "ResolveTrace",
            "GetErrors",
            "ResolveError",
        };

        foreach (var methodName in expectedMethods)
        {
            Assert.NotNull(controllerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public));
        }
    }

    [Fact]
    public async Task EmptyDatabaseReturnsEmptyAnalyticsBuckets()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        var service = new AnalyticsApiService(context, new ConfigurationBuilder().Build());

        var visits = await service.GetVisitsAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var browsers = await service.GetBrowsersAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var os = await service.GetOsAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var resolutions = await service.GetResolutionAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var users = await service.GetUsersAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var loadTimes = await service.GetLoadTimesAsync(new AnalyticsQueryRequest(), CancellationToken.None);
        var liveUsers = await service.GetLiveUsersAsync(CancellationToken.None);
        var traces = await service.GetTracesAsync(new AnalyticsLogQueryRequest(), CancellationToken.None);
        var errors = await service.GetErrorsAsync(new AnalyticsLogQueryRequest(), CancellationToken.None);

        Assert.Empty(visits.AccessHistory);
        Assert.Empty(browsers);
        Assert.Empty(os);
        Assert.Empty(resolutions);
        Assert.Empty(users);
        Assert.Empty(loadTimes);
        Assert.Empty(liveUsers.Items);
        Assert.Empty(traces.Items);
        Assert.Empty(errors.Items);
        Assert.Equal(0, visits.Views);
        Assert.Equal(0, traces.TotalCount);
        Assert.Equal(0, errors.TotalCount);
    }

    [Fact]
    public async Task RecordBeaconCreatesThenUpdatesTheSameSessionPage()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        var service = new AnalyticsApiService(context, new ConfigurationBuilder().Build());
        var request = new AnalyticsBeaconRequest
        {
            SessionId = "session-1",
            PageId = "page-1",
            Href = "/reports?id=1",
            PageTime = 100,
        };

        await service.RecordBeaconAsync(7, false, request, CancellationToken.None);
        await service.RecordBeaconAsync(
            7,
            false,
            new AnalyticsBeaconRequest
            {
                SessionId = "session-1",
                PageId = "page-1",
                Href = "/reports?id=1",
                PageTime = 250,
            },
            CancellationToken.None
        );

        var analytic = Assert.Single(context.Analytics);
        Assert.Equal(250, analytic.PageTime);
        Assert.Equal(7, analytic.UserId);
    }

    [Fact]
    public async Task TraceIngestionAndResolutionPreserveHandledState()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        var service = new AnalyticsApiService(context, new ConfigurationBuilder().Build());

        await service.RecordTracesAsync(
            7,
            "test-agent",
            "https://referrer.example",
            new AnalyticsTraceIngestRequest
            {
                Logs = new[]
                {
                    new AnalyticsTraceEntryRequest { Level = 5000, Message = "failure", Logger = "test" },
                },
            },
            CancellationToken.None
        );

        var trace = Assert.Single(context.AnalyticsTraces);
        await service.ResolveTraceAsync(trace.Id, 1, CancellationToken.None);
        Assert.Equal(1, (await context.AnalyticsTraces.SingleAsync()).Handled);

        await service.ResolveTraceAsync(trace.Id, 0, CancellationToken.None);
        Assert.Null((await context.AnalyticsTraces.SingleAsync()).Handled);
    }

    [Fact]
    public async Task ErrorListReportsPaginationAndUnresolvedCount()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.AnalyticsErrors.AddRange(
            new AnalyticsError { Id = 1, UserId = 7, UserAgent = "agent", LogDateTime = DateTime.Now, Handled = null },
            new AnalyticsError { Id = 2, UserId = 7, UserAgent = "agent", LogDateTime = DateTime.Now.AddMinutes(-1), Handled = 1 }
        );
        await context.SaveChangesAsync();
        Assert.Equal(2, await context.AnalyticsErrors.CountAsync());
        Assert.Equal(
            2,
            await context.AnalyticsErrors
                .Where(x => x.UserAgent != null && x.LogDateTime >= DateTime.Now.AddDays(-1) && x.LogDateTime <= DateTime.Now)
                .CountAsync()
        );
        var service = new AnalyticsApiService(context, new ConfigurationBuilder().Build());

        var result = await service.GetErrorsAsync(
            new AnalyticsLogQueryRequest { StartAt = -86400, EndAt = 0, Page = 0 },
            CancellationToken.None
        );

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.UnresolvedCount);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(2, result.Items.Count);
    }
}
