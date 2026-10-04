using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/weaponInfo")]
public class WeaponInfoController : Controller
{
    private readonly IWeaponService _weaponService;

    public WeaponInfoController(IWeaponService weaponService)
    {
        _weaponService = weaponService;
    }

    [HttpGet("{weaponItemId}")]
    public async Task<ActionResult> GetWeaponInfoAsync(int weaponItemId)
    {
        var result = await _weaponService.GetWeaponInfo(weaponItemId);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(AuthConstants.Policies.Mutterblack)]
    [HttpGet("byname/{weaponName}")]
    public async Task<ActionResult> GetWeaponInfoByNameAsync(string weaponName)
    {
        var result = await _weaponService.GetWeaponInfoByName(weaponName);
        if (result == null)
        {
            return NotFound($"Unable to find weapon info for '{weaponName}'");
        }

        return Ok(result);
    }
}
