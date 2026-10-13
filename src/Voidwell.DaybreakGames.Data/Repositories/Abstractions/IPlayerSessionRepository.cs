using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface IPlayerSessionRepository
{
    Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit, CancellationToken cancellationToken, int page = 0);
    Task<PlayerSession?> GetPlayerSessionAsync(int sessionId, CancellationToken cancellationToken);
    Task AddAsync(PlayerSession entity, CancellationToken cancellationToken = default);
}
