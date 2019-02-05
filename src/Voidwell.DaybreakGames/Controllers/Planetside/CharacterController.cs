using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/character")]
    public class CharacterController : Controller
    {
        private readonly ICharacterService _characterService;
        private readonly IPlayerMonitor _playerMonitor;

        public CharacterController(ICharacterService characterService, IPlayerMonitor playerMonitor)
        {
            _characterService = characterService;
            _playerMonitor = playerMonitor;
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

        [HttpGet("{characterId}/state")]
        public async Task<ActionResult> GetCharacterOnlineState(string characterId)
        {
            var result = await _playerMonitor.GetAsync(characterId);
            return Ok(result);
        }

        [HttpGet("byname/{characterName}")]
        public async Task<ActionResult> GetCharacterStatsByName(string characterName)
        {
            var result = await _characterService.GetCharacterByName(characterName);
            if (result == null)
            {
                return NotFound($"Unable to find stats with character: '{characterName}'");
            }

            return Ok(result);
        }

        [HttpGet("byname/{characterName}/weapon/{weaponName}")]
        public async Task<ActionResult> GetCharacterWeaponStatsByName(string characterName, string weaponName)
        {
            var result = await _characterService.GetCharacterWeaponByName(characterName, weaponName);
            if (result == null)
            {
                return NotFound($"Unable to find character weapon stats with character: '{characterName}' or weapon '{weaponName}'");
            }

            return Ok(result);
        }

        [HttpPost("byname")]
        public async Task<ActionResult> GetMultipleCharacterStatsByName([FromBody] IEnumerable<string> characterNames)
        {
            var result = await _characterService.GetCharactersByName(characterNames.Take(25).ToArray());

            return Ok(result);
        }
    }
}
