using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Voidwell.DaybreakGames.Logging
{
    public class GelfLoggerProvider : ILoggerProvider
    {
        private readonly GelfOptions _options;

        public GelfLoggerProvider(IOptions<GelfOptions> options) : this(options.Value)
        {
        }

        public GelfLoggerProvider(GelfOptions options)
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
