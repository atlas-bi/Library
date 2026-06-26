using System.Text;
using Atlas_Web.Configuration;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.IdentityModel.Tokens;
using Atlas_Web.Authentication;

namespace Atlas_Web;

public static class ProgramConfiguration
{
    public static void ConfigureCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("NextJs", policy =>
            {
                var origins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>();
                
                if (origins == null || origins.Length == 0)
                {
                    if (builder.Environment.IsEnvironment("Test"))
                    {
                        origins = new[] { "http://localhost:3000" };
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "CORS allowed origins are not configured. Please set Cors:AllowedOrigins in configuration."
                        );
                    }
                }
                
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }

    public static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
        {
            if (builder.Environment.IsEnvironment("Test"))
            {
#pragma warning disable S6781 // JWT secret keys should not be disclosed - Test defaults only, not production secrets
                jwtKey = "test-jwt-secret-key-for-ci-testing-minimum-32-characters";
                jwtIssuer = "atlas-test-issuer";
                jwtAudience = "atlas-test-audience";
#pragma warning restore S6781
            }
            else
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing. Please set Jwt:Key, Jwt:Issuer, and Jwt:Audience via environment variables or configuration files."
                );
            }
        }

#pragma warning disable S6781 // JWT secret keys should not be disclosed - Key from config or test defaults, not hardcoded
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
#pragma warning restore S6781
        builder.Services.AddScoped<JwtTokenService>(_ => new JwtTokenService(signingKey, jwtIssuer, jwtAudience));

        if (BooleanConfig.IsEnabled(builder.Configuration["Demo"]))
        {
            ConfigureDemoAuthentication(builder, jwtIssuer, jwtAudience, signingKey);
        }
        else
        {
            ConfigureNegotiateAuthentication(builder, jwtIssuer, jwtAudience, signingKey);
        }
    }

    private static void ConfigureDemoAuthentication(
        WebApplicationBuilder builder,
        string jwtIssuer,
        string jwtAudience,
        SymmetricSecurityKey signingKey)
    {
#pragma warning disable S1116
        builder
            .Services.AddAuthentication(options => options.DefaultScheme = "Demo")
            .AddScheme<DemoSchemeOptions, DemoAuthHandler>("Demo", options =>
            {
                options.Username = builder.Configuration["DEMO_ADMIN_USERNAME"] ?? "Default";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = signingKey,
                };
            });
        ;
#pragma warning restore S1116
    }

    private static void ConfigureNegotiateAuthentication(
        WebApplicationBuilder builder,
        string jwtIssuer,
        string jwtAudience,
        SymmetricSecurityKey signingKey)
    {
        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate()
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = signingKey,
                };
            });
    }
}
