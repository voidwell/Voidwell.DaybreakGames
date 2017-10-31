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
            var result = await _outfitService.GetOutfitFull(outfitId);
            return Ok(result);
        }

        [HttpGet("{outfitId}/members")]
        public async Task<ActionResult> GetOutfitMembers(string outfitId)
        {
            var result = await _outfitService.GetOutfitMembers(outfitId);
            return Ok(result);
        }
    }
}