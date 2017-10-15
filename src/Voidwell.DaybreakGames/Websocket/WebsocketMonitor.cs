using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketMonitor : IWebsocketMonitor, IDisposable
    {
        private readonly IWebsocketEventHandler _handler;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ILogger<WebsocketMonitor> _logger;
        private readonly CensusWebSocketClient _client;

        private CancellationTokenSource _connectToken;

        public WebsocketMonitor(IWebsocketEventHandler handler, IWorldMonitor worldMonitor, IConfiguration config, ILogger<WebsocketMonitor> logger)
        {
            _handler = handler;
            _worldMonitor = worldMonitor;
            _logger = logger;

            _client = new CensusWebSocketClient(config.GetValue<string>("CensusKey"));
        }

        public string GetStatus()
        {
            return _client.GetState().ToString();
        }
        
        public async Task StartMonitor()
        {
            _logger.LogInformation("Starting websocket monitor...");

            if (_client.IsConnecting() || _client.IsConnected())
            {
                return;
            }

            _connectToken = new CancellationTokenSource();

            await _client.ConnectAsync(_connectToken.Token);

            _logger.LogInformation("Started websocket monitor.");

            Task.Run(() => Task.WhenAll(Receive(), Subscribe()));
        }

        public async Task StopMonitor()
        {
            _logger.LogInformation("Stopping websocket monitor...");

            if (_client.IsConnecting())
            {
                _connectToken.Cancel();
            }

            if (!_client.IsConnected())
            {
                return;
            }

            await _client.CloseAsync();

            _logger.LogInformation("Stopped websocket monitor.");
        }

        public async Task ResetMonitor()
        {
            _logger.LogInformation("Resetting websocket monitor...");

            if (_client.IsConnecting())
            {
                _connectToken.Cancel();
            }

            if (_client.IsConnected())
            {
                await _client.CloseAsync();
            }

            await _client.ConnectAsync();

            _logger.LogInformation("Reset websocket monitor.");
        }

        private async Task Subscribe()
        {
            await _client.Subscribe(new[] { "17" }, "PlayerLogin", "PlayerLogout");
        }

        private async Task Receive()
        {
            while (_client.IsConnected())
            {
                var message = await _client.ReceiveAsync();

                if (message.Result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                {
                    Task.Run(() => _client.CloseAsync());
                }
                else
                {
                    Task.Run(() => _handler.Process(message.Content));
                }
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
