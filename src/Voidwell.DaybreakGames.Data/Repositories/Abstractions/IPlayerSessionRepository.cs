using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IPlayerSessionRepository
    {
        Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit, int page = 0);
        Task<PlayerSession> GetPlayerSessionAsync(int sessionId);
        Task AddAsync(PlayerSession entity);
    }
}