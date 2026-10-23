using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class RecentUpdatesApiTests
{
    [Fact]
    public async Task GetRecentReports_ReturnsTheTenNewestUpdates()
    {
        await using var context = CreateContext();
        for (var i = 1; i <= 11; i++)
        {
            context.ReportObjects.Add(
                new ReportObject
                {
                    ReportObjectId = i,
                    Name = "Report_" + i,
                    SourceServer = "OLAP01",
                    SourceDb = "Catalog",
                    SourceTable = "Cube",
                }
            );
            context.ReportObjectDocs.Add(
                new ReportObjectDoc
                {
                    ReportObjectId = i,
                    LastUpdateDateTime = new DateTime(2026, 1, i),
                }
            );
        }

        await context.SaveChangesAsync();
        var service = new ReportsApiService(context, null, null, new ConfigurationBuilder().Build(), new MemoryCache(new MemoryCacheOptions()), null, null);

        var recent = await service.GetRecentReportsAsync(CancellationToken.None);

        Assert.Equal(10, recent.Count);
        Assert.Equal(11, recent[0].Id);
        Assert.Equal("Report_11", recent[0].Name);
        Assert.Equal(2, recent[^1].Id);
    }

    [Fact]
    public async Task GetRecentInitiatives_ReturnsTheTenNewestUpdates()
    {
        await using var context = CreateContext();
        for (var i = 1; i <= 11; i++)
        {
            context.Initiatives.Add(
                new Initiative
                {
                    InitiativeId = i,
                    Name = "Initiative_" + i,
                    LastUpdateDate = new DateTime(2026, 2, i),
                }
            );
        }

        await context.SaveChangesAsync();
        var service = new InitiativesApiService(
            context,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );

        var recent = await service.GetRecentInitiativesAsync(CancellationToken.None);

        Assert.Equal(10, recent.Count);
        Assert.Equal("Initiative_11", recent[0].Name);
        Assert.DoesNotContain(recent, x => x.Id == 1);
    }

    [Fact]
    public async Task GetRecentCollections_ReturnsTheTenNewestUpdates()
    {
        await using var context = CreateContext();
        for (var i = 1; i <= 11; i++)
        {
            context.Collections.Add(
                new Collection
                {
                    CollectionId = i,
                    Name = "Collection_" + i,
                    LastUpdateDate = new DateTime(2026, 3, i),
                }
            );
        }

        await context.SaveChangesAsync();
        var service = new CollectionsApiService(
            context,
            null,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );

        var recent = await service.GetRecentCollectionsAsync(CancellationToken.None);

        Assert.Equal(10, recent.Count);
        Assert.Equal(11, recent[0].Id);
        Assert.Equal("Collection_11", recent[0].Name);
        Assert.DoesNotContain(recent, x => x.Id == 1);
    }

    private static Atlas_WebContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new Atlas_WebContext(options);
    }
}
