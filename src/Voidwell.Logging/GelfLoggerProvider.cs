using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Voidwell.Logging
{
    public class GelfLoggerProvider : ILoggerProvider
    {
        private readonly LoggingOptions _options;

        public GelfLoggerProvider(IOptions<LoggingOptions> options) : this(options.Value)
        {
        }

        public GelfLoggerProvider(LoggingOptions options)
        {

            if (string.IsNullOrEmpty(options.LogSource))
            {
                throw new ArgumentException("GELF log source is required.", nameof(options));
            }

            _options = options;
        }

        public ILogger CreateLogger(string name)
        {
            return new GelfLogger(name, _options);
        }

        public void Dispose()
        {
        }
    }
}
