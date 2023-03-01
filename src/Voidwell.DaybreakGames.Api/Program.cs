using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog.Events;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using Voidwell.Microservice.Logging;

namespace Voidwell.DaybreakGames.Api
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
                .UseMicroserviceLogging(new LoggingOptions
                {
                    MinLogLevel = LogEventLevel.Information,
                    IncludeMicrosoftInformation = true,
                    LoggingOutput = "flat",
                    IgnoreRules = new List<Func<LogEvent, bool>>
                    {
                        e => Matching.FromSource("Microsoft.AspNetCore.Routing")(e),
                        e => Matching.FromSource("Microsoft.AspNetCore.Mvc")(e),
                        e => Matching.FromSource("Microsoft.EntityFrameworkCore")(e) && e.Level < LogEventLevel.Error,
                        e => Matching.FromSource("Microsoft.EntityFrameworkCore.Update")(e),
                        e => Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command")(e)
                    }
                })
                .Build();
    }
}
