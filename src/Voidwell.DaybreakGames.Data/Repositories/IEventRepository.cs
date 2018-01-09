using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.ResolvedModels;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDeath>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task AddAsync<T>(T entity) where T : class;
        Task<DbEventFacilityControl> GetLatestFacilityControl(string worldId, string zoneId, DateTime date);
        Task<IEnumerable<DbEventDeath>> GetDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<DbEventVehicleDestroy>> GetVehicleDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<DbEventFacilityControl>> GetFacilityControlsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
    }
}