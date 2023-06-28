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

namespace web.Tests.FunctionTests
{
    public static class Utilities
    {
        public static void InitializeDbForTests(Atlas_WebContext db)
        {
            db.Users.AddRange(GetSeedUser());
            //db.Messages.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(Atlas_WebContext db)
        {
            db.Users.RemoveRange(GetSeedUser());
            //db.Messages.RemoveRange(db.Messages);
            InitializeDbForTests(db);
        }

        public static List<User> GetSeedUser()
        {
            return new List<User>()
            {
                new User() { Username = "Default", UserId = 1 },
                new User() { Username = "user 2", UserId = 2 }
            };
        }
        // public static List<Message> GetSeedingMessages()
        // {
        //     return new List<Message>()asf
        //     {
        //         new Message(){ Text = "TEST RECORD: You're standing on my scarf." },
        //         new Message(){ Text = "TEST RECORD: Would you like a jelly baby?" },
        //         new Message(){ Text = "TEST RECORD: To the rational mind, " +
        //             "nothing is inexplicable; only unexplained." }
        //     };
        // }
    }

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
                new DbContextOptionsBuilder<Atlas_WebContext>()
                    .UseSqlServer(ConnectionString)
                    .Options
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
