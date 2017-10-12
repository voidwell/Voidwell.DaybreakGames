using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class OnlineCharacterService : IOnlineCharacterService, IDisposable
    {
        private readonly ICharacterService _characterService;
        private readonly IUpdaterService _updaterService;
        private readonly PS2DbContext _ps2DbContext;
        private readonly List<OnlineCharacter> _onlineCharacters;

        public OnlineCharacterService(PS2DbContext ps2DbContext, ICharacterService characterService, IUpdaterService updaterService)
        {
            _ps2DbContext = ps2DbContext;
            _characterService = characterService;
            _updaterService = updaterService;

            _onlineCharacters = new List<OnlineCharacter>();
        }

        public IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(string worldId)
        {
            return _onlineCharacters.Where(c => c.Character.WorldId == worldId);
        }

        public async Task SetCharacterOnline(string characterId, DateTime loginDate)
        {
            var character = await _characterService.GetCharacter(characterId);

            if (character == null)
            {
                return;
            }

            _onlineCharacters.Add(new OnlineCharacter
            {
                Character = new OnlineCharacterProfile
                {
                    CharacterId = character.Id,
                    FactionId = character.FactionId,
                    Name = character.Name,
                    WorldId = character.WorldId
                },
                LoginDate = loginDate
            });
        }

        public async Task SetCharacterOffline(string characterId, DateTime logoutDate)
        {
            await _characterService.GetCharacter(characterId);

            var onlineCharacter = _onlineCharacters.SingleOrDefault(c => c.Character.CharacterId == characterId);
            if (onlineCharacter == null)
            {
                return;
            }

            var duration = logoutDate - onlineCharacter.LoginDate;

            if (duration.Minutes >= 300000)
            {
                await _updaterService.AddToQueue(characterId);
            }

            _ps2DbContext.PlayerSessions.Add(new DbPlayerSession
            {
                CharacterId = characterId,
                LoginDate = onlineCharacter.LoginDate,
                LogoutDate = logoutDate,
                Duration = duration.Milliseconds
            });
            await _ps2DbContext.SaveChangesAsync();

            _onlineCharacters.Remove(onlineCharacter);
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
