using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Api.Controllers
{
    [Route("services")]
    public class ServicesController : Controller
    {
        private readonly IStatefulHostedServiceManager _serviceManager;

        public ServicesController(IStatefulHostedServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetAllServiceStatus()
        {
            var states = await _serviceManager.GetServiceStatusAsync(CancellationToken.None);

            return Ok(states);
        }

        [HttpGet("{serviceName}/status")]
        public async Task<ActionResult> GetServiceStatus(string serviceName)
        {
            var state = await _serviceManager.GetServiceStatusAsync(serviceName, CancellationToken.None);
            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        [HttpPost("{serviceName}/enable")]
        public async Task<ActionResult> PostEnableService(string serviceName)
        {
            if (!_serviceManager.VerifyServiceExists(serviceName))
            {
                return NotFound();
            }

            await _serviceManager.StartServiceAsync(serviceName, CancellationToken.None);

            var status = await _serviceManager.GetServiceStatusAsync(serviceName, CancellationToken.None);

            return Ok(status);
        }

        [HttpPost("{serviceName}/disable")]
        public async Task<ActionResult> PostDisableService(string serviceName)
        {
            if (!_serviceManager.VerifyServiceExists(serviceName))
            {
                return NotFound();
            }

            await _serviceManager.StopServiceAsync(serviceName, CancellationToken.None);

            var status = await _serviceManager.GetServiceStatusAsync(serviceName, CancellationToken.None);

            return Ok(status);
        }
    }
}
