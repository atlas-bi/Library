using System;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Services.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace web.Tests.FunctionTests.Services;

public class DemoDataSeederTests
{
    [Fact]
    public async Task SeedAsync_CreatesConfiguredAdminAndVersionMarker()
    {
        await using var context = CreateContext();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "local-admin");

        var admin = await context.Users.SingleAsync(user => user.Username == "local-admin");
        var marker = await context.GlobalSiteSettings.SingleAsync(
            setting => setting.Name == DemoDataSeeder.SeedMarkerName
        );

        Assert.Equal("Local Admin", admin.FullName);
        Assert.Equal("test-v1", marker.Value);
    }

    [Fact]
    public async Task SeedAsync_PopulatesUserVisibleFeatures()
    {
        await using var context = CreateContext();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "local-admin");

        Assert.True(await context.Users.CountAsync() >= 12);
        Assert.Equal(
            new DateTime(2026, 8, 12, 9, 0, 0, DateTimeKind.Utc),
            await context
                .Users.Where(user => user.Username == "local-admin")
                .Select(user => user.LastLoadDate)
                .SingleAsync()
        );
        Assert.Contains(context.UserRoles, role => role.Name == "Administrator");
        Assert.Contains(context.UserRoles, role => role.Name == "User");
        Assert.True(await context.UserGroups.CountAsync() >= 4);
        Assert.True(await context.ReportObjects.CountAsync() >= 25);
        Assert.True(await context.Terms.CountAsync() >= 15);
        Assert.True(await context.Collections.CountAsync() >= 6);
        Assert.True(await context.Initiatives.CountAsync() >= 3);
        var reportDocs = await context.ReportObjectDocs.ToListAsync();
        Assert.NotEmpty(reportDocs);
        Assert.All(reportDocs, doc => Assert.True(doc.ReportObjectId > 0));
        Assert.NotEmpty(await context.CollectionReports.ToListAsync());
        Assert.NotEmpty(await context.CollectionTerms.ToListAsync());
        Assert.NotEmpty(await context.ReportObjectRunDatas.ToListAsync());
        var runBridges = await context.ReportObjectRunDataBridges.ToListAsync();
        Assert.NotEmpty(runBridges);
        Assert.All(
            runBridges,
            bridge =>
            {
                Assert.True(bridge.ReportObjectId > 0);
                Assert.False(string.IsNullOrWhiteSpace(bridge.RunId));
            }
        );
        var maintenanceLogs = await context.MaintenanceLogs.ToListAsync();
        Assert.NotEmpty(maintenanceLogs);
        Assert.All(maintenanceLogs, log => Assert.True(log.ReportId > 0));
        Assert.NotEmpty(await context.ReportObjectDocTerms.ToListAsync());
        Assert.NotEmpty(await context.ReportObjectSubscriptions.ToListAsync());
        var sharedItems = await context.SharedItems.ToListAsync();
        Assert.NotEmpty(sharedItems);
        Assert.All(sharedItems, item => Assert.StartsWith("/reports?id=", item.Url));
        Assert.NotEmpty(await context.StarredReports.ToListAsync());
        Assert.NotEmpty(await context.StarredCollections.ToListAsync());
        Assert.NotEmpty(await context.StarredInitiatives.ToListAsync());
        Assert.NotEmpty(await context.StarredTerms.ToListAsync());
        Assert.NotEmpty(await context.StarredGroups.ToListAsync());
        Assert.NotEmpty(await context.UserFavoriteFolders.ToListAsync());
    }

    [Fact]
    public async Task SeedAsync_WithAppliedVersion_DoesNotDuplicateData()
    {
        await using var context = CreateContext();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "local-admin");
        var userCount = await context.Users.CountAsync();
        var reportCount = await context.ReportObjects.CountAsync();

        await seeder.SeedAsync("test-v1", "local-admin");

        Assert.Equal(userCount, await context.Users.CountAsync());
        Assert.Equal(reportCount, await context.ReportObjects.CountAsync());
        Assert.Single(
            await context
                .GlobalSiteSettings.Where(setting =>
                    setting.Name == DemoDataSeeder.SeedMarkerName
                )
                .ToListAsync()
        );
    }

    [Fact]
    public async Task SeedAsync_WithNewVersion_ReplacesExistingData()
    {
        await using var context = CreateContext();
        context.Users.Add(new User { Username = "obsolete-user" });
        context.GlobalSiteSettings.Add(
            new GlobalSiteSetting
            {
                Name = DemoDataSeeder.SeedMarkerName,
                Description = "Old demo seed",
                Value = "old-version",
            }
        );
        await context.SaveChangesAsync();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "local-admin", resetExistingData: true);

        Assert.DoesNotContain(context.Users, user => user.Username == "obsolete-user");
        Assert.Equal(
            "test-v1",
            await context
                .GlobalSiteSettings.Where(setting =>
                    setting.Name == DemoDataSeeder.SeedMarkerName
                )
                .Select(setting => setting.Value)
                .SingleAsync()
        );
    }

    [Fact]
    public async Task SeedAsync_WithExistingDataAndResetDisabled_Throws()
    {
        await using var context = CreateContext();
        context.Users.Add(new User { Username = "existing-user" });
        await context.SaveChangesAsync();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => seeder.SeedAsync("test-v1", "local-admin")
        );

        Assert.Contains("DEMO_SEED_RESET", exception.Message);
        Assert.Contains(context.Users, user => user.Username == "existing-user");
    }

    [Fact]
    public async Task SeedAsync_WithExistingRelatedDataAndResetDisabled_Throws()
    {
        await using var context = CreateContext();
        context.UserGroups.Add(new UserGroup { GroupName = "Existing group" });
        await context.SaveChangesAsync();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => seeder.SeedAsync("test-v1", "local-admin")
        );

        Assert.Contains("DEMO_SEED_RESET", exception.Message);
    }

    [Fact]
    public async Task SeedAsync_ReusesReferenceDataCreatedByMigrations()
    {
        await using var context = CreateContext();
        context.UserRoles.AddRange(
            new UserRole { Name = "Administrator" },
            new UserRole { Name = "Report Writer" },
            new UserRole { Name = "User" }
        );
        context.ReportObjectTypes.AddRange(
            new ReportObjectType { Name = "SSRS Report" },
            new ReportObjectType { Name = "Tableau Workbook" }
        );
        context.MaintenanceSchedules.AddRange(
            new MaintenanceSchedule { Name = "Quarterly" },
            new MaintenanceSchedule { Name = "Yearly" },
            new MaintenanceSchedule { Name = "Audit Only" }
        );
        context.MaintenanceLogStatuses.AddRange(
            new MaintenanceLogStatus { Name = "Approved - No Changes" },
            new MaintenanceLogStatus { Name = "Approved - With Changes" },
            new MaintenanceLogStatus { Name = "Recommend Retire" }
        );
        await context.SaveChangesAsync();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "local-admin");

        Assert.Single(context.UserRoles.Where(role => role.Name == "Administrator"));
        Assert.Single(context.UserRoles.Where(role => role.Name == "User"));
        Assert.Single(context.ReportObjectTypes.Where(type => type.Name == "SSRS Report"));
        Assert.Single(context.ReportObjectTypes.Where(type => type.Name == "Tableau Workbook"));
        Assert.Equal(
            "SSRS",
            context.ReportObjectTypes.Single(type => type.Name == "SSRS Report").ShortName
        );
        Assert.Equal(
            "Tableau",
            context.ReportObjectTypes.Single(type => type.Name == "Tableau Workbook").ShortName
        );
        Assert.Equal(3, await context.MaintenanceSchedules.CountAsync());
        Assert.Equal(3, await context.MaintenanceLogStatuses.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_NormalizesBlankAdminUsernameLikeDemoAuthentication()
    {
        await using var context = CreateContext();
        var seeder = new DemoDataSeeder(context, NullLogger<DemoDataSeeder>.Instance);

        await seeder.SeedAsync("test-v1", "   ");

        Assert.Contains(context.Users, user => user.Username == "Default");
        Assert.DoesNotContain(context.Users, user => string.IsNullOrWhiteSpace(user.Username));
    }

    private static Atlas_WebContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new Atlas_WebContext(options);
    }
}
