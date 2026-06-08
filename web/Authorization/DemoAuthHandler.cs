using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Atlas_Web.Authentication
{
#pragma warning disable S2094
    public class DemoSchemeOptions : AuthenticationSchemeOptions
    {
        public string Username { get; set; } = "Default";
    }

    public class DemoAuthHandler : AuthenticationHandler<DemoSchemeOptions>
    {
        public DemoAuthHandler(
            IOptionsMonitor<DemoSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        )
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var username = string.IsNullOrWhiteSpace(Options.Username)
                ? "Default"
                : Options.Username.Trim();
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, "Demo");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Demo");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
