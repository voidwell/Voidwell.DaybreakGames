using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames
{
    public interface IHostedService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task ForceStopAsync(CancellationToken cancellationToken);
        bool IsRunning();
    }
}
