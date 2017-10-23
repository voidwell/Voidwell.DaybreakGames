using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Websocket;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/monitor")]
    public class MonitorController : Controller
    {
        private readonly IWebsocketMonitor _websocketMonitor;

        public MonitorController(IWebsocketMonitor websocketMonitor)
        {
            _websocketMonitor = websocketMonitor;
        }

        [HttpGet("status")]
        public async Task<ActionResult> IsRunning()
        {
            var status = _websocketMonitor.IsRunning();
            return Ok(status);
        }

        [HttpPost("start")]
        public async Task<ActionResult> PostStart()
        {
            await _websocketMonitor.StartAsync(CancellationToken.None);
            return NoContent();
        }

        [HttpPost("stop")]
        public async Task<ActionResult> PostStop()
        {
            await _websocketMonitor.StopAsync(CancellationToken.None);
            return NoContent();
        }

        [HttpPost("stop/force")]
        public async Task<ActionResult> PostForceStop()
        {
            await _websocketMonitor.ForceStopAsync(CancellationToken.None);
            return NoContent();
        }
    }
}
