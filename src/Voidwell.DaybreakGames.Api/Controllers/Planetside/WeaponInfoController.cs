using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/weaponInfo")]
    public class WeaponInfoController : Controller
    {
        private readonly IWeaponService _weaponService;

        public WeaponInfoController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpGet("{weaponItemId}")]
        public async Task<ActionResult> GetWeaponInfo(int weaponItemId)
        {
            var result = await _weaponService.GetWeaponInfo(weaponItemId);
            return Ok(result);
        }
    }
}
