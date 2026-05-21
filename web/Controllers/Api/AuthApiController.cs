#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly JwtTokenService _jwt;
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _config;

    public AuthApiController(JwtTokenService jwt, Atlas_WebContext context, IConfiguration config)
    {
        _jwt = jwt;
        _context = context;
        _config = config;
    }

    [AllowAnonymous]
    [HttpGet("login")]
    [SuppressMessage("Security", "S5146:HTTP request redirections should not be open to forging attacks", Justification = "URL is validated against CORS allowlist in GetSafeRedirectUrl before redirect")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context - False positive: #nullable enable is set at file scope
    public async Task<IActionResult> Login([FromQuery] string? returnUrl = null)
#pragma warning restore CS8632
    {
        var safeReturnUrlResult = GetSafeRedirectUrl(returnUrl);
        if (safeReturnUrlResult is BadRequestObjectResult)
        {
            return safeReturnUrlResult;
        }

        var safeReturnUrl = ((OkObjectResult)safeReturnUrlResult).Value as string ?? string.Empty;

        if (_config["Demo"] == "True")
        {
            var demoUsername = _config["DEMO_ADMIN_USERNAME"];
            var selectedDemoUsername = string.IsNullOrWhiteSpace(demoUsername)
                ? "Default"
                : demoUsername.Trim();
            var user = await _context.Users.FirstOrDefaultAsync(
                x => x.Username == selectedDemoUsername
            );

            if (user == null)
            {
                return NotFound($"Demo user '{selectedDemoUsername}' not found.");
            }

            var demoToken = _jwt.IssueToken(
                user.Username ?? "Default",
                user.FullnameCalc ?? "Guest",
                user.UserId
            );

            return Redirect(BuildTokenRedirectUrl(safeReturnUrl, demoToken));
        }

        if (User.Identity?.IsAuthenticated != true)
        {
            return Redirect(BuildSamlLoginUrl(safeReturnUrl));
        }

        var apiUser = await ResolveAuthenticatedUserAsync(User);
        if (apiUser == null)
        {
            return Unauthorized(new { error = "Authenticated SAML user could not be resolved." });
        }

        var token = _jwt.IssueToken(
            apiUser.Username ?? User.Identity?.Name ?? "Guest",
            apiUser.FullnameCalc ?? User.FindFirstValue("Fullname") ?? apiUser.Username ?? "Guest",
            apiUser.UserId
        );

        return Redirect(BuildTokenRedirectUrl(safeReturnUrl, token));
    }

    private IActionResult GetSafeRedirectUrl(string? returnUrl)
    {
        var allowedOrigins = _config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var defaultCallbackPath = _config["Auth:DefaultCallbackPath"];
        
        if (string.IsNullOrWhiteSpace(defaultCallbackPath))
        {
            return BadRequest("Auth:DefaultCallbackPath is not configured.");
        }

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return BuildDefaultRedirectUrl(allowedOrigins, defaultCallbackPath);
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var parsedReturnUrl))
        {
            return BuildDefaultRedirectUrl(allowedOrigins, defaultCallbackPath);
        }

        if (IsUrlAllowed(parsedReturnUrl, allowedOrigins))
        {
            return Ok(returnUrl);
        }

        return BuildDefaultRedirectUrl(allowedOrigins, defaultCallbackPath);
    }

    private bool IsUrlAllowed(Uri url, string[] allowedOrigins)
    {
        foreach (var origin in allowedOrigins)
        {
            if (Uri.TryCreate(origin, UriKind.Absolute, out var parsedOrigin)
                && string.Equals(parsedOrigin.Scheme, url.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(parsedOrigin.Host, url.Host, StringComparison.OrdinalIgnoreCase)
                && parsedOrigin.Port == url.Port)
            {
                return true;
            }
        }
        return false;
    }

    private IActionResult BuildDefaultRedirectUrl(string[] allowedOrigins, string defaultCallbackPath)
    {
        var defaultOrigin = allowedOrigins.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(defaultOrigin))
        {
            return BadRequest("No allowed origins configured.");
        }
        
        var safeUrl = defaultOrigin.TrimEnd('/') + defaultCallbackPath;
        return Ok(safeUrl);
    }

    private async Task<User?> ResolveAuthenticatedUserAsync(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirstValue("UserId");
        if (int.TryParse(userIdClaim, out var userId))
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        var identityName = principal.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(identityName))
        {
            var user = await FindUserByIdentityAsync(identityName);
            if (user != null)
            {
                return user;
            }
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrWhiteSpace(email))
        {
            return await FindUserByIdentityAsync(email);
        }

        return null;
    }

    private Task<User?> FindUserByIdentityAsync(string identity)
    {
        if (identity.Contains("@"))
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Email == identity || x.Username == identity);
        }

        return _context.Users.FirstOrDefaultAsync(x => x.Username == identity);
    }

    private string BuildSamlLoginUrl(string safeReturnUrl)
    {
        var apiLoginUrl = QueryHelpers.AddQueryString(
            Url.Content("~/api/auth/login"),
            "returnUrl",
            safeReturnUrl
        );

        return QueryHelpers.AddQueryString(
            Url.Content("~/Auth/Login"),
            "returnUrl",
            apiLoginUrl
        );
    }

    private static string BuildTokenRedirectUrl(string safeReturnUrl, string token)
    {
        return QueryHelpers.AddQueryString(safeReturnUrl, "token", token);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            username = User.Identity?.Name,
            fullname = User.FindFirst("Fullname")?.Value,
            userId = User.FindFirst("UserId")?.Value,
            roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
            permissions = User.FindAll("Permission").Select(c => c.Value).ToArray(),
            adminEnabled = User.FindFirst("AdminEnabled")?.Value == "Y",
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { ok = true });
    }
}
