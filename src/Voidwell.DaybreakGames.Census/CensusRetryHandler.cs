using Microsoft.Extensions.Logging;
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
            var response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            if (_logger != null)
            {
                _logger.LogInformation($"HttpRequest failed. Trying up to {MaxRetries} more times.");
            }

            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            return response;
        }
    }
}
