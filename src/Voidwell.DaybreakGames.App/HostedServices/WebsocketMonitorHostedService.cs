using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.GameState.CensusStream;
using Voidwell.DaybreakGames.Utils.StatefulHostedService;

namespace Voidwell.DaybreakGames.App.HostedServices
{
    public class WebsocketMonitorHostedService : StatefulHostedService
    {
        private readonly IWebsocketMonitor _service;
        private readonly AppOptions _options;
        private readonly ILogger<WebsocketMonitorHostedService> _logger;

        public WebsocketMonitorHostedService(IWebsocketMonitor service, IOptions<AppOptions> options, ICache cache,
            ILogger<WebsocketMonitorHostedService> logger)
            : base(cache)
        {
            _service = service;
            _options = options.Value;
            _logger = logger;
        }

        public override string ServiceName => "CensusMonitor";

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableCensusMonitor)
            {
                _logger.LogInformation("Census monitor is disabled");
                return;
            }

            _logger.LogInformation("Starting census stream monitor");

            try
            {
                await _service.ConnectAsync();
            }
            catch (Exception ex)
            {
                await _service.DisconnectAsync();
                await UpdateStateAsync(false);

                _logger.LogError(91435, ex, "Failed to establish initial connection to Census. Will not attempt to reconnect.");
            }
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            await _service.DisconnectAsync();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _service?.DisconnectAsync();
            await base.OnApplicationShutdown(cancellationToken);
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_service.GetLastHeartbeat());
        }
    }
}
