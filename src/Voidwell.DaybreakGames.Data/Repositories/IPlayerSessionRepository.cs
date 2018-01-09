using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IPlayerSessionRepository
    {
        Task<IEnumerable<DbPlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit);
        Task<DbPlayerSession> GetPlayerSessionAsync(string sessionId);
        Task AddAsync(DbPlayerSession entity);
    }
}