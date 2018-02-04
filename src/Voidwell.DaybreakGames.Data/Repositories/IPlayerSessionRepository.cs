using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IPlayerSessionRepository
    {
        Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit);
        Task<PlayerSession> GetPlayerSessionAsync(string sessionId);
        Task AddAsync(PlayerSession entity);
    }
}