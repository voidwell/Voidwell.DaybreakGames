using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.App.HostedServices
{
    public abstract class StatefulHostedServiceClient : IHostedService
    {
        public IStatefulHostedService Service { get; }

        protected StatefulHostedServiceClient(IStatefulHostedService hostedService)
        {
            Service = hostedService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Service.OnApplicationStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Service.OnApplicationShutdown(cancellationToken);
        }
    }
}
