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

        [HttpGet("{characterId}")]
        public async Task<ActionResult> GetCharacterById(string characterId)
        {
            var details = await _characterService.GetCharacterDetails(characterId);
            if (details == null)
            {
                return NotFound($"Unable to find character with id: '{characterId}'");
            }

            return Ok(details);
        }

        [HttpGet("{characterId}/sessions")]
        public async Task<ActionResult> GetCharacterSessionsById(string characterId)
        {
            var result = await _characterService.GetSessions(characterId);
            return Ok(result);
        }

        [HttpGet("{characterId}/sessions/{sessionId}")]
        public async Task<ActionResult> GetCharacterSessionsById(string characterId, int sessionId)
        {
            var result = await _characterService.GetSession(characterId, sessionId);
            return Ok(result);
        }
    }
}
