using Serilog;
using Serilog.Formatting.Compact;

namespace Voidwell.DaybreakGames.Api.Logging;

internal static class LoggingBuilderExtensions
{
    private const string ApplicationName = "Voidwell.DaybreakGames";

    public static ILoggingBuilder AddApiLogging(this ILoggingBuilder builder, IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", ApplicationName);

        if (hostEnvironment.IsDevelopment())
        {
            loggerConfig.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}][{SourceContext}]{NewLine}{Message:lj}{NewLine}{Exception}");
        }
        else
        {
            loggerConfig.WriteTo.Console(new CompactJsonFormatter());
        }

        builder.ClearProviders();
        builder.AddSerilog(loggerConfig.CreateLogger());

        return builder;
    }
}
