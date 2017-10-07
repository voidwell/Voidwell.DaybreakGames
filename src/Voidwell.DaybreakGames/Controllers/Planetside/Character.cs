using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/character")]
    public class Character : Controller
    {
        [HttpGet("search/{query}")]
        public async Task<ActionResult> LookupCharactersByName(string query)
        {
            var result = await CharacterService.LookupCharactersByName(query);
            return Ok(result);
        }

        [HttpGet("{characterId}")]
        public async Task<ActionResult> GetCharacterById(string characterId)
        {
            var result = await CharacterService.GetCharacterById(characterId);
            return Ok(result);
        }
    }
}
