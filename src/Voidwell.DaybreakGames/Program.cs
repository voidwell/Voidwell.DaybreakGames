using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Voidwell.DaybreakGames
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5000")
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Error);
                    builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
                    builder.AddFilter("Microsoft.EntityFrameworkCore.Update", LogLevel.None);
                    builder.AddFilter("Microsoft.EntityframeworkCore.Database.Command", LogLevel.None);
                    builder.AddDebug();
                })
                .Build();
    }
}
