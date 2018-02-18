using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Census.Stream;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger<WebsocketMonitor> _logger;

        private readonly CensusStreamClient _client;

        public override string ServiceName => "CensusMonitor";

        public WebsocketMonitor(IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options, ICache cache, ILogger<WebsocketMonitor> logger)
            :base(cache)
        {
            _handler = handler;
            _options = options.Value;
            _logger = logger;

            var subscription = new CensusStreamSubscription
            {
                Characters = _options.CensusWebsocketCharacters,
                Worlds = _options.CensusWebsocketWorlds,
                EventNames = _options.CensusWebsocketServices
            };

            _client = new CensusStreamClient(subscription, apiKey: _options.CensusServiceKey);
        }

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableCensusMonitor)
            {
                _logger.LogInformation("Census monitor is disabled");
                return;
            }

            _logger.LogInformation("Starting census stream monitor");

            await _client.ConnectAsync(cancellationToken);

            StartListening();
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            await _client.CloseAsync(cancellationToken);
        }

        private void StartListening()
        {
            Task.Run(StartListeningAsync);
        }

        private async Task StartListeningAsync()
        {
            while (_isRunning)
            {
                if (_client.GetState() != CensusStreamState.Open)
                    continue;

                JToken message = null;

                try
                {
                    message = await _client.ReceiveAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Websocket receive failed: {ex.Message}");
                }

                if (message == null)
                {
                    continue;
                }

                await _handler.Process(message);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
