using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/outfit")]
    public class OutfitController : Controller
    {
        private readonly IOutfitService _outfitService;

        public OutfitController(IOutfitService outfitService)
        {
            _outfitService = outfitService;
        }

        [HttpGet("{outfitId}")]
        public async Task<ActionResult> GetOutfit(string outfitId)
        {
            var details = await _outfitService.GetOutfitDetails(outfitId);
            if (details == null)
            {
                return NotFound($"Unable to find outfit with id: '{outfitId}'");
            }

            return Ok(details);
        }

        [HttpGet("{outfitId}/members")]
        public async Task<ActionResult> GetOutfitMembers(string outfitId)
        {
            var result = await _outfitService.GetOutfitMembers(outfitId);
            return Ok(result);
        }

        [HttpGet("byalias/{outfitAlias}")]
        public async Task<ActionResult> GetOutfitByAlias(string outfitAlias)
        {
            var result = await _outfitService.GetOutfitByAlias(outfitAlias);
            if (result == null)
            {
                return NotFound($"Unable to find stats with outfit: '{outfitAlias}'");
            }

            return Ok(result);
        }
    }
}