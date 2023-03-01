using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/directive")]
    public class DirectiveController : Controller
    {
        private readonly IDirectiveService _directiveService;

        public DirectiveController(IDirectiveService directiveService)
        {
            _directiveService = directiveService;
        }

        [HttpGet("outline/{factionId}")]
        public async Task<ActionResult> GetDirectiveOutlineByFactionId(int factionId)
        {
            var data = await _directiveService.GetDirectivesOutlineAsync(factionId);
            if (data == null)
            {
                return NotFound($"Unable to find directives for faction id: '{factionId}'");
            }

            return Ok(data);
        }

        [HttpGet("character/{characterId}")]
        public async Task<ActionResult> GetDirectivesByCharacterId(string characterId)
        {
            var data = await _directiveService.GetCharacterDirectivesAsync(characterId);
            if (data == null)
            {
                return NotFound($"Unable to find directives for character : '{characterId}'");
            }

            return Ok(data);
        }

    }
}
