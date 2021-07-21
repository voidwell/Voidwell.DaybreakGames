using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public class StoreUpdaterSchedulerHostedService : IHostedService
    {
        private readonly IStoreUpdaterService _storeUpdaterService;
        private readonly StoreOptions _options;
        private readonly ILogger<StoreUpdaterSchedulerHostedService> _logger;

        private readonly Dictionary<string, Timer> _updaterTimers = new Dictionary<string, Timer>();

        public StoreUpdaterSchedulerHostedService(IStoreUpdaterService storeUpdaterService, IOptions<StoreOptions> options, ILogger<StoreUpdaterSchedulerHostedService> logger)
        {
            _storeUpdaterService = storeUpdaterService;
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableUpdater)
            {
                return Task.CompletedTask;
            }

            _logger.LogWarning("Updater scheduler starting");

            foreach (var storeUpdate in _storeUpdaterService.GetStoreUpdateLog())
            {
                RegisterUpdaterTimer(storeUpdate);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Updater scheduler stopped!");

            _updaterTimers.Clear();

            return Task.CompletedTask;
        }

        private void RegisterUpdaterTimer(LastStoreUpdate lastStoreUpdate)
        {
            _updaterTimers[lastStoreUpdate.StoreName]?.Dispose();

            var remainingInterval = TimeSpan.Zero;
            if (lastStoreUpdate.LastUpdated != null)
            {
                var offset = lastStoreUpdate.LastUpdated.Value.Add(lastStoreUpdate.UpdateInterval) - DateTime.UtcNow;
                if (offset.TotalMilliseconds > 0)
                {
                    remainingInterval = offset;
                }
            }

            _updaterTimers[lastStoreUpdate.StoreName] = new Timer(HandleTimer, lastStoreUpdate.StoreName, remainingInterval, lastStoreUpdate.UpdateInterval);
        }

        private async void HandleTimer(object stateInfo)
        {
            await _storeUpdaterService.UpdateStoreAsync(stateInfo as string);
        }
    }
}
