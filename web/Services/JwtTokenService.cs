using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Atlas_Web.Services;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _signingKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenService(SymmetricSecurityKey signingKey, string issuer, string audience)
    {
        _signingKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
        _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));
    }

    public string IssueToken(string username, string fullname, int userId)
    {
#pragma warning disable S6781 // JWT secret keys should not be disclosed - False positive: key is injected at startup, not read from config
        var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
#pragma warning restore S6781

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("Fullname", fullname),
            new Claim("UserId", userId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
