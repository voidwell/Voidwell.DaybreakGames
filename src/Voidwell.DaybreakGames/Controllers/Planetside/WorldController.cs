using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/world")]
    public class WorldController : Controller
    {
        private readonly IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllWorlds ()
        {
            var result = await _worldService.GetAllWorlds();
            return Ok(result);
        }
    }
}
