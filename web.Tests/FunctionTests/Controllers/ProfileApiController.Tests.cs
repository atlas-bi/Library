using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class ProfileApiControllerTests
{
    [Theory]
    [InlineData("stars")]
    [InlineData("subscriptions")]
    public async Task InvalidRelationshipType_ReturnsBadRequest(string endpoint)
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase($"profile-invalid-{endpoint}")
            .Options;
        await using var context = new Atlas_WebContext(options);
        var service = new ProfileApiService(
            context,
            new ConfigurationBuilder().AddInMemoryCollection().Build()
        );
        var controller = new ProfileApiController(service);

        IActionResult result;
        if (endpoint == "stars")
        {
            result = (await controller.GetStars(1, "user")).Result;
        }
        else
        {
            result = (await controller.GetSubscriptions(1, "user")).Result;
        }

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
