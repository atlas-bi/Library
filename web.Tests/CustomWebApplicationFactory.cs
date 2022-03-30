using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Atlas_Web.Models;

namespace web.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<Atlas_WebContext>)
                    );

                    services.Remove(descriptor);

                    services.AddDbContext<Atlas_WebContext>(
                        options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        }
                    );

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<Atlas_WebContext>();
                        var logger = scopedServices.GetRequiredService<
                            ILogger<CustomWebApplicationFactory<TStartup>>
                        >();

                        db.Database.EnsureCreated();

                        try
                        {
                            Utilities.InitializeDbForTests(db);
                            logger.LogWarning("Test database initialized");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(
                                ex,
                                "An error occurred seeding the "
                                    + "database with test messages. Error: {Message}",
                                ex.Message
                            );
                        }
                    }
                }
            );
        }
    }
}
