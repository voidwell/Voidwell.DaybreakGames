using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/alert")]
    public class AlertController : Controller
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet("{worldId}/{instanceId}")]
        public async Task<ActionResult> GetAlert(string worldId, string instanceId)
        {
            var result = await _alertService.GetAlert(worldId, instanceId);
            return Ok(result);
        }

        [HttpGet("{worldId}")]
        public async Task<ActionResult> GetAlertsByWorldId(string worldId)
        {
            var result = await _alertService.GetAlerts(worldId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAlerts()
        {
            var result = await _alertService.GetAllAlerts();
            return Ok(result);
        }
    }
}
