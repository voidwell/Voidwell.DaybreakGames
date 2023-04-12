using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/zone")]
    public class ZoneController : Controller
    {
        private readonly IZoneStore _zoneStore;

        public ZoneController(IZoneStore zoneStore)
        {
            _zoneStore = zoneStore;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllZones()
        {
            var result = await _zoneStore.GetAllZones();
            return Ok(result);
        }
    }
}
