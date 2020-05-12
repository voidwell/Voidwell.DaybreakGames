using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Utils.StatefulHostedService
{
    public interface IStatefulHostedService  : IHostedService
    {
        Task OnApplicationStartup(CancellationToken cancellationToken);
        Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task<ServiceState> GetStateAsync(CancellationToken cancellationToken);
        string ServiceName { get; }
    }
}
