using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace Atlas_Web.Controllers
{
    [AllowAnonymous]
    [Route("LoggedOut")]
    public class LoggedOutController : Controller
    {
        private readonly Saml2Configuration config;

        public LoggedOutController(Saml2Configuration config)
        {
            this.config = config;
        }

        public IActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            binding.Unbind(Request.ToGenericHttpRequest(), new Saml2LogoutResponse(config));

            return Redirect(Url.Content("~/"));
        }
    }
}
