using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Initiatives;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class InitiativesApiControllerTests
{
    [Fact]
    public async Task GetInitiatives_ReturnsSortedListSurfaceWithPermissionsAndFlags()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-list")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name" });
        context.Initiatives.AddRange(
            new Initiative
            {
                InitiativeId = 2,
                Name = "Zeta Initiative",
                Description = "Zeta body",
            },
            new Initiative
            {
                InitiativeId = 1,
                Name = "Alpha Initiative",
                Description = "Alpha body",
            }
        );
        context.StarredInitiatives.Add(new StarredInitiative { StarId = 1, Ownerid = 1, Initiativeid = 1 });
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(("features:enable_user_profile", "true"), ("features:enable_sharing", "true"), ("features:enable_feedback", "false")),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "viewer", permissions: new[] { "Create Initiative" })
        );

        var result = await controller.GetInitiatives();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<InitiativeListResponseDto>(ok.Value);

        Assert.True(payload.Permissions.CanCreateInitiative);
        Assert.True(payload.Features.UserProfilesEnabled);
        Assert.True(payload.Features.SharingEnabled);
        Assert.False(payload.Features.FeedbackEnabled);
        Assert.Equal(new[] { 1, 2 }, payload.Items.Select(x => x.Id).ToArray());

        var alpha = Assert.Single(payload.Items.Where(x => x.Id == 1));
        Assert.True(alpha.IsStarred);
        Assert.Equal(1, alpha.StarCount);
    }

    [Fact]
    public async Task GetInitiative_ReturnsDetailWithViewerDrivenFlagsAndLinkedCollections()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-detail")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.AddRange(
            new User { UserId = 1, Username = "viewer", FullnameCalc = "Viewer Name", Email = "viewer@example.com" },
            new User { UserId = 2, Username = "owner", FullnameCalc = "Owner Name", Email = "owner@example.com" },
            new User { UserId = 3, Username = "exec", FullnameCalc = "Executive Name", Email = "exec@example.com" }
        );
        context.FinancialImpacts.Add(new FinancialImpact { Id = 4, Name = "High" });
        context.StrategicImportances.Add(new StrategicImportance { Id = 5, Name = "Critical" });
        context.Initiatives.Add(
            new Initiative
            {
                InitiativeId = 10,
                Name = "Revenue Lift",
                Description = "Revenue body",
                OperationOwnerId = 2,
                ExecutiveOwnerId = 3,
                FinancialImpact = 4,
                StrategicImportance = 5,
                LastUpdateUser = 1,
                LastUpdateDate = new System.DateTime(2026, 7, 1),
                Hidden = "Y",
            }
        );
        context.Collections.AddRange(
            new Collection { CollectionId = 100, InitiativeId = 10, Name = "Operations", Description = "Ops body" },
            new Collection { CollectionId = 101, InitiativeId = 10, Name = "Finance", Description = "Finance body" }
        );
        context.StarredInitiatives.Add(new StarredInitiative { StarId = 5, Ownerid = 1, Initiativeid = 10 });
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(("features:enable_user_profile", "true"), ("features:enable_sharing", "true"), ("features:enable_feedback", "true")),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(
                userId: 1,
                username: "viewer",
                permissions: new[] { "Create Initiative", "Edit Initiative", "Delete Initiative", "View Other User" }
            )
        );

        var result = await controller.GetInitiative(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<InitiativeDetailDto>(ok.Value);

        Assert.Equal(10, payload.Id);
        Assert.True(payload.IsStarred);
        Assert.Equal(1, payload.StarCount);
        Assert.True(payload.CanCreateInitiative);
        Assert.True(payload.CanEditInitiative);
        Assert.True(payload.CanDeleteInitiative);
        Assert.True(payload.CanViewUserProfiles);
        Assert.Equal("Owner Name", payload.OperationOwner.FullName);
        Assert.Equal("Executive Name", payload.ExecutiveOwner.FullName);
        Assert.Equal("High", payload.FinancialImpact.Name);
        Assert.Equal("Critical", payload.StrategicImportance.Name);
        Assert.Equal(2, payload.Collections.Count);
    }

    [Fact]
    public async Task CreateInitiative_CreatesRecordAndAssignsLinkedCollections()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-create")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "creator", FullnameCalc = "Creator Name" });
        context.Collections.AddRange(
            new Collection { CollectionId = 20, Name = "Ops" },
            new Collection { CollectionId = 21, Name = "Finance" }
        );
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "creator", permissions: new[] { "Create Initiative" })
        );

        var result = await controller.CreateInitiative(
            new CreateInitiativeRequestDto
            {
                Name = "Create Me",
                Description = "Created body",
                Hidden = "Y",
                CollectionIds = new[] { 20, 21 },
            }
        );

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<InitiativeDetailDto>(created.Value);
        Assert.Equal("Create Me", payload.Name);

        var initiative = Assert.Single(context.Initiatives);
        Assert.Equal(1, initiative.LastUpdateUser);
        Assert.Equal("Y", initiative.Hidden);
        Assert.All(context.Collections.OrderBy(x => x.CollectionId), collection => Assert.Equal(initiative.InitiativeId, collection.InitiativeId));
    }

    [Fact]
    public async Task UpdateInitiative_UpdatesFieldsAndSynchronizesLinkedCollections()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-update")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(new User { UserId = 1, Username = "editor", FullnameCalc = "Editor Name" });
        context.Initiatives.Add(
            new Initiative
            {
                InitiativeId = 30,
                Name = "Original",
                Description = "Original body",
                Hidden = "N",
            }
        );
        context.Collections.AddRange(
            new Collection { CollectionId = 40, InitiativeId = 30, Name = "Keep Removed" },
            new Collection { CollectionId = 41, Name = "Add Me" }
        );
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "editor", permissions: new[] { "Edit Initiative" })
        );

        var result = await controller.UpdateInitiative(
            30,
            new UpdateInitiativeRequestDto
            {
                Name = "Updated",
                Description = "Updated body",
                Hidden = "Y",
                CollectionIds = new[] { 41 },
            }
        );

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<InitiativeDetailDto>(ok.Value);
        Assert.Equal("Updated", payload.Name);

        var initiative = Assert.Single(context.Initiatives);
        Assert.Equal("Updated", initiative.Name);
        Assert.Equal("Y", initiative.Hidden);
        Assert.Equal(1, initiative.LastUpdateUser);
        Assert.Null(context.Collections.Single(x => x.CollectionId == 40).InitiativeId);
        Assert.Equal(30, context.Collections.Single(x => x.CollectionId == 41).InitiativeId);
    }

    [Fact]
    public async Task DeleteInitiative_ClearsCollectionLinksAndRemovesRecord()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-delete")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Initiatives.Add(new Initiative { InitiativeId = 50, Name = "Delete Me" });
        context.Collections.AddRange(
            new Collection { CollectionId = 60, InitiativeId = 50, Name = "Linked A" },
            new Collection { CollectionId = 61, InitiativeId = 50, Name = "Linked B" }
        );
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(
            service,
            BuildPrincipal(userId: 1, username: "deleter", permissions: new[] { "Delete Initiative" })
        );

        var result = await controller.DeleteInitiative(50);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Initiatives);
        Assert.All(context.Collections, collection => Assert.Null(collection.InitiativeId));
    }

    [Fact]
    public async Task SearchCollections_ReturnsAvailableCollectionMatches()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "initiatives-api-search-collections")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Collections.AddRange(
            new Collection { CollectionId = 70, Name = "Financial Planning", Description = "Alpha body" },
            new Collection { CollectionId = 71, Name = "Clinical Quality", Description = "Beta body" }
        );
        await context.SaveChangesAsync();

        var service = new InitiativesApiService(
            context,
            BuildConfig(),
            new MemoryCache(new MemoryCacheOptions())
        );
        var controller = BuildController(service, BuildPrincipal(userId: 1, username: "viewer"));

        var result = await controller.SearchCollections("fin");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<InitiativeCollectionSearchResultDto>>(ok.Value);

        var item = Assert.Single(payload);
        Assert.Equal(70, item.Id);
        Assert.Equal("Financial Planning", item.Name);
    }

    private static InitiativesApiController BuildController(
        IInitiativesApiService service,
        ClaimsPrincipal principal
    )
    {
        return new InitiativesApiController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal },
            },
        };
    }

    private static IConfiguration BuildConfig(params (string Key, string Value)[] values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(x => x.Key, x => x.Value))
            .Build();
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
