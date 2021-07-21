using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;

namespace Voidwell.DaybreakGames.Api.Controllers
{
    [Route("store")]
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
        public async Task<ActionResult> PostForceUpdateStore(string storeName)
        {
            var result = await _storeUpdaterService.UpdateStoreAsync(storeName);

            return Ok(result);
        }
    }
}
