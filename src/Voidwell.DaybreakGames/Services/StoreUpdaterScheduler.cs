using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services
{
    public class StoreUpdaterScheduler : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StoreUpdaterScheduler> _logger;
        private readonly Func<PS2DbContext> _dbContextFactory;
        private Dictionary<string, Timer> _updaterTimers = new Dictionary<string, Timer>();

        public StoreUpdaterScheduler(Func<PS2DbContext> dbContextFactory, IServiceProvider serviceProvider, ILogger<StoreUpdaterScheduler> logger)
        {
            _dbContextFactory = dbContextFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var updatableTypes = typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a));

            var storeUpdaterInterfaces = updatableTypes.Where(a => a.GetTypeInfo().IsInterface && !typeof(IUpdateable).IsEquivalentTo(a));
            var storeUpdaterTypes = updatableTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract);

            var storeUpdaterMatches = storeUpdaterTypes.Select(t => new[] { t, storeUpdaterInterfaces.SingleOrDefault(i => i.IsAssignableFrom(t)) })
                .Where(m => m[1] != null) ;

            var dbContext = _dbContextFactory();
            foreach (var updaterPair in storeUpdaterMatches)
            {
                RegisterUpdater(updaterPair, dbContext);
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

        private void RegisterUpdater(Type[] updaterPair, PS2DbContext dbContext)
        {
            var updater = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;
            var updaterHistory = dbContext.UpdaterScheduler.SingleOrDefault(u => u.ServiceName == updater.ServiceName);

            if (_updaterTimers.ContainsKey(updater.ServiceName))
                return;

            TimeSpan remainingInterval = TimeSpan.Zero;
            if (updaterHistory != null && updaterHistory.LastUpdateDate != null)
            {
                var offset = updaterHistory.LastUpdateDate.Add(updater.UpdateInterval) - DateTime.Now;
                if (offset.TotalMilliseconds > 0)
                {
                    remainingInterval = offset;
                }
            }

            Timer timer = new Timer(HandleTimer, updaterPair, remainingInterval, updater.UpdateInterval);
            _updaterTimers.Add(updater.ServiceName, timer);
        }

        private async void HandleTimer(Object stateInfo)
        {
            var updaterPair = stateInfo as Type[];

            var updaterService = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;

            _logger.LogInformation($"Updating {updaterService.ServiceName}.");

            await updaterService.RefreshStore();

            _logger.LogInformation($"Update complete for {updaterService.ServiceName}.");

            var dbContext = _dbContextFactory();
            var dataModel = new DbPS2UpdaterScheduler { ServiceName = updaterService.ServiceName, LastUpdateDate = DateTime.Now };
            await dbContext.UpdaterScheduler.UpsertAsync(dataModel);
            await dbContext.SaveChangesAsync();
        }
    }
}
