using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

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

        [HttpGet("byname/{weaponName}")]
        public async Task<ActionResult> GetWeaponInfoByName(string weaponName)
        {
            var result = await _weaponService.GetWeaponInfoByName(weaponName);
            if (result == null)
            {
                return NotFound($"Unable to find weapon info for '{weaponName}'");
            }

            return Ok(result);
        }
    }
}
