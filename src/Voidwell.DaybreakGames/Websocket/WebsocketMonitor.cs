using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Census.Stream;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly DaybreakGamesOptions _options;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ILogger<WebsocketMonitor> _logger;

        private readonly CensusStreamClient _client;
        private CensusHeartbeat _lastHeartbeat;

        public override string ServiceName => "CensusMonitor";

        public WebsocketMonitor(IWebsocketEventHandler handler, IOptions<DaybreakGamesOptions> options,
            IWorldMonitor worldMonitor, ICache cache, ILogger<WebsocketMonitor> logger)
                :base(cache)
        {
            _handler = handler;
            _options = options.Value;
            _worldMonitor = worldMonitor;
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
            await _worldMonitor.ClearAllWorldStates();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.CloseAsync(cancellationToken);
        }

        public override async Task<ServiceState> GetStatus(CancellationToken cancellationToken)
        {
            var status = await base.GetStatus(cancellationToken);

            status.Details = _lastHeartbeat;

            return status;
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
                    _logger.LogInformation($"Websocket receive failed: {ex}");
                }

                if (message == null)
                {
                    continue;
                }

                if (message.Value<string>("type") == "heartbeat")
                {
                    _lastHeartbeat = new CensusHeartbeat
                    {
                        LastHeartbeat = DateTime.UtcNow,
                        Contents = message.ToObject<object>()
                    };

                    continue;
                }

                ProcessMessage(message);
            }
        }

        private void ProcessMessage(JToken message)
        {
            Task.Run(() =>
            {
                JToken local = message;
                return _handler.Process(local);
            });
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
