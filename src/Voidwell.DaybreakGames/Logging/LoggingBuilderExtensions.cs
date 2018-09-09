using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Voidwell.DaybreakGames.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddGelf(this ILoggingBuilder builder, Action<GelfOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, GelfLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
