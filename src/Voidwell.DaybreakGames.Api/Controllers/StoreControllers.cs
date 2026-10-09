using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;

namespace Voidwell.DaybreakGames.Api.Controllers;

[Route("ps2/store")]
[Authorize(Roles = AuthConstants.Roles.Administrator)]
public class StoreController : Controller
{
    private readonly IStoreUpdaterService _storeUpdaterService;

    public StoreController(IStoreUpdaterService storeUpdaterService)
    {
        _storeUpdaterService = storeUpdaterService;
    }

    [HttpGet("updatelog")]
    public ActionResult GetAllUpdateLogs()
    {
        var logs = _storeUpdaterService.GetStoreUpdateLog();

        return Ok(logs);
    }

    [HttpPost("update/{storeName}")]
    public async Task<ActionResult> PostForceUpdateStoreAsync(string storeName)
    {
        var result = await _storeUpdaterService.UpdateStoreAsync(storeName);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
