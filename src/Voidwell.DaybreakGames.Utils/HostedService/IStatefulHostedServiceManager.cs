using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public interface IStatefulHostedServiceManager : IHostedService
    {
        Task<IEnumerable<ServiceState>> GetServiceStatusAsync(CancellationToken cancellationToken);
        Task<ServiceState> GetServiceStatusAsync(string serviceName, CancellationToken cancellationToken);
        Task StartServiceAsync(string serviceName, CancellationToken cancellationToken);
        Task StopServiceAsync(string serviceName, CancellationToken cancellationToken);
        bool VerifyServiceExists(string serviceName);
    }
}