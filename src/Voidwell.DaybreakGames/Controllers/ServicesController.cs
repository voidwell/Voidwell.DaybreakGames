using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket;

namespace Voidwell.DaybreakGames.Controllers
{
    [Route("services")]
    public class ServicesController : Controller
    {
        private readonly IWebsocketMonitor _websocketMonitor;
        private readonly ICharacterUpdaterService _characterUpdaterService;

        public ServicesController(IWebsocketMonitor websocketMonitor, ICharacterUpdaterService characterUpdaterService)
        {
            _websocketMonitor = websocketMonitor;
            _characterUpdaterService = characterUpdaterService;
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetAllServiceStatus()
        {
            var states = new List<ServiceState>();

            var characterUpdaterState = _characterUpdaterService.GetStatus(CancellationToken.None);
            var websocketMonitorState = _websocketMonitor.GetStatus(CancellationToken.None);

            await Task.WhenAll(characterUpdaterState, websocketMonitorState);

            states.Add(characterUpdaterState.Result);
            states.Add(websocketMonitorState.Result);

            return Ok(states);
        }

        [HttpGet("{service}/status")]
        public async Task<ActionResult> GetServiceStatus(string service)
        {
            ServiceState status = null;

            if (service == _characterUpdaterService.ServiceName)
            {
                status = await _characterUpdaterService.GetStatus(CancellationToken.None);
            }
            else if (service == _websocketMonitor.ServiceName)
            {
                status = await _websocketMonitor.GetStatus(CancellationToken.None);
            }

            if (status != null)
            {
                return Ok(status);
            }

            return NotFound();
        }

        [HttpPost("{service}/enable")]
        public async Task<ActionResult> PostEnableService(string service)
        {
            if (service == _characterUpdaterService.ServiceName)
            {
                await _characterUpdaterService.StartAsync(CancellationToken.None);
                return Ok(await _characterUpdaterService.GetStatus(CancellationToken.None));
            }
            else if (service == _websocketMonitor.ServiceName)
            {
                await _websocketMonitor.StartAsync(CancellationToken.None);
                return Ok(await _websocketMonitor.GetStatus(CancellationToken.None));
            }

            return NotFound();
        }

        [HttpPost("{service}/disable")]
        public async Task<ActionResult> PostDisableService(string service)
        {
            if (service == _characterUpdaterService.ServiceName)
            {
                await _characterUpdaterService.StopAsync(CancellationToken.None);
                return Ok(await _characterUpdaterService.GetStatus(CancellationToken.None));
            }
            else if (service == _websocketMonitor.ServiceName)
            {
                await _websocketMonitor.StopAsync(CancellationToken.None);
                return Ok(await _websocketMonitor.GetStatus(CancellationToken.None));
            }

            return NotFound();
        }
    }
}
