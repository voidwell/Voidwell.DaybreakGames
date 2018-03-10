using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census
{
    public class CensusRetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 2;
        private readonly ILogger _logger;

        public CensusRetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        public CensusRetryHandler(HttpMessageHandler innerHandler, ILogger logger)
            : base(innerHandler)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int tryCount = 0;
            HttpResponseMessage response = null;
            Exception lastException = null;

            do
            {
                lastException = null;

                try
                {
                    response = await base.SendAsync(request, cancellationToken);
                } catch(Exception ex)
                {
                    lastException = ex;
                }

                if (tryCount == 0 && (response == null || !response.IsSuccessStatusCode))
                {
                    Console.WriteLine($"HttpRequest failed ({response?.StatusCode}). Trying up to {MaxRetries} more times.");
                }

                tryCount++;
            } while ((response == null || !response.IsSuccessStatusCode) && tryCount < MaxRetries);

            if (lastException != null)
            {
                throw lastException;
            }

            return response;
        }
    }
}
