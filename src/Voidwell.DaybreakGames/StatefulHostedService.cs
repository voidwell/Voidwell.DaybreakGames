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

        public StatefulHostedService(ICache cache)
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
            await UpdateState(true);
            await StartInternalAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await UpdateState(false);
            await StopInternalAsync(cancellationToken);
        }
        
        public virtual Task<ServiceState> GetStatus(CancellationToken cancellationToken)
        {
            var state = new ServiceState
            {
                Name = ServiceName,
                IsEnabled = _isRunning
            };
            return Task.FromResult(state);
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);

        private async Task UpdateState(bool isEnabled)
        {
            _isRunning = isEnabled;
            var state = await GetStatus(CancellationToken.None);
            await _cache.SetAsync(GetCacheKey(), state);
        }

        private string GetCacheKey()
        {
            return $"service-{ServiceName}-state";
        }
    }
}
