using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/character")]
    public class CharacterController : Controller
    {
        private readonly ICharacterService _characterService;
        private readonly ICharacterSessionService _characterSessionService;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly ICharacterDirectiveService _characterDirectiveService;

        public CharacterController(ICharacterService characterService, ICharacterSessionService characterSessionService,
            IPlayerMonitor playerMonitor, ICharacterDirectiveService characterDirectiveService)
        {
            _characterService = characterService;
            _characterSessionService = characterSessionService;
            _playerMonitor = playerMonitor;
            _characterDirectiveService = characterDirectiveService;
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
        public async Task<ActionResult> GetCharacterSessionsByCharacterId(string characterId, [FromQuery]int page = 0)
        {
            var result = await _characterSessionService.GetSessions(characterId, page: page);
            return Ok(result);
        }

        [HttpGet("{characterId}/sessions/{sessionId}")]
        public async Task<ActionResult> GetCharacterSessionBySessionId(string characterId, int sessionId)
        {
            var result = await _characterSessionService.GetSession(characterId, sessionId);
            return Ok(result);
        }

        [HttpGet("{characterId}/sessions/live")]
        public async Task<ActionResult> GetCharacterLiveSession(string characterId)
        {
            var result = await _characterSessionService.GetSession(characterId);
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

        [HttpGet("{characterId}/directives")]
        public async Task<ActionResult> GetDirectivesByCharacterId(string characterId)
        {
            var data = await _characterDirectiveService.GetCharacterDirectivesAsync(characterId);
            if (data == null)
            {
                return NotFound($"Unable to find directives for character : '{characterId}'");
            }

            return Ok(data);
        }
    }
}
