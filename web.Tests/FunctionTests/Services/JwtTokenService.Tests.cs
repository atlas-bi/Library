using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Atlas_Web.Services;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace web.Tests.FunctionTests.Services;

public class JwtTokenServiceTests
{
    [Fact]
    public void IssueToken_WritesExpectedClaimsMetadataAndLifetime()
    {
        var before = DateTime.UtcNow;
        var service = CreateService();

        var token = service.IssueToken("alice", "Alice Example", 42);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("atlas-test-issuer", jwt.Issuer);
        Assert.Equal("atlas-test-audience", jwt.Audiences.Single());
        Assert.Equal("alice", jwt.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal("Alice Example", jwt.Claims.Single(c => c.Type == "Fullname").Value);
        Assert.Equal("42", jwt.Claims.Single(c => c.Type == "UserId").Value);
        Assert.True(jwt.ValidTo >= before.AddHours(7));
        Assert.True(jwt.ValidTo <= before.AddHours(9));
    }

    [Fact]
    public void Constructor_Throws_WhenSigningKeyIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JwtTokenService(null!, "issuer", "audience")
        );

        Assert.Equal("signingKey", exception.ParamName);
    }

    [Fact]
    public void Constructor_Throws_WhenIssuerIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JwtTokenService(CreateSigningKey(), null!, "audience")
        );

        Assert.Equal("issuer", exception.ParamName);
    }

    [Fact]
    public void Constructor_Throws_WhenAudienceIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new JwtTokenService(CreateSigningKey(), "issuer", null!)
        );

        Assert.Equal("audience", exception.ParamName);
    }

    private static JwtTokenService CreateService()
    {
        return new JwtTokenService(
            CreateSigningKey(),
            "atlas-test-issuer",
            "atlas-test-audience"
        );
    }

    private static SymmetricSecurityKey CreateSigningKey()
    {
        return new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("test-jwt-secret-key-for-service-tests-32-chars-minimum")
        );
    }
}
