using Atlas_Web;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Xunit;

namespace web.Tests.FunctionTests;

public class ProgramConfigurationTests
{
    [Fact]
    public void ConfigureJwtAuthentication_UsesSamlWhenSamlConfigurationExists()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Demo"] = "false",
            ["Jwt:Key"] = "test-jwt-secret-key-for-saml-config-test-32-chars",
            ["Jwt:Issuer"] = "test-issuer",
            ["Jwt:Audience"] = "test-audience",
            ["Saml2:Issuer"] = "atlas-library",
        });

        ProgramConfiguration.ConfigureJwtAuthentication(builder);

        using var provider = builder.Services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        Assert.Equal(Saml2Constants.AuthenticationScheme, options.DefaultAuthenticateScheme);
        Assert.Equal(Saml2Constants.AuthenticationScheme, options.DefaultChallengeScheme);
    }
}
