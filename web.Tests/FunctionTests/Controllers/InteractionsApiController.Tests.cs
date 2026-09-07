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
}
