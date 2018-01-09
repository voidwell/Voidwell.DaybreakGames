using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System;

namespace Voidwell.DaybreakGames.Websocket
{
    public class HostedWebsocketMonitor : IHostedService
    {
        private readonly IWebsocketMonitor _websocketMonitor;
        private readonly DaybreakGamesOptions _options;

        public HostedWebsocketMonitor(IWebsocketMonitor websocketMonitor, DaybreakGamesOptions options)
        {
            _websocketMonitor = websocketMonitor;
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.AutostartWebsocketMonitor)
            {
                #pragma warning disable CS4014
                Task.Run(() => _websocketMonitor.StartAsync());
                #pragma warning restore CS4014
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _websocketMonitor.StopAsync();
        }
    }
}
