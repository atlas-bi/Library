using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Contracts.Api.Settings;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class SettingsApiControllerTests
{
    [Fact]
    public void SettingsApi_ExposesExpectedRoutes()
    {
        var methods = typeof(SettingsApiController).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        Assert.Contains(methods, method => method.GetCustomAttribute<HttpGetAttribute>()?.Template == "site-messages");
        Assert.Contains(methods, method => method.GetCustomAttribute<HttpGetAttribute>()?.Template == "search");
        Assert.Contains(methods, method => method.GetCustomAttribute<HttpGetAttribute>()?.Template == "roles");
        Assert.Contains(methods, method => method.GetCustomAttribute<HttpGetAttribute>()?.Template == "tags/{type}");
    }

    [Fact]
    public async Task SiteMessages_ReadAndWriteUsesMsgSettingsOnly()
    {
        await using var context = new Atlas_WebContext(new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.GlobalSiteSettings.AddRange(
            new GlobalSiteSetting { Id = 1, Name = "msg", Value = "1", Description = "Welcome" },
            new GlobalSiteSetting { Id = 2, Name = "global_css", Value = "body{}" });
        await context.SaveChangesAsync();
        var controller = BuildController(context, "Manage Global Site Settings");

        var before = Assert.IsType<OkObjectResult>((await controller.GetSiteMessages()).Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<SiteMessageDto>>(before.Value));

        var created = Assert.IsType<OkObjectResult>((await controller.AddSiteMessage(
            new SiteMessageRequestDto { Value = "2", Description = "Updated" })).Result);
        Assert.Equal("2", Assert.IsType<SiteMessageDto>(created.Value).Value);
        Assert.Equal(2, await context.GlobalSiteSettings.CountAsync(x => x.Name == "msg"));
    }

    [Fact]
    public async Task SiteMessageWriteRequiresPermission()
    {
        await using var context = new Atlas_WebContext(new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var controller = BuildController(context);

        var result = await controller.AddSiteMessage(new SiteMessageRequestDto { Value = "1" });

        Assert.IsType<ForbidResult>(result.Result);
    }

    private static SettingsApiController BuildController(Atlas_WebContext context, params string[] permissions)
    {
        return new SettingsApiController(context, new MemoryCache(new MemoryCacheOptions()), null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        permissions.Select(x => new Claim("Permission", x)), "Test"))
                }
            }
        };
    }
}
