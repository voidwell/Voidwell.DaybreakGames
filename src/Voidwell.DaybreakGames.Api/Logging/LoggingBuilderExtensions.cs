using Serilog;
using Serilog.Formatting.Compact;

namespace Voidwell.DaybreakGames.Api.Logging;

internal static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddLogging(this ILoggingBuilder builder, IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        var applicationName = configuration.GetValue<string>("ApplicationName") ?? hostEnvironment.ApplicationName;

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName);

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
