using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Voidwell.DaybreakGames.Utils;
using Newtonsoft.Json.Linq;
using Voidwell.DaybreakGames.App.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Voidwell.DaybreakGames.Api.Controllers
{
    [Route("services")]
    public class ServicesController : Controller
    {
        private readonly IEnumerable<IStatefulHostedService> _services;

        public ServicesController(IServiceProvider serviceProvider)
        {
            _services = serviceProvider.GetServices<IHostedService>()
                .Where(a => a is StatefulHostedServiceClient)
                .Cast<StatefulHostedServiceClient>()
                .Select(a => a.Service);
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetAllServiceStatus()
        {
            var serviceStateTasks = _services?.Select(a => a.GetStateAsync(CancellationToken.None));

            var states = await Task.WhenAll(serviceStateTasks);

            return Ok(states);
        }

        [HttpGet("{serviceName}/status")]
        public async Task<ActionResult> GetServiceStatus(string serviceName)
        {
            var service = _services?.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return NotFound();
            }

            var status = await service.GetStateAsync(CancellationToken.None);

            return Ok(status);
        }

        [HttpPost("{serviceName}/enable")]
        public async Task<ActionResult> PostEnableService(string serviceName)
        {
            var service = _services?.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return NotFound();
            }

            await service.StartAsync(CancellationToken.None);

            var status = await service.GetStateAsync(CancellationToken.None);

            return Ok(status);
        }

        [HttpPost("{serviceName}/disable")]
        public async Task<ActionResult> PostDisableService(string serviceName)
        {
            var service = _services?.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return NotFound();
            }

            await service.StopAsync(CancellationToken.None);

            var status = await service.GetStateAsync(CancellationToken.None);

            return Ok(status);
        }
    }
}
