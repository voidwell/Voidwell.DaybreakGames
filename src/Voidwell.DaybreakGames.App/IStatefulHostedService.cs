using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames
{
    public interface IStatefulHostedService
    {
        Task OnApplicationStartup(CancellationToken cancellationToken);
        Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<ServiceState> GetStateAsync(CancellationToken cancellationToken);
        string ServiceName { get; }
    }
}
