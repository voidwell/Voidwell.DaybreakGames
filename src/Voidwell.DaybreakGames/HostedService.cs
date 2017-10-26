using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames
{
    public abstract class HostedService : IHostedServiceExtended
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            _cts.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            await CleanupAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
        }

        public virtual Task ForceStopAsync(CancellationToken cancellationToken)
        {
            return StopAsync(cancellationToken);
        }

        public bool IsRunning()
        {
            return _executingTask != null && !(_cts?.IsCancellationRequested ?? true);
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        protected virtual Task CleanupAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
