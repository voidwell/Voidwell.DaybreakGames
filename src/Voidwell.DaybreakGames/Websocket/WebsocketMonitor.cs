using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly IWorldMonitor _worldMonitor;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger<WebsocketMonitor> _logger;

        private readonly CensusWebSocketClient _client;

        public WebsocketMonitor(IWebsocketEventHandler handler, IWorldMonitor worldMonitor, IOptions<DaybreakGamesOptions> options, ILogger<WebsocketMonitor> logger)
        {
            _handler = handler;
            _worldMonitor = worldMonitor;
            _options = options.Value;
            _logger = logger;

            _client = new CensusWebSocketClient(_options.CensusServiceKey);
        }

        public async Task StartAsync()
        {
            await StartConnectionAsync();
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Stopping websocket monitor...");

            if (_client.IsConnected())
            {
                await _client.CloseAsync();
            }

            if (_client.IsConnecting())
            {
                _client.Abort();
            }

            _logger.LogInformation("Stopped websocket monitor.");
        }

        public bool IsRunning()
        {
            return _client != null && (_client.IsConnected() || _client.IsConnected());
        }

        private async Task StartConnectionAsync()
        {
            if (_client.IsConnected())
            {
                await _client.CloseAsync();
            }
            else if (_client.IsConnecting())
            {
                _client.Abort();
            }

            _logger.LogInformation("Starting websocket monitor...");

            await _client.ConnectAsync();
            await _client.Subscribe(_options.CensusWebsocketCharacters, _options.CensusWebsocketWorlds, _options.CensusWebsocketServices.ToArray());

            StartListening();

            _logger.LogInformation("Started websocket monitor.");
        }

        private void StartListening()
        {
            Task.Run(StartListeningAsync);
        }

        private async Task StartListeningAsync()
        {
            var failed = false;

            while (_client.IsConnected())
            {
                CensusWebSocketReceiveResult message = null;

                try
                {
                    message = await _client.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Websocket receive failed. Attempting reconnect: {ex.Message}");
                    failed = true;
                    await _client.CloseAsync();
                }

                if (message != null)
                {
                    if (message.Result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                    {
                        await _client.CloseAsync();
                    }
                    else
                    {
                        await _handler.Process(message.Content);
                    }
                }
            }

            if (failed)
            {
                await Task.Delay(5000);
                #pragma warning disable CS4014
                Task.Run(StartConnectionAsync);
                #pragma warning restore CS4014
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                Task.Run(DisposeAsync);
            }
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
        }
    }
}
