using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.ResolvedModels;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<ResolvedModels.EventDeath>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task AddAsync<T>(T entity) where T : class;
        Task<EventFacilityControl> GetLatestFacilityControl(string worldId, string zoneId, DateTime date);
        Task<IEnumerable<Models.Planetside.EventDeath>> GetDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<EventVehicleDestroy>> GetVehicleDeathEventsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<EventFacilityControl>> GetFacilityControlsByDateAsync(string worldId, string zoneId, DateTime startDate, DateTime endDate);
    }
}