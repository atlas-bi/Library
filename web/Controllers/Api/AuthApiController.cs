#nullable enable
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Login([FromQuery] string? returnUrl = null)
    {
        if (_config["Demo"] != "True")
        {
            return Unauthorized(new { error = "SAML login not configured for API flow." });
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == "Default");
        if (user == null)
        {
            return NotFound("Demo user not found.");
        }

        var safeReturnUrlResult = GetSafeRedirectUrl(returnUrl);
        if (safeReturnUrlResult is BadRequestObjectResult)
        {
            return safeReturnUrlResult;
        }

        var safeReturnUrl = ((OkObjectResult)safeReturnUrlResult).Value as string ?? string.Empty;
        var token = _jwt.IssueToken(
            user.Username ?? "Default",
            user.FullnameCalc ?? "Guest",
            user.UserId
        );
        
        var redirectUrl = $"{safeReturnUrl}?token={token}";
        return Redirect(redirectUrl);
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

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            username = User.Identity?.Name,
            fullname = User.FindFirst("Fullname")?.Value,
            userId = User.FindFirst("UserId")?.Value,
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { ok = true });
    }
}
