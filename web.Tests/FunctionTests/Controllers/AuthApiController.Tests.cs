using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class AuthApiControllerTests
{
    [Fact]
    public async Task Login_UsesConfiguredDemoAdminUsername_WhenDemoModeIsEnabled()
    {
        await using var context = CreateContext();
        context.Users.Add(
            new User
            {
                UserId = 99,
                Username = "local-admin",
                FullnameCalc = "Local Admin",
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, BuildConfig(("Demo", "True"), ("DEMO_ADMIN_USERNAME", "local-admin")));

        var result = await controller.Login("http://localhost:3000/auth/callback");

        var redirect = Assert.IsType<RedirectResult>(result);
        var target = new Uri(redirect.Url!, UriKind.Absolute);
        var token = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(target.Query)["token"]
            .Single();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(
            "local-admin",
            jwtToken.Claims.Single(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value
        );
        Assert.Equal("99", jwtToken.Claims.Single(c => c.Type == "UserId").Value);
    }

    [Fact]
    public async Task Login_ReturnsNotFound_WhenConfiguredDemoUserDoesNotExist()
    {
        await using var context = CreateContext();
        var controller = BuildController(context, BuildConfig(("Demo", "True"), ("DEMO_ADMIN_USERNAME", "missing")));

        var result = await controller.Login("http://localhost:3000/auth/callback");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Demo user 'missing' not found.", notFound.Value);
    }

    [Fact]
    public async Task Login_UsesDefaultRedirect_WhenReturnUrlIsMissing()
    {
        await using var context = CreateContext();
        context.Users.Add(new User { UserId = 7, Username = "Default", FullnameCalc = "Default User" });
        await context.SaveChangesAsync();

        var controller = BuildController(context, BuildConfig(("Demo", "True")));

        var result = await controller.Login();

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.StartsWith("http://localhost:3000/auth/callback?token=", redirect.Url);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("http://evil.example.com/auth/callback")]
    public async Task Login_UsesDefaultRedirect_WhenReturnUrlIsUnsafe(string returnUrl)
    {
        await using var context = CreateContext();
        context.Users.Add(new User { UserId = 7, Username = "Default", FullnameCalc = "Default User" });
        await context.SaveChangesAsync();

        var controller = BuildController(context, BuildConfig(("Demo", "True")));

        var result = await controller.Login(returnUrl);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.StartsWith("http://localhost:3000/auth/callback?token=", redirect.Url);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenDefaultCallbackPathIsMissing()
    {
        await using var context = CreateContext();
        var controller = BuildController(
            context,
            BuildConfig(
                ("Demo", "True"),
                ("Auth:DefaultCallbackPath", ""),
                ("Cors:AllowedOrigins:0", "http://localhost:3000")
            )
        );

        var result = await controller.Login();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Auth:DefaultCallbackPath is not configured.", badRequest.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenNoAllowedOriginsConfigured()
    {
        await using var context = CreateContext();
        var controller = BuildController(
            context,
            BuildConfig(
                ("Demo", "True"),
                ("Cors:AllowedOrigins:0", ""),
                ("Auth:DefaultCallbackPath", "/auth/callback")
            )
        );

        var result = await controller.Login();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No allowed origins configured.", badRequest.Value);
    }

    [Fact]
    public async Task Login_RedirectsToSamlLogin_WhenDemoModeIsDisabledAndUserIsAnonymous()
    {
        await using var context = CreateContext();
        var controller = BuildController(
            context,
            BuildConfig(("Demo", "False")),
            BuildPrincipal(isAuthenticated: false)
        );

        var result = await controller.Login();

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal(
            "/Auth/Login?returnUrl=%2Fapi%2Fauth%2Flogin%3FreturnUrl%3Dhttp%253A%252F%252Flocalhost%253A3000%252Fauth%252Fcallback",
            redirect.Url
        );
    }

    [Fact]
    public async Task Login_IssuesTokenForAuthenticatedUser_WhenDemoModeIsDisabled()
    {
        await using var context = CreateContext();
        context.Users.Add(
            new User
            {
                UserId = 42,
                Username = "saml-user",
                FullnameCalc = "Saml User",
                Email = "saml@example.com",
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(
            context,
            BuildConfig(("Demo", "False")),
            BuildPrincipal(
                userId: 42,
                username: "saml-user",
                fullname: "Saml Principal",
                isAuthenticated: true
            )
        );

        var result = await controller.Login("http://localhost:3000/auth/callback");

        var redirect = Assert.IsType<RedirectResult>(result);
        var token = Microsoft.AspNetCore.WebUtilities.QueryHelpers
            .ParseQuery(new Uri(redirect.Url!, UriKind.Absolute).Query)["token"]
            .Single();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("saml-user", jwtToken.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal("Saml User", jwtToken.Claims.Single(c => c.Type == "Fullname").Value);
        Assert.Equal("42", jwtToken.Claims.Single(c => c.Type == "UserId").Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenAuthenticatedUserCannotBeResolved()
    {
        await using var context = CreateContext();
        var controller = BuildController(
            context,
            BuildConfig(("Demo", "False")),
            BuildPrincipal(userId: 404, username: "missing-user", isAuthenticated: true)
        );

        var result = await controller.Login("http://localhost:3000/auth/callback");

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(
            "Authenticated SAML user could not be resolved.",
            ReadProperty<string>(unauthorized.Value, "error")
        );
    }

    [Fact]
    public void Me_ReturnsCurrentUserClaims()
    {
        using var context = CreateContext();
        var controller = BuildController(
            context,
            BuildConfig(("Demo", "False")),
            BuildPrincipal(
                userId: 42,
                username: "api-user",
                fullname: "Api User",
                isAuthenticated: true,
                permissions: new[] { "View Groups", "Edit Users" },
                roles: new[] { "Admin", "Editor" },
                adminEnabled: "Y"
            )
        );

        var result = controller.Me();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("api-user", ReadProperty<string>(ok.Value, "username"));
        Assert.Equal("Api User", ReadProperty<string>(ok.Value, "fullname"));
        Assert.Equal("42", ReadProperty<string>(ok.Value, "userId"));
        Assert.Equal(new[] { "Admin", "Editor" }, ReadProperty<string[]>(ok.Value, "roles"));
        Assert.Equal(
            new[] { "View Groups", "Edit Users" },
            ReadProperty<string[]>(ok.Value, "permissions")
        );
        Assert.True(ReadProperty<bool>(ok.Value, "adminEnabled"));
    }

    [Fact]
    public void Logout_ReturnsOkTrue()
    {
        using var context = CreateContext();
        var controller = BuildController(context, BuildConfig());

        var result = controller.Logout();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.True(ReadProperty<bool>(ok.Value, "ok"));
    }

    private static Atlas_WebContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new Atlas_WebContext(options);
    }

    private static AuthApiController BuildController(
        Atlas_WebContext context,
        IConfiguration config = null,
        ClaimsPrincipal principal = null
    )
    {
        var controller = new AuthApiController(CreateJwtService(), context, config ?? BuildConfig())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal ?? BuildPrincipal(isAuthenticated: false),
                },
            },
            Url = BuildUrlHelper(),
        };

        return controller;
    }

    private static JwtTokenService CreateJwtService()
    {
        var signingKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                "test-jwt-secret-key-for-function-tests-32-chars-minimum"
            )
        );

        return new JwtTokenService(signingKey, "atlas-test-issuer", "atlas-test-audience");
    }

    private static IConfiguration BuildConfig(params (string Key, string Value)[] overrides)
    {
        var values = new Dictionary<string, string>
        {
            ["Demo"] = "True",
            ["DEMO_ADMIN_USERNAME"] = "Default",
            ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
            ["Auth:DefaultCallbackPath"] = "/auth/callback",
            ["Jwt:Issuer"] = "atlas-test-issuer",
            ["Jwt:Audience"] = "atlas-test-audience",
        };

        foreach (var (key, value) in overrides)
        {
            values[key] = value;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    private static ClaimsPrincipal BuildPrincipal(
        int userId = 0,
        string username = null,
        string fullname = null,
        bool isAuthenticated = false,
        IEnumerable<string> permissions = null,
        IEnumerable<string> roles = null,
        string adminEnabled = "N"
    )
    {
        var claims = new List<Claim>();

        if (username != null)
        {
            claims.Add(new Claim(ClaimTypes.Name, username));
        }

        if (fullname != null)
        {
            claims.Add(new Claim("Fullname", fullname));
        }

        if (userId != 0)
        {
            claims.Add(new Claim("UserId", userId.ToString()));
        }

        claims.Add(new Claim("AdminEnabled", adminEnabled));

        if (permissions != null)
        {
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));
        }

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var identity = isAuthenticated
            ? new ClaimsIdentity(claims, "TestAuth")
            : new ClaimsIdentity();

        return new ClaimsPrincipal(identity);
    }

    private static IUrlHelper BuildUrlHelper()
    {
        var url = new Mock<IUrlHelper>();
        url.Setup(x => x.Content(It.IsAny<string>()))
            .Returns<string>(value => value.Replace("~/", "/"));
        return url.Object;
    }

    private static T ReadProperty<T>(object instance, string propertyName)
    {
        return (T)instance.GetType().GetProperty(propertyName)!.GetValue(instance)!;
    }
}
