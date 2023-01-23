using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Live.GameState
{
    public interface IPlayerMonitor
    {
        Task<OnlineCharacter> SetOnlineAsync(string characterId, DateTime timestamp);
        Task<OnlineCharacter> SetOfflineAsync(string characterId, DateTime timestamp);
        Task<OnlineCharacter> SetLastSeenAsync(string characterId, int zoneId, DateTime timestamp);
        Task<IEnumerable<OnlineCharacter>> GetAllAsync(int worldId, int? zoneId = null);
        Task<OnlineCharacter> GetAsync(string characterId);
        Task<long> GetPlayerCountAsync(int worldId);
        Task ClearWorldAsync(int worldId);
    }
}
