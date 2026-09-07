using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SolrNet;
using SolrNet.Commands.Parameters;
using web.Tests.FunctionTests;

namespace web.Tests.BrowserTests
{
    public class BrowserFactory<TEntryPoint> : IDisposable
        where TEntryPoint : Program
    {
        private readonly IHost _host;

        public Uri BaseAddress { get; }

        public BrowserFactory()
        {
            if (!BrowsersTestData.UseBrowserStack)
            {
                BaseAddress = new Uri("http://127.0.0.1:5000");
                return;
            }

            IHostBuilder builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // needs to be relative to actual project root
                    // so static resources are picked up.
                    // basically we are changing from /web.Tests to /web.
                    webBuilder.UseContentRoot(
                        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "web"))
                    );
                    webBuilder.UseUrls("http://127.0.0.1:0");
                    webBuilder.UseSetting(
                        WebHostDefaults.ApplicationKey,
                        typeof(TEntryPoint).Assembly.GetName().Name
                    );
                    webBuilder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(
                            new Dictionary<string, string>
                            {
                                ["Demo"] = "True",
                                ["DEMO_ADMIN_USERNAME"] = "Default",
                                ["Jwt:Key"] =
                                    "test-jwt-secret-key-for-browser-tests-32-chars-minimum",
                                ["Jwt:Issuer"] = "atlas-test-issuer",
                                ["Jwt:Audience"] = "atlas-test-audience",
                                ["Cors:AllowedOrigins:0"] = "http://localhost:3000",
                                ["Auth:DefaultCallbackPath"] = "/auth/callback",
                            }
                        );
                    });
                })
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<Atlas_WebContext>)
                    );

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<Atlas_WebContext>(options =>
                    {
                        options.UseInMemoryDatabase("LiveServerTests");
                    });
                    services.AddSingleton(CreateSolrAtlasStub());
                    services.AddSingleton(CreateSolrAtlasLookupsStub());

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
                });

            // Start the host in the background.
            // Shut it down in the Dispose method below.
            _host = builder.Build();
            _host.Start();

            // Store base address so that tests can pass it to the browser.
            var address = _host
                ?.Services?.GetRequiredService<IServer>()
                ?.Features.Get<IServerAddressesFeature>()
                ?.Addresses.First();

            if (address != null)
            {
                BaseAddress = new Uri(address);
            }
            else
            {
                BaseAddress = new Uri("http://127.0.0.1:5000");
            }
        }

        private static ISolrReadOnlyOperations<SolrAtlas> CreateSolrAtlasStub()
        {
            var mock = new Mock<ISolrReadOnlyOperations<SolrAtlas>>();
            mock.Setup(x => x.Query(It.IsAny<ISolrQuery>(), It.IsAny<QueryOptions>()))
                .Returns(
                    new SolrQueryResults<SolrAtlas>
                    {
                        NumFound = 0,
                        FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>(),
                        Highlights = new Dictionary<string, SolrNet.Impl.HighlightedSnippets>(),
                        Header = new SolrNet.ResponseHeader { QTime = 1 },
                    }
                );
            return mock.Object;
        }

        private static ISolrReadOnlyOperations<SolrAtlasLookups> CreateSolrAtlasLookupsStub()
        {
            var mock = new Mock<ISolrReadOnlyOperations<SolrAtlasLookups>>();
            mock.Setup(x => x.Query(It.IsAny<ISolrQuery>(), It.IsAny<QueryOptions>()))
                .Returns(
                    new SolrQueryResults<SolrAtlasLookups>
                    {
                        NumFound = 0,
                        FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>(),
                        Highlights = new Dictionary<string, SolrNet.Impl.HighlightedSnippets>(),
                        Header = new SolrNet.ResponseHeader { QTime = 1 },
                    }
                );
            return mock.Object;
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
