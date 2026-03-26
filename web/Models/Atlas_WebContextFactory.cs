using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Atlas_Web.Models;

public class Atlas_WebContextFactory : IDesignTimeDbContextFactory<Atlas_WebContext>
{
    public Atlas_WebContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.cust.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<Atlas_WebContext>();
        var connectionString = configuration.GetConnectionString("AtlasDatabase");
        
        optionsBuilder.UseSqlServer(connectionString);

        return new Atlas_WebContext(optionsBuilder.Options);
    }
}
