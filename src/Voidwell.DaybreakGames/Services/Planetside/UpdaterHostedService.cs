using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class UpdaterHostedService : IHostedService
    {
        private readonly IUpdaterService _updaterService;

        public UpdaterHostedService(IUpdaterService updaterService)
        {
            _updaterService = updaterService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _updaterService.Startup();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
