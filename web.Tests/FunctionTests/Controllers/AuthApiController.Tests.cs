using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class AuthApiControllerTests
{
    [Fact]
    public async Task Login_UsesConfiguredDemoAdminUsername_WhenDemoModeIsEnabled()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: "auth-api-demo-admin")
            .Options;

        await using var context = new Atlas_WebContext(options);
        context.Users.Add(
            new User
            {
                UserId = 99,
                Username = "local-admin",
                FullnameCalc = "Local Admin",
            }
        );
        await context.SaveChangesAsync();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["Demo"] = "True",
                    ["DEMO_ADMIN_USERNAME"] = "local-admin",
                    ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
                    ["Auth:DefaultCallbackPath"] = "/auth/callback",
                    ["Jwt:Issuer"] = "atlas-test-issuer",
                    ["Jwt:Audience"] = "atlas-test-audience",
                }
            )
            .Build();

        var signingKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                "test-jwt-secret-key-for-function-tests-32-chars-minimum"
            )
        );
        var jwt = new JwtTokenService(signingKey, "atlas-test-issuer", "atlas-test-audience");
        var controller = new AuthApiController(jwt, context, config);

        var result = await controller.Login("http://localhost:3000/auth/callback");

        var redirect = Assert.IsType<RedirectResult>(result);
        var target = new System.Uri(redirect.Url!, System.UriKind.Absolute);
        var token = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(target.Query)["token"]
            .Single();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(
            "local-admin",
            jwtToken.Claims.Single(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value
        );
        Assert.Equal("99", jwtToken.Claims.Single(c => c.Type == "UserId").Value);
    }
}
