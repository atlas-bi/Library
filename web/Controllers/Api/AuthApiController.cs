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
    public async Task<IActionResult> Login([FromQuery] string returnUrl = "http://localhost:3000/auth/callback")
    {
        if (_config["Demo"] == "True")
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == "Default");
            if (user == null)
                return NotFound("Demo user not found.");

            var token = _jwt.IssueToken(
                user.Username ?? "Default",
                user.FullnameCalc ?? "Guest",
                user.UserId
            );
            return Redirect($"{returnUrl}?token={token}");
        }

        return Unauthorized(new { error = "SAML login not configured for API flow." });
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
