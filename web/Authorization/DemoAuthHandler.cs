using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Atlas_Web.Authentication
{
#pragma warning disable S2094
    public class DemoSchemeOptions : AuthenticationSchemeOptions { }

    public class DemoAuthHandler : AuthenticationHandler<DemoSchemeOptions>
    {
        public DemoAuthHandler(
            IOptionsMonitor<DemoSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Default"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "Default");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Default");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
