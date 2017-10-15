using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> GetStatus()
        {
            var status = _websocketMonitor.GetStatus();
            return Ok(status);
        }

        [HttpPost("start")]
        public async Task<ActionResult> PostStart()
        {
            await _websocketMonitor.StartMonitor();
            return NoContent();
        }

        [HttpPost("stop")]
        public async Task<ActionResult> PostStop()
        {
            await _websocketMonitor.StopMonitor();
            return NoContent();
        }

        [HttpPost("reset")]
        public async Task<ActionResult> PostReset()
        {
            await _websocketMonitor.ResetMonitor();
            return NoContent();
        }
    }
}
