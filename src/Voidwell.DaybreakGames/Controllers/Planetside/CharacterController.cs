using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/character")]
    public class CharacterController : Controller
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult> LookupCharactersByName(string query)
        {
            var result = await _characterService.LookupCharactersByName(query);
            return Ok(result);
        }

        [HttpGet("{characterId}")]
        public async Task<ActionResult> GetCharacterById(string characterId)
        {
            var result = await _characterService.GetCharacter(characterId);
            return Ok(result);
        }
    }
}
