using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MapRegionStore : IMapRegionStore
    {
        private readonly IMapRepository _mapRepository;

        public MapRegionStore(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }
    }
}
