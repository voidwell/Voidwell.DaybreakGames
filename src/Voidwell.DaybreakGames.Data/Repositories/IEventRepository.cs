using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<DbEventDeath>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
    }
}