using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/combatReport")]
    public class CombatReportController : Controller
    {
        private readonly ICombatReportService _combatReportService;

        public CombatReportController(ICombatReportService combatReportService)
        {
            _combatReportService = combatReportService;
        }

        [HttpGet("{worldId}/{zoneId}/{startDate}/{endDate}")]
        public async Task<ActionResult> GetCombatReport(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            var result = await _combatReportService.GetCombatReport(worldId, zoneId, startDate, endDate);
            return Ok(result);
        }
    }
}
