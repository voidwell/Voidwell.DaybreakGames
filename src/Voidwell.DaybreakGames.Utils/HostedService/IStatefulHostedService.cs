using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public interface IStatefulHostedService : IHostedService
    {
        Task OnApplicationStartup(CancellationToken cancellationToken);
        Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task<object> GetStatusAsync(CancellationToken cancellationToken);
    }
}
