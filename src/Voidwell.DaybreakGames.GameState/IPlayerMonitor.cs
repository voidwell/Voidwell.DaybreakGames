using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.GameState
{
    public interface IPlayerMonitor
    {
        Task<OnlineCharacter> SetCharacterOnlineAsync(string characterId, int worldId, int factionId, string outfitId, DateTime timestamp);
        Task<OnlineCharacter> SetCharacterOfflineAsync(string characterId, DateTime timestamp);
        Task<OnlineCharacter> UpdateLastSeenAsync(string characterId, int worldId, int factionId, string outfitId, int zoneId, DateTime timestamp);
        Task<IEnumerable<OnlineCharacter>> GetOnlineCharactersAsync(int worldId, int? zoneId = null);
        Task<bool> IsCharacterOnlineAsync(string characterId);
        Task ClearAllCharactersAsync(int worldId);
        Task<long> GetPlayerCountAsync(int worldId);
    }
}