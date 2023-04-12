using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface ICharacterSessionService
    {
        Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId, int limit = 25, int page = 0);
        Task<Domain.Models.PlayerSession> GetSession(string characterId, int sessionId);
        Task<Domain.Models.PlayerSession> GetSession(string characterId);
    }
}