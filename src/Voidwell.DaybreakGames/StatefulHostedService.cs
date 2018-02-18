using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames
{
    public abstract class StatefulHostedService : IStatefulHostedService
    {
        protected bool _isRunning { get; set; }
        private ICache _cache { get; set; }

        public abstract string ServiceName { get; }

        public StatefulHostedService(ICache cache)
        {
            _cache = cache;
        }

        public virtual async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            var state = await _cache.GetAsync<bool?>(GetCacheKey());
            if (state != null && state.Value)
            {
                await StartInternalAsync(cancellationToken);
            }
        }

        public virtual Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            await StartInternalAsync(cancellationToken);
            await _cache.SetAsync(GetCacheKey(), true);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            await StopInternalAsync(cancellationToken);
            await _cache.SetAsync(GetCacheKey(), false);
        }
        
        public virtual Task<bool> GetStatus(CancellationToken cancellationToken)
        {
            return Task.FromResult(_isRunning);
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);

        private string GetCacheKey()
        {
            return $"service-{ServiceName}-state";
        }
    }
}
