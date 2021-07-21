using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public class StoreUpdaterService : IStoreUpdaterService
    {
        private readonly IUpdaterSchedulerRepository _updaterSchedulerRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StoreUpdaterService> _logger;

        private readonly Dictionary<string, Type> _updaterServiceTypes = new Dictionary<string, Type>();
        private readonly Dictionary<string, LastStoreUpdate> _updateLog = new Dictionary<string, LastStoreUpdate>();

        private readonly SemaphoreSlim _updateLock = new SemaphoreSlim(1);

        public StoreUpdaterService(IUpdaterSchedulerRepository updaterSchedulerRepository, IServiceProvider serviceProvider, ILogger<StoreUpdaterService> logger)
        {
            _updaterSchedulerRepository = updaterSchedulerRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;

            LoadUpdaterServiceTypes();
        }

        public IEnumerable<LastStoreUpdate> GetStoreUpdateLog()
        {
            return _updateLog.Values.OrderBy(a => a.StoreName).ToList();
        }

        public async Task<LastStoreUpdate> UpdateStoreAsync(string storeName)
        {
            await _updateLock.WaitAsync();

            _logger.LogInformation($"Updating {storeName}.");

            try
            {
                var updaterServiceType = _updaterServiceTypes[storeName];
                var updaterService = _serviceProvider.GetRequiredService(updaterServiceType) as IUpdateable;

                await updaterService.RefreshStore();

                _logger.LogInformation($"Update complete for {updaterService.StoreName}.");

                var dataModel = new UpdaterScheduler { Id = updaterService.StoreName, LastUpdateDate = DateTime.UtcNow };
                await _updaterSchedulerRepository.UpsertAsync(dataModel);

                UpdateServiceLog(updaterService, dataModel.LastUpdateDate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update failed for {storeName}: {ex}");
            }
            finally
            {
                _updateLock.Release();
            }

            return _updateLog[storeName];
        }

        private void LoadUpdaterServiceTypes()
        {
            var updatableTypes = typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a));

            var storeUpdaterInterfaces = updatableTypes.Where(a => a.GetTypeInfo().IsInterface && !typeof(IUpdateable).IsEquivalentTo(a));
            var storeUpdaterTypes = updatableTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract);

            var storeUpdaterMatches = storeUpdaterTypes.Select(t => storeUpdaterInterfaces.SingleOrDefault(i => i.IsAssignableFrom(t)))
                .Where(a => a != null);

            foreach (var storeUpdaterType in storeUpdaterMatches)
            {
                var service = _serviceProvider.GetRequiredService(storeUpdaterType) as IUpdateable;
                var updaterHistory = _updaterSchedulerRepository.GetUpdaterHistoryByServiceName(service.StoreName);

                _updaterServiceTypes[service.StoreName] = storeUpdaterType;
                UpdateServiceLog(service, updaterHistory?.LastUpdateDate);
            }
        }

        private void UpdateServiceLog(IUpdateable service, DateTime? updateTime)
        {
            _updateLog[service.StoreName] = new LastStoreUpdate
            {
                StoreName = service.StoreName,
                LastUpdated = updateTime,
                UpdateInterval = service.UpdateInterval
            };
        }
    }
}
