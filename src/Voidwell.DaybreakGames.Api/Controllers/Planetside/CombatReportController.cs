using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Api.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/combatReport")]
    public class CombatReportController : Controller
    {
        private readonly ICombatReportService _combatReportService;

        public CombatReportController(ICombatReportService combatReportService)
        {
            _combatReportService = combatReportService;
        }

        [HttpPost]
        public async Task<ActionResult> GetCombatReport([FromBody]CombatReportRequest request)
        {
            var result = await _combatReportService.GetCombatReport(request.WorldId, request.ZoneId, request.StartDate, request.EndDate);
            return Ok(result);
        }
    }
}
