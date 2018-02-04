using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IMapRepository
    {
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(string zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(string zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<string> facilityIds);
        Task UpsertRangeAsync(IEnumerable<MapHex> entities);
        Task UpsertRangeAsync(IEnumerable<MapRegion> entities);
        Task UpsertRangeAsync(IEnumerable<FacilityLink> entities);
    }
}
