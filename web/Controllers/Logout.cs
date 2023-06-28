using Microsoft.AspNetCore.Authorization;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers
{
    [AllowAnonymous]
    [Route("Logout")]
    public class LogoutController : Controller
    {
        private readonly Saml2Configuration config;
        const string relayStateReturnUrl = "ReturnUrl";

        public LogoutController(Saml2Configuration config)
        {
            this.config = config;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(config, User).DeleteSession(
                HttpContext
            );
            return binding.Bind(saml2LogoutRequest).ToActionResult();
        }
    }
}
