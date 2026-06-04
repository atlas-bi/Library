using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Groups;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class GroupsApiControllerTests
{
    [Fact]
    public async Task GetGroups_ReturnsSortedListSurfaceWithPermissionsAndFlags()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "groups-api-list")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.UserGroups.AddRange(
            new UserGroup
            {
                GroupId = 2,
                GroupName = "Zeta Team",
                GroupEmail = "zeta@example.com",
                GroupType = "Security",
                GroupSource = "LDAP",
            },
            new UserGroup
            {
                GroupId = 1,
                GroupName = "Alpha Team",
                GroupEmail = "alpha@example.com",
                GroupType = "Department",
                GroupSource = "LDAP",
            }
        );
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["features:enable_user_profile"] = "true",
                }
            )
            .Build();

        var service = new GroupsApiService(context, config, new MemoryCache(new MemoryCacheOptions()));
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "View Groups" })
        );

        var result = await controller.GetGroups();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<GroupListResponseDto>(ok.Value);

        Assert.True(payload.Permissions.CanViewGroups);
        Assert.True(payload.Features.UserProfilesEnabled);
        Assert.Equal(new[] { 1, 2 }, payload.Items.Select(x => x.Id).ToArray());
        Assert.Equal("/groups?id=1", payload.Items[0].Url);
    }

    [Fact]
    public async Task GetGroup_ReturnsDetailWithViewerDrivenFlags()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "groups-api-detail")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.UserGroups.Add(
            new UserGroup
            {
                GroupId = 10,
                GroupName = "Revenue Team",
                GroupEmail = "revenue@example.com",
                GroupType = "Department",
                GroupSource = "LDAP",
            }
        );
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["features:enable_user_profile"] = "true",
                }
            )
            .Build();

        var service = new GroupsApiService(context, config, new MemoryCache(new MemoryCacheOptions()));
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 1,
                username: "viewer",
                permissions: new[] { "View Groups", "View Other User", "View Site Analytics" }
            )
        );

        var result = await controller.GetGroup(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<GroupDetailDto>(ok.Value);

        Assert.Equal(10, payload.Id);
        Assert.Equal("Revenue Team", payload.Name);
        Assert.Equal("revenue@example.com", payload.Email);
        Assert.True(payload.Permissions.CanViewGroups);
        Assert.True(payload.Permissions.CanViewUserProfiles);
        Assert.True(payload.Permissions.CanViewSiteAnalytics);
        Assert.True(payload.Features.UserProfilesEnabled);
    }

    [Fact]
    public async Task GetGroupUsers_ReturnsMembers()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "groups-api-users")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.UserGroups.Add(
            new UserGroup { GroupId = 20, GroupName = "Finance Team", GroupType = "Department" }
        );
        context.Users.AddRange(
            new User
            {
                UserId = 1,
                Username = "analyst",
                FullnameCalc = "Analyst Name",
                Email = "analyst@example.com",
                Phone = "555-1000",
                EpicId = "E100",
                EmployeeId = "EMP100",
            },
            new User
            {
                UserId = 2,
                Username = "manager",
                FullnameCalc = "Manager Name",
                Email = "manager@example.com",
                Phone = "555-2000",
                EpicId = "E200",
                EmployeeId = "EMP200",
            }
        );
        context.UserGroupsMemberships.AddRange(
            new UserGroupsMembership { MembershipId = 1, GroupId = 20, UserId = 2 },
            new UserGroupsMembership { MembershipId = 2, GroupId = 20, UserId = 1 }
        );
        await context.SaveChangesAsync();

        var service = new GroupsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 9,
                username: "viewer",
                permissions: new[] { "View Groups", "View Other User" }
            )
        );

        var result = await controller.GetGroupUsers(20);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<GroupUserDto>>(ok.Value);

        Assert.Equal(2, payload.Count);
        Assert.Equal(new[] { 1, 2 }, payload.Select(x => x.Id).ToArray());
        Assert.All(payload, item => Assert.True(item.CanOpenUserProfile));
        Assert.Equal("/users?id=1", payload[0].Url);
    }

    [Fact]
    public async Task GetGroupReports_ReturnsReportSummaryRows()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "groups-api-reports")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.UserGroups.Add(
            new UserGroup { GroupId = 30, GroupName = "Clinical Team", GroupType = "Department" }
        );
        context.ReportObjectTypes.Add(
            new ReportObjectType
            {
                ReportObjectTypeId = 5,
                Name = "SSRS Report",
                ShortName = "SSRS",
                Visible = "Y",
            }
        );
        context.ReportObjects.AddRange(
            new ReportObject
            {
                ReportObjectId = 100,
                Name = "Revenue",
                DisplayTitle = "Revenue Snapshot",
                ReportObjectTypeId = 5,
                LastModifiedDate = new System.DateTime(2026, 6, 1),
                SourceDb = "warehouse",
                SourceServer = "reports",
                SourceTable = "dbo.revenue",
            },
            new ReportObject
            {
                ReportObjectId = 101,
                Name = "Census",
                DisplayTitle = "Daily Census",
                ReportObjectTypeId = 5,
                LastModifiedDate = new System.DateTime(2026, 6, 2),
                SourceDb = "warehouse",
                SourceServer = "reports",
                SourceTable = "dbo.census",
            }
        );
        context.ReportGroupsMemberships.AddRange(
            new ReportGroupsMembership { MembershipId = 1, GroupId = 30, ReportId = 101 },
            new ReportGroupsMembership { MembershipId = 2, GroupId = 30, ReportId = 100 }
        );
        context.ReportObjectSubscriptions.AddRange(
            new ReportObjectSubscription { SubscriptionId = "1", ReportObjectId = 100 },
            new ReportObjectSubscription { SubscriptionId = "2", ReportObjectId = 100 },
            new ReportObjectSubscription { SubscriptionId = "3", ReportObjectId = 101 }
        );
        context.ReportObjectRunDataBridges.AddRange(
            new ReportObjectRunDataBridge { BridgeId = 1, ReportObjectId = 100, Runs = 9 },
            new ReportObjectRunDataBridge { BridgeId = 2, ReportObjectId = 101, Runs = 4 }
        );
        context.StarredReports.AddRange(
            new StarredReport { StarId = 1, Ownerid = 10, Reportid = 100 },
            new StarredReport { StarId = 2, Ownerid = 11, Reportid = 100 },
            new StarredReport { StarId = 3, Ownerid = 12, Reportid = 101 }
        );
        await context.SaveChangesAsync();

        var service = new GroupsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 9, username: "viewer", permissions: new[] { "View Groups" })
        );

        var result = await controller.GetGroupReports(30);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<GroupReportDto>>(ok.Value);

        Assert.Equal(2, payload.Count);
        Assert.Equal(new[] { 101, 100 }, payload.Select(x => x.Id).ToArray());
        Assert.Equal("/reports?id=101", payload[0].Url);
        Assert.Equal(1, payload[0].SubscriptionCount);
        Assert.Equal(1, payload[0].FavoriteCount);
        Assert.Equal(4, payload[0].RunCount);
        Assert.Equal(2, payload[1].SubscriptionCount);
        Assert.Equal(2, payload[1].FavoriteCount);
        Assert.Equal(9, payload[1].RunCount);
    }

    [Fact]
    public async Task GroupEndpoints_ReturnNotFoundForMissingGroup()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "groups-api-missing")
            .Options;

        await using var context = new Atlas_WebContext(options);
        var service = new GroupsApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "View Groups" })
        );

        var detail = await controller.GetGroup(404);
        var users = await controller.GetGroupUsers(404);
        var reports = await controller.GetGroupReports(404);

        Assert.IsType<NotFoundResult>(detail.Result);
        Assert.IsType<NotFoundResult>(users.Result);
        Assert.IsType<NotFoundResult>(reports.Result);
    }

    private static GroupsApiController BuildController(
        IGroupsApiService service,
        ClaimsPrincipal principal
    )
    {
        return new GroupsApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal },
            },
        };
    }

    private static ClaimsPrincipal BuildPrincipal(
        int userId,
        string username,
        IEnumerable<string> permissions = null
    )
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("UserId", userId.ToString()),
            new("Fullname", username),
            new("AdminEnabled", "Y"),
        };

        if (permissions != null)
        {
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
