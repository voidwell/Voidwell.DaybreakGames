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
        public bool IsRunning()
        {
            return _websocketMonitor.IsRunning();
        }

        [HttpPost("start")]
        public async Task<ActionResult> PostStart()
        {
            await _websocketMonitor.StartAsync();
            return NoContent();
        }

        [HttpPost("stop")]
        public async Task<ActionResult> PostStop()
        {
            await _websocketMonitor.StopAsync();
            return NoContent();
        }
    }
}
