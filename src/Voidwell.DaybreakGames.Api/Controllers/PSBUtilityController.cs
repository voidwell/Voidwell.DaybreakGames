using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services;

namespace Voidwell.DaybreakGames.Api.Controllers
{
    [Route("psb")]
    public class PSBUtilityController : Controller
    {
        private readonly IPSBUtilityService _psbUtilityService;

        public PSBUtilityController(IPSBUtilityService psbUtilityService)
        {
            _psbUtilityService = psbUtilityService;
        }

        [HttpGet("sessions")]
        public async Task<ActionResult> GetLastOnlinePSBAccounts()
        {
            var results = await _psbUtilityService.GetLastOnlinePSBAccounts();
            return Ok(results);
        }
    }
}
