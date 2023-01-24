using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public class StatefulHostedServiceWrapper<TService> : IHostedService
        where TService : class, IStatefulHostedService
    {
        private readonly TService _service;

        public StatefulHostedServiceWrapper(TService service)
        {
            _service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationShutdown(cancellationToken);
        }
    }
}
