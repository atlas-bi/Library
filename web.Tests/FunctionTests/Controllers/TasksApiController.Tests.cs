using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Contracts.Api.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Mvc;
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
        Assert.Equal("api/tasks", typeof(TasksApiController)
            .GetCustomAttributes<RouteAttribute>().Single().Template);
        Assert.Contains(
            typeof(TasksApiController).GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>(),
            attribute => attribute.AuthenticationSchemes == "Bearer"
        );
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyBucketsForEmptyDatabase()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new Atlas_WebContext(options);
        var service = new TasksApiService(context);

        var result = await service.GetTasksAsync(new ClaimsPrincipal(), CancellationToken.None);

        Assert.Empty(result.Unused);
        Assert.Empty(result.MaintenanceRequired);
        Assert.Empty(result.TopUndocumented);
    }
}
