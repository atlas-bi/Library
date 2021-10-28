
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.cust.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.cust.{env.EnvironmentName}.json",
                                         optional: true, reloadOnChange: true);

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true).UseStartup<Startup>();
                });
    }
}
