using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames
{
    public interface IHostedServiceExtended : IHostedService
    {
        Task ForceStopAsync(CancellationToken cancellationToken);
        bool IsRunning();
    }
}
