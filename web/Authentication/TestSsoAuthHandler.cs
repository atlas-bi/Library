using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Atlas_Web.Authentication;

public class TestSsoSchemeOptions : AuthenticationSchemeOptions
{
    public const string HeaderName = "X-Test-User";
}

public class TestSsoAuthHandler : AuthenticationHandler<TestSsoSchemeOptions>
{
    public TestSsoAuthHandler(
        IOptionsMonitor<TestSsoSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(TestSsoSchemeOptions.HeaderName, out var values))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var username = values.ToString().Trim();
        if (string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
        };
        var identity = new ClaimsIdentity(claims, "TestSso");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestSso");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
