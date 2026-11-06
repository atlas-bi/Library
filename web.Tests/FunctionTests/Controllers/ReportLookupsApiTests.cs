using System;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class ReportLookupsApiTests
{
    [Fact]
    public async Task GetLookupValues_ReadsSqlListsInNameOrder()
    {
        await using var context = CreateContext();
        context.OrganizationalValues.AddRange(
            new OrganizationalValue { Id = 2, Name = "Zeta" },
            new OrganizationalValue { Id = 1, Name = "Alpha" }
        );
        context.UserRoles.Add(new UserRole { UserRolesId = 7, Name = "Director" });
        await context.SaveChangesAsync();

        var service = new ReportsApiService(
            context,
            null,
            null,
            new ConfigurationBuilder().Build(),
            new MemoryCache(new MemoryCacheOptions()),
            null
        );

        var values = await service.GetLookupValuesAsync("org-value", CancellationToken.None);
        var roles = await service.GetLookupValuesAsync("user-roles", CancellationToken.None);
        var unknown = await service.GetLookupValuesAsync("missing", CancellationToken.None);

        Assert.Equal(new[] { 1, 2 }, new[] { values[0].Id, values[1].Id });
        Assert.Equal("Alpha", values[0].Name);
        Assert.Equal(7, Assert.Single(roles).Id);
        Assert.Equal("Director", roles[0].Name);
        Assert.Empty(unknown);
    }

    private static Atlas_WebContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new Atlas_WebContext(options);
    }
}
