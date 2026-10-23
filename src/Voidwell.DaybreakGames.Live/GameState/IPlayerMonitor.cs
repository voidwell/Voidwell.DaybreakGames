using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Live.GameState;

public interface IPlayerMonitor
{
    Task<OnlineCharacter?> SetOnlineAsync(long characterId, DateTime timestamp);
    Task<OnlineCharacter?> SetOfflineAsync(long characterId, DateTime timestamp);
    Task<OnlineCharacter?> SetLastSeenAsync(long characterId, int zoneId, DateTime timestamp);
    Task<IEnumerable<OnlineCharacter>?> GetAllAsync(int worldId, int? zoneId = null);
    Task<OnlineCharacter> GetAsync(long characterId);
    Task<long> GetPlayerCountAsync(int worldId);
    Task ClearWorldAsync(int worldId);
}
