using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Stream;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger<WebsocketMonitor> _logger;

        private readonly CensusStreamClient _client;
        private bool _isRunning = false;

        public WebsocketMonitor(IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options, ILogger<WebsocketMonitor> logger)
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

        public async Task StartAsync(CancellationToken ct)
        {
            _isRunning = true;
            _logger.LogInformation("Starting census stream monitor");

            await _client.ConnectAsync(ct);

            StartListening();
        }

        public async Task StopAsync(CancellationToken ct)
        {
            _logger.LogInformation("Stopping census stream monitor");

            await _client.CloseAsync(ct);

            _isRunning = false;
        }

        public bool IsRunning()
        {
            return _isRunning;
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
