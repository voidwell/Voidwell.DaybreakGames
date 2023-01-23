using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MapRegionStore : IMapRegionStore
    {
        private readonly IMapRepository _mapRepository;
        private readonly MapRegionCollection _mapRegionCollection;
        private readonly IMapper _mapper;

        public MapRegionStore(IMapRepository mapRepository, MapRegionCollection mapRegionCollection, IMapper mapper)
        {
            _mapRepository = mapRepository;
            _mapRegionCollection = mapRegionCollection;
            _mapper = mapper;
        }

        public string StoreName => "MapRegionStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task RefreshStore()
        {
            var mapRegions = await _mapRegionCollection.GetCollectionAsync();

            if (mapRegions != null)
            {
                await _mapRepository.UpsertRangeAsync(mapRegions.Select(_mapper.Map<MapRegion>));
            }
        }
    }
}
