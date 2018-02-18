using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames
{
    public interface IStatefulHostedService
    {
        Task OnApplicationStartup(CancellationToken cancellationToken);
        Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<bool> GetStatus(CancellationToken cancellationToken);
        string ServiceName { get; }
    }
}
