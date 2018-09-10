using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Voidwell.Logging.Middleware;

namespace Voidwell.Logging
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddGelf(this ILoggingBuilder builder, Action<LoggingOptions> configure)
        {
            builder.Services.AddOptions();
            builder.Services.Configure(configure);

            builder.Services.AddSingleton<ILoggerProvider, GelfLoggerProvider>();
            return builder;
        }

        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestCompletedMiddleware>();
        }
    }
}
