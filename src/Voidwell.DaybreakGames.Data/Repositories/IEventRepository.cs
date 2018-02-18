using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Death>> GetDeathEventsForCharacterIdByDateAsync(string characterId, DateTime lower, DateTime upper);
        Task AddAsync<T>(T entity) where T : class;
        Task<FacilityControl> GetLatestFacilityControl(int worldId, int zoneId, DateTime date);
        Task<IEnumerable<Death>> GetDeathEventsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate);
        Task<IEnumerable<VehicleDestroy>> GetVehicleDeathEventsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate);
        Task<IEnumerable<FacilityControl>> GetFacilityControlsByDateAsync(int worldId, int zoneId, DateTime startDate, DateTime? endDate);
    }
}