using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace web.Tests
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString =
            @"Server=(localdb)\mssqllocaldb;Database=atlas_test;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        // context.AddRange(
                        //     new Blog { Name = "Blog1", Url = "http://blog1.com" },
                        //     new Blog { Name = "Blog2", Url = "http://blog2.com" });
                        // context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public Atlas_WebContext CreateContext() =>
            new Atlas_WebContext(
                new DbContextOptionsBuilder<Atlas_WebContext>().UseSqlServer(
                    ConnectionString
                ).Options
            );

        public IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

        public IConfiguration CreateConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.cust.json", optional: true)
                .AddJsonFile("appsettings.Test.json", optional: true)
                // .AddJsonFile(
                //     $"appsettings.cust.{env.EnvironmentName}.json",
                //     optional: true
                // )
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
