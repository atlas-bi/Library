using System;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // Initialize and seed the test database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<Atlas_WebContext>();
                var logger = services.GetRequiredService<ILogger<WebFactory<TStartup>>>();

                try
                {
                    // Ensure database is created
                    db.Database.EnsureCreated();
                    
                    // Seed test data
                    web.Tests.FunctionTests.Utilities.InitializeDbForTests(db);
                    logger.LogInformation("Test database initialized and seeded");
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "An error occurred initializing the test database. Error: {Message}",
                        ex.Message
                    );
                    throw;
                }
            }

            return host;
        }
    }
}
