using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Services;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.HostedServices
{
    public class StoreUpdaterSchedulerHostedService : IHostedService
    {
        private readonly IUpdaterSchedulerRepository _updaterSchedulerRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger<StoreUpdaterSchedulerHostedService> _logger;
        private readonly Dictionary<string, Timer> _updaterTimers = new Dictionary<string, Timer>();

        private readonly List<object> _pendingWork = new List<object>();
        private bool _isWorking = false;

        public StoreUpdaterSchedulerHostedService(IUpdaterSchedulerRepository updaterSchedulerRepository, IServiceProvider serviceProvider,
            IOptions<DaybreakGamesOptions> options, ILogger<StoreUpdaterSchedulerHostedService> logger)
        {
            _updaterSchedulerRepository = updaterSchedulerRepository;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableUpdater)
            {
                return Task.CompletedTask;
            }

            var updatableTypes = typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a));

            var storeUpdaterInterfaces = updatableTypes.Where(a => a.GetTypeInfo().IsInterface && !typeof(IUpdateable).IsEquivalentTo(a));
            var storeUpdaterTypes = updatableTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract);

            var storeUpdaterMatches = storeUpdaterTypes.Select(t => new[] { t, storeUpdaterInterfaces.SingleOrDefault(i => i.IsAssignableFrom(t)) })
                .Where(m => m[1] != null) ;

            foreach (var updaterPair in storeUpdaterMatches)
            {
                RegisterUpdater(updaterPair);
            }

            _logger.LogInformation("Updater scheduler ready.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Updater scheduler stopped!");

            _updaterTimers.Clear();

            return Task.CompletedTask;
        }

        private void RegisterUpdater(Type[] updaterPair)
        {
            var updater = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;
            var updaterHistory = _updaterSchedulerRepository.GetUpdaterHistoryByServiceName(updater.ServiceName);

            if (_updaterTimers.ContainsKey(updater.ServiceName))
                return;

            var remainingInterval = TimeSpan.Zero;
            if (updaterHistory?.LastUpdateDate != null)
            {
                var offset = updaterHistory.LastUpdateDate.Add(updater.UpdateInterval) - DateTime.UtcNow;
                if (offset.TotalMilliseconds > 0)
                {
                    remainingInterval = offset;
                }
            }

            var timer = new Timer(HandleTimer, updaterPair, remainingInterval, updater.UpdateInterval);
            _updaterTimers.Add(updater.ServiceName, timer);
        }

        private async void HandleTimer(object stateInfo)
        {
            if (_isWorking)
            {
                _pendingWork.Add(stateInfo);
                return;
            }

            _isWorking = true;

            var updaterPair = stateInfo as Type[];

            var updaterService = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;

            _logger.LogInformation($"Updating {updaterService.ServiceName}.");

            try
            {
                await updaterService.RefreshStore();

                _logger.LogInformation($"Update complete for {updaterService.ServiceName}.");

                var dataModel = new UpdaterScheduler { Id = updaterService.ServiceName, LastUpdateDate = DateTime.UtcNow };
                await _updaterSchedulerRepository.UpsertAsync(dataModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update failed for {updaterService.ServiceName}: {ex}");
            }
            finally
            {
                _isWorking = false;

                if (_pendingWork.Count > 0)
                {
                    var pendingWork = _pendingWork[0];
                    _pendingWork.RemoveAt(0);
                    HandleTimer(pendingWork);
                }
            }
        }
    }
}
