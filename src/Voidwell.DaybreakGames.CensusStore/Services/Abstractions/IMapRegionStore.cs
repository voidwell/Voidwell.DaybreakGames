using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IMapRegionStore
    {
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(params int[] facilityIds);
    }
}