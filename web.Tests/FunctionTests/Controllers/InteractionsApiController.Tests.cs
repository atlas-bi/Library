using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Interactions;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class InteractionsApiControllerTests
{
    [Fact]
    public void SubmitAccessRequest_ExposesAccessRequestPostEndpoint()
    {
        var method = typeof(InteractionsApiController).GetMethod(
            "SubmitAccessRequest",
            BindingFlags.Instance | BindingFlags.Public
        );

        Assert.NotNull(method);

        var route = Assert.Single(method.GetCustomAttributes<HttpPostAttribute>());
        Assert.Equal("access-request", route.Template);
    }

    [Fact]
    public async Task SubmitAccessRequest_RejectsMissingDirector()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new InteractionsApiService(
            context,
            new ConfigurationBuilder().Build(),
            null,
            null,
            cache,
            null
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SubmitAccessRequestAsync(
                new ClaimsPrincipal(),
                new AccessRequestRequestDto
                {
                    ReportName = "Executive Dashboard",
                    ReportUrl = "https://library.example/reports?id=1",
                    DirectorName = " ",
                },
                CancellationToken.None
            )
        );

        Assert.Equal("Report name, report URL, and director are required.", exception.Message);
    }

    [Fact]
    public async Task SearchDirectors_ReturnsDirectAndGroupDirectorsMatchingTheQuery()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.UserRoles.Add(new UserRole { UserRolesId = 3, Name = "Director" });
        context.UserRoles.Add(new UserRole { UserRolesId = 4, Name = "User" });
        context.Users.AddRange(
            new User
            {
                UserId = 1,
                Username = "jane",
                FullnameCalc = "Jane Smith",
                Email = "jane@example.org",
            },
            new User
            {
                UserId = 2,
                Username = "sam",
                FullnameCalc = "Sam Director",
                Email = "sam@example.org",
            },
            new User
            {
                UserId = 3,
                Username = "bob",
                FullnameCalc = "Bob Analyst",
                Email = "bob@example.org",
            }
        );
        context.UserRoleLinks.Add(
            new UserRoleLink { UserRoleLinksId = 1, UserId = 1, UserRolesId = 3 }
        );
        context.UserRoleLinks.Add(
            new UserRoleLink { UserRoleLinksId = 2, UserId = 3, UserRolesId = 4 }
        );
        context.GroupRoleLinks.Add(
            new GroupRoleLink { GroupRoleLinksId = 1, GroupId = 9, UserRolesId = 3 }
        );
        context.UserGroupsMemberships.Add(
            new UserGroupsMembership { MembershipId = 1, GroupId = 9, UserId = 2 }
        );
        await context.SaveChangesAsync();

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new InteractionsApiService(
            context,
            new ConfigurationBuilder().Build(),
            null,
            null,
            cache,
            null
        );

        var matches = await service.SearchDirectorsAsync("Smith", CancellationToken.None);
        var match = Assert.Single(matches);
        Assert.Equal(1, match.Id);
        Assert.Equal("Jane Smith", match.Name);
        Assert.Equal("jane@example.org", match.Email);

        var directors = await service.SearchDirectorsAsync(null, CancellationToken.None);
        Assert.Equal(new[] { 1, 2 }, directors.Select(x => x.Id).OrderBy(x => x).ToArray());
    }

    [Fact]
    public async Task ToggleStar_StarsAndUnstarsInitiativesTermsUsersAndGroups()
    {
        await using var context = CreateContext();
        context.Initiatives.Add(new Initiative { InitiativeId = 8, Name = "Quality" });
        context.Terms.Add(new Term { TermId = 15, Name = "Encounter" });
        context.Users.Add(new User { UserId = 4, Username = "target", FullnameCalc = "Target" });
        context.UserGroups.Add(new UserGroup { GroupId = 9, GroupName = "Finance" });
        context.StarredInitiatives.Add(new StarredInitiative { Ownerid = 2, Initiativeid = 8 });
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var user = UserPrincipal(1);

        var initiative = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "Initiative", Id = 8 },
            CancellationToken.None
        );
        var term = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "term", Id = 15 },
            CancellationToken.None
        );
        var starredUser = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "user", Id = 4 },
            CancellationToken.None
        );
        var group = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "group", Id = 9 },
            CancellationToken.None
        );

        Assert.True(initiative.IsStarred);
        Assert.Equal(2, initiative.Count);
        Assert.True(term.IsStarred);
        Assert.Equal(1, term.Count);
        Assert.True(starredUser.IsStarred);
        Assert.True(group.IsStarred);

        var unstarred = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "initiative", Id = 8 },
            CancellationToken.None
        );

        Assert.False(unstarred.IsStarred);
        Assert.Equal(1, unstarred.Count);
        Assert.Equal(0, await context.StarredInitiatives.CountAsync(x => x.Ownerid == 1));
    }

    [Fact]
    public async Task ToggleStar_StarsAndUnstarsASearch()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        var user = UserPrincipal(1);

        var starred = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "search", Search = "revenue" },
            CancellationToken.None
        );
        var unstarred = await service.ToggleStarAsync(
            user,
            new ToggleStarRequestDto { Type = "search", Search = " revenue " },
            CancellationToken.None
        );

        Assert.True(starred.IsStarred);
        Assert.Equal(1, starred.Count);
        Assert.False(unstarred.IsStarred);
        Assert.Equal(0, unstarred.Count);
    }

    [Fact]
    public async Task ToggleStar_ReturnsNullWhenTheTargetDoesNotExist()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var missing = await service.ToggleStarAsync(
            UserPrincipal(1),
            new ToggleStarRequestDto { Type = "term", Id = 99 },
            CancellationToken.None
        );

        Assert.Null(missing);
    }

    private static Atlas_WebContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new Atlas_WebContext(options);
    }

    private static InteractionsApiService CreateService(Atlas_WebContext context)
    {
        return new InteractionsApiService(
            context,
            new ConfigurationBuilder().Build(),
            null,
            null,
            new MemoryCache(new MemoryCacheOptions()),
            null
        );
    }

    private static ClaimsPrincipal UserPrincipal(int userId)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()) })
        );
    }
}
