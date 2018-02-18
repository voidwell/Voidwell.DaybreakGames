using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            var states = new Dictionary<string, bool>();

            var characterUpdaterState = _characterUpdaterService.GetStatus(CancellationToken.None);
            var websocketMonitorState = _websocketMonitor.GetStatus(CancellationToken.None);

            await Task.WhenAll(characterUpdaterState, websocketMonitorState);

            states.Add(_characterUpdaterService.ServiceName, characterUpdaterState.Result);
            states.Add(_websocketMonitor.ServiceName, websocketMonitorState.Result);

            return Ok(states);
        }

        [HttpGet("{service}/status")]
        public async Task<ActionResult> GetServiceStatus(string service)
        {
            bool status = false;

            if (service.ToLower() == _characterUpdaterService.ServiceName.ToLower())
            {
                status = await _characterUpdaterService.GetStatus(CancellationToken.None);
            }
            else if (service.ToLower() == _websocketMonitor.ServiceName.ToLower())
            {
                status = await _websocketMonitor.GetStatus(CancellationToken.None);
            }

            return Ok(status);
        }

        [HttpPost("{service}/enable")]
        public async Task<ActionResult> PostEnableService(string service)
        {
            if (service.ToLower() == _characterUpdaterService.ServiceName.ToLower())
            {
                await _characterUpdaterService.StartAsync(CancellationToken.None);
            }
            else if (service.ToLower() == _websocketMonitor.ServiceName.ToLower())
            {
                await _websocketMonitor.StartAsync(CancellationToken.None);
            }

            return NoContent();
        }

        [HttpPost("{service}/disable")]
        public async Task<ActionResult> PostDisableService(string service)
        {
            if (service.ToLower() == _characterUpdaterService.ServiceName.ToLower())
            {
                await _characterUpdaterService.StopAsync(CancellationToken.None);
            }
            else if (service.ToLower() == _websocketMonitor.ServiceName.ToLower())
            {
                await _websocketMonitor.StopAsync(CancellationToken.None);
            }

            return NoContent();
        }
    }
}
