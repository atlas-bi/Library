using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using web.Tests.FunctionTests;
using Microsoft.Extensions.Logging;
using Atlas_Web.Models;

namespace web.Tests.BrowserTests
{
    public class BrowserFactory<TEntryPoint> : IDisposable where TEntryPoint : Program
    {
        private readonly IHost? _host;

        public Uri BaseAddress { get; }

        public BrowserFactory()
        {
            IHostBuilder builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<TEntryPoint>();

                        // needs to be relative to actual project root
                        // so static resources are picked up.
                        // basically we are changing from /web.Tests to /web.
                        webBuilder.UseSolutionRelativeContentRoot("web");
                        webBuilder.UseUrls("http://127.0.0.1:0");
                    }
                )
                .ConfigureServices(
                    services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<Atlas_WebContext>)
                        );

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<Atlas_WebContext>(
                            options =>
                            {
                                options.UseInMemoryDatabase("LiveServerTests");
                            }
                        );

                        var sp = services.BuildServiceProvider();

                        using (var scope = sp.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            var db = scopedServices.GetRequiredService<Atlas_WebContext>();
                            var logger = scopedServices.GetRequiredService<
                                ILogger<BrowserFactory<TEntryPoint>>
                            >();

                            db.Database.EnsureDeleted();
                            db.Database.EnsureCreated();

                            try
                            {
                                // test data is loaded here.
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

            // Start the host in the background.
            // Shut it down in the Dispose method below.
            _host = builder.Build();
            _host.Start();

            // Store base address so that tests can pass it to the browser.
            var address =
                _host?.Services?.GetRequiredService<IServer>()?.Features.Get<IServerAddressesFeature>()?.Addresses.First();

            if (address != null)
            {
                BaseAddress = new Uri(address);
            }
            else
            {
                BaseAddress = new Uri("http://127.0.0.1:5000");
            }
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
