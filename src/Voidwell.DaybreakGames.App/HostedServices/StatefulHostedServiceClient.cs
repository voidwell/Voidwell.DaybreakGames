using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Voidwell.DaybreakGames.HostedServices
{
    public abstract class StatefulHostedServiceClient : IHostedService
    {
        private IStatefulHostedService _hostedService { get; set; }

        public StatefulHostedServiceClient(IStatefulHostedService hostedService)
        {
            _hostedService = hostedService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _hostedService.OnApplicationStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _hostedService.OnApplicationShutdown(cancellationToken);
        }
    }
}
