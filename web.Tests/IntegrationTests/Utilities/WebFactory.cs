using System;
using System.Collections.Generic;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace web.Tests.IntegrationTests
{
    public class WebFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Jwt:Key"] = "test-jwt-secret-key-for-integration-tests-32-chars-minimum",
                    ["Jwt:Issuer"] = "atlas-test-issuer",
                    ["Jwt:Audience"] = "atlas-test-audience",
                    ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
                    ["Auth:DefaultCallbackPath"] = "/auth/callback"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Add InMemory database for testing
                // Program.cs won't register SQL Server in Test environment, so no conflict
                services.AddDbContext<Atlas_WebContext>(options =>
                {
                    options.UseInMemoryDatabase("AtlasIntegrationTestDb");
                });
            });
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
                    web.Tests.FunctionTests.Utilities.InitializeDbForTests(db);
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
}
