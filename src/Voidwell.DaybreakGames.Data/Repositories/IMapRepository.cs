using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IMapRepository
    {
        Task<IEnumerable<DbMapRegion>> GetMapRegionsByZoneIdAsync(string zoneId);
        Task<IEnumerable<DbFacilityLink>> GetFacilityLinksByZoneIdAsync(string zoneId);
        Task<IEnumerable<DbMapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<string> facilityIds);
        Task UpsertRangeAsync(IEnumerable<DbMapHex> entities);
        Task UpsertRangeAsync(IEnumerable<DbMapRegion> entities);
        Task UpsertRangeAsync(IEnumerable<DbFacilityLink> entities);
    }
}
