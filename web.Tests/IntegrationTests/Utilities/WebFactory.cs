using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace web.Tests.IntegrationTests
{
    public class WebFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly string _databaseName = $"AtlasIntegrationTestDb-{Guid.NewGuid()}";
        private const string JwtKey =
            "test-jwt-secret-key-for-integration-tests-32-chars-minimum";

        protected virtual bool DemoEnabled => true;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(BuildConfiguration());
            });

            builder.ConfigureTestServices(services =>
            {
                // Add InMemory database for testing
                // Program.cs won't register SQL Server in Test environment, so no conflict
                services.AddDbContext<Atlas_WebContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                services.AddScoped<Atlas_Web.Services.JwtTokenService>(_ =>
                    new Atlas_Web.Services.JwtTokenService(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                        "atlas-test-issuer",
                        "atlas-test-audience"
                    )
                );

                services.PostConfigure<JwtBearerOptions>("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "atlas-test-issuer",
                        ValidAudience = "atlas-test-audience",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
                    };
                });

                services.AddSingleton(CreateSolrAtlasStub());
                services.AddSingleton(CreateSolrAtlasLookupsStub());
            });
        }

        private Dictionary<string, string> BuildConfiguration()
        {
            return new Dictionary<string, string>
            {
                ["Demo"] = DemoEnabled ? "True" : "False",
                ["DEMO_ADMIN_USERNAME"] = "Default",
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = "atlas-test-issuer",
                ["Jwt:Audience"] = "atlas-test-audience",
                ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
                ["Auth:DefaultCallbackPath"] = "/auth/callback",
            };
        }

        private static ISolrReadOnlyOperations<SolrAtlas> CreateSolrAtlasStub()
        {
            var mock = new Mock<ISolrReadOnlyOperations<SolrAtlas>>();
            mock.Setup(x =>
                    x.QueryAsync(
                        It.IsAny<ISolrQuery>(),
                        It.IsAny<QueryOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(CreateSolrResults<SolrAtlas>());
            mock.Setup(x => x.Query(It.IsAny<ISolrQuery>(), It.IsAny<QueryOptions>()))
                .Returns(CreateSolrResults<SolrAtlas>());
            return mock.Object;
        }

        private static ISolrReadOnlyOperations<SolrAtlasLookups> CreateSolrAtlasLookupsStub()
        {
            var mock = new Mock<ISolrReadOnlyOperations<SolrAtlasLookups>>();
            mock.Setup(x =>
                    x.QueryAsync(
                        It.IsAny<ISolrQuery>(),
                        It.IsAny<QueryOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(CreateSolrResults<SolrAtlasLookups>());
            mock.Setup(x => x.Query(It.IsAny<ISolrQuery>(), It.IsAny<QueryOptions>()))
                .Returns(CreateSolrResults<SolrAtlasLookups>());
            return mock.Object;
        }

        private static SolrQueryResults<T> CreateSolrResults<T>()
        {
            return new SolrQueryResults<T>
            {
                NumFound = 0,
                FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>(),
                Highlights = new Dictionary<string, SolrNet.Impl.HighlightedSnippets>(),
                Header = new SolrNet.ResponseHeader { QTime = 1 },
            };
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // Seed the database after host is created
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<Atlas_WebContext>();
                var logger = services.GetRequiredService<ILogger<WebFactory<TStartup>>>();

                try
                {
                    db.Database.EnsureCreated();
                    if (!db.Users.Any())
                    {
                        web.Tests.FunctionTests.Utilities.InitializeDbForTests(db);
                    }
                    logger.LogInformation("Test database initialized and seeded");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the test database.");
                    throw;
                }
            }

            return host;
        }
    }

    public class SsoWebFactory<TStartup> : WebFactory<TStartup>
        where TStartup : class
    {
        protected override bool DemoEnabled => false;
    }
}
