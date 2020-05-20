using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Api.Controllers
{
    [Route("services")]
    public class ServicesController : Controller
    {
        private readonly IEnumerable<IStatefulHostedService> _services;

        public ServicesController(IServiceProvider serviceProvider)
        {
            var statefulHostedTypes = typeof(IStatefulHostedService).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IStatefulHostedService).IsAssignableFrom(a) && !typeof(IStatefulHostedService).IsEquivalentTo(a));

            var statefulHostedServiceTypes = statefulHostedTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract);

            _services = statefulHostedTypes.Where(a => a.GetTypeInfo().IsInterface && statefulHostedServiceTypes.Any(a.IsAssignableFrom))
                .Select(a => serviceProvider.GetService(a) as IStatefulHostedService)
                .Where(a => a != null)
                .OrderBy(a => a.ServiceName);
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetAllServiceStatus()
        {
            var serviceStateTasks = _services.Select(a => a.GetStateAsync(CancellationToken.None));

            var states = await Task.WhenAll(serviceStateTasks);

            return Ok(states);
        }

        [HttpGet("{serviceName}/status")]
        public async Task<ActionResult> GetServiceStatus(string serviceName)
        {
            var service = _services.FirstOrDefault(a => a.ServiceName == serviceName);
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
            var service = _services.FirstOrDefault(a => a.ServiceName == serviceName);
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
            var service = _services.FirstOrDefault(a => a.ServiceName == serviceName);
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
