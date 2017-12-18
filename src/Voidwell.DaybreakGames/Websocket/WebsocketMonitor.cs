using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : HostedService, IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly IWorldMonitor _worldMonitor;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger<WebsocketMonitor> _logger;
        private readonly CensusWebSocketClient _client;

        public WebsocketMonitor(IWebsocketEventHandler handler, IWorldMonitor worldMonitor, DaybreakGamesOptions options, ILogger<WebsocketMonitor> logger)
        {
            _handler = handler;
            _worldMonitor = worldMonitor;
            _options = options;
            _logger = logger;

            _client = new CensusWebSocketClient(_options.CensusServiceKey);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.AutostartWebsocketMonitor)
            {
                return base.StartAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting websocket monitor...");

            if (_client.IsConnecting() || _client.IsConnected())
            {

            }

            var connectTask = _client.ConnectAsync(cancellationToken);
            connectTask.ContinueWith((task) => {
                _logger.LogInformation("Started websocket monitor.");

                Task.WhenAll(Receive(cancellationToken), Subscribe(cancellationToken));
            });

            return connectTask;
        }

        public override Task ForceStopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Force stopping websocket monitor...");

            _client.Abort();

            _logger.LogInformation("Force stopped websocket monitor.");

            return Task.CompletedTask;
        }

        protected override async Task CleanupAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping websocket monitor...");

            if (!_client.IsConnected())
            {
                return;
            }

            await _client.CloseAsync(cancellationToken);

            _logger.LogInformation("Stopped websocket monitor.");
        }

        private async Task Subscribe(CancellationToken cancellationToken)
        {
            await _client.Subscribe(_options.CensusWebsocketWorlds, _options.CensusWebsocketServices.ToArray());
        }

        private async Task Receive(CancellationToken cancellationToken)
        {
            while (_client.IsConnected())
            {
                var message = await _client.ReceiveAsync(cancellationToken);

                #pragma warning disable CS4014
                if (message.Result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                {
                    
                    Task.Run(() => _client.CloseAsync());
                }
                else
                {
                    Task.Run(() => _handler.Process(message.Content));
                }
                #pragma warning restore CS4014
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
