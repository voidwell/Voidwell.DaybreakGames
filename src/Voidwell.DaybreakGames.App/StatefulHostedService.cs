using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames
{
    public abstract class StatefulHostedService : IStatefulHostedService
    {
        protected bool _isRunning { get; set; } = false;
        private ICache _cache { get; set; }

        public abstract string ServiceName { get; }

        protected StatefulHostedService(ICache cache)
        {
            _cache = cache;
        }

        public virtual async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            var state = await _cache.GetAsync<ServiceState>(GetCacheKey());
            if (state != null && state.IsEnabled)
            {
                _isRunning = true;
                await StartInternalAsync(cancellationToken);
            }
        }

        public virtual Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(true);
            await StartInternalAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(false);
            await StopInternalAsync(cancellationToken);
        }
        
        public async Task<ServiceState> GetStateAsync(CancellationToken cancellationToken)
        {
            var details = await GetStatusAsync(cancellationToken);

            return new ServiceState
            {
                Name = ServiceName,
                IsEnabled = _isRunning,
                Details = details
            };
        }

        protected async Task UpdateStateAsync(bool isEnabled)
        {
            _isRunning = isEnabled;
            var state = await GetStateAsync(CancellationToken.None);
            await _cache.SetAsync(GetCacheKey(), state);
        }

        protected virtual Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(null as object);
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);

        private string GetCacheKey()
        {
            return $"service-{ServiceName}-state";
        }
    }
}
