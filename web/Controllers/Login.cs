using Microsoft.AspNetCore.Authorization;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers
{
    [AllowAnonymous]
    [Route("Auth/Login")]
    public class LoginController : Controller
    {
        private readonly Saml2Configuration config;
        const string relayStateReturnUrl = "ReturnUrl";

        public LoginController(Saml2Configuration config)
        {
            this.config = config;
        }

        public IActionResult Login(string returnUrl = null)
        {
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(
                new Dictionary<string, string>
                {
                    { relayStateReturnUrl, returnUrl ?? Url.Content("~/") }
                }
            );

            return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
        }
    }
}
