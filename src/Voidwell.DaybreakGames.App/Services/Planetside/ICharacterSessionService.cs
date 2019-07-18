using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICharacterSessionService
    {
        Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId, int limit = 25, int page = 0);
        Task<Models.PlayerSession> GetSession(string characterId, int sessionId);
        Task<Models.PlayerSession> GetSession(string characterId);
    }
}