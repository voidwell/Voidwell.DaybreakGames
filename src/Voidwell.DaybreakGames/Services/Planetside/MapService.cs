using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        private readonly CensusMap _censusMap;
        private readonly ICache _cache;

        public string ServiceName => "MapService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private readonly KeyedSemaphoreSlim _mapRegionsLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _facilityLinksLock = new KeyedSemaphoreSlim();

        private const string _cacheKeyPrefix = "ps2.mapService";
        private TimeSpan _mapRegionCacheExpiration = TimeSpan.FromMinutes(30);
        private TimeSpan _facilityLinksCacheExpiration = TimeSpan.FromMinutes(30);

        public MapService(IMapRepository mapRepository, CensusMap censusMap, ICache cache)
        {
            _mapRepository = mapRepository;
            _censusMap = censusMap;
            _cache = cache;
        }

        public async Task<IEnumerable<MapOwnership>> GetMapOwnership(int worldId, int zoneId)
        {
            var ownership = await _censusMap.GetMapOwnership(worldId, zoneId);
            if (ownership == null)
            {
                return null;
            }

            return ownership.Regions.Row.Select(o => new MapOwnership(o.RowData.RegionId, o.RowData.FactionId));
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegions(int zoneId)
        {
            using (await _mapRegionsLock.WaitAsync(zoneId.ToString()))
            {
                var cacheKey = GetCacheKey($"mapregion-{zoneId}");

                var mapRegions = await _cache.GetAsync<IEnumerable<MapRegion>>(cacheKey);
                if (mapRegions != null)
                {
                    return mapRegions;
                }

                mapRegions = await _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);

                if (mapRegions != null)
                {
                    await _cache.SetAsync(cacheKey, mapRegions, _mapRegionCacheExpiration);
                }

                return mapRegions;
            }
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinks(int zoneId)
        {
            using (await _facilityLinksLock.WaitAsync(zoneId.ToString()))
            {
                var cacheKey = GetCacheKey($"facilityLinks-{zoneId}");

                var facilityLinks = await _cache.GetAsync<IEnumerable<FacilityLink>>(cacheKey);
                if (facilityLinks != null)
                {
                    return facilityLinks;
                }

                facilityLinks = await _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);

                if (facilityLinks != null)
                {
                    await _cache.SetAsync(cacheKey, facilityLinks, _facilityLinksCacheExpiration);
                }

                return facilityLinks;
            }
        }

        public Task<IEnumerable<MapRegion>> FindRegions(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task RefreshStore()
        {
            var mapHexs = await _censusMap.GetAllMapHexs();

            if (mapHexs != null)
            {
                await _mapRepository.UpsertRangeAsync(mapHexs.Select(ConvertToDbModel));
            }

            var mapRegions = await _censusMap.GetAllMapRegions();

            if (mapRegions != null)
            {
                mapRegions = mapRegions.ToList().Where(a => a.MapRegionId != 6329 && a.FacilityId != 400127);

                await _mapRepository.UpsertRangeAsync(mapRegions.Select(ConvertToDbModel));
            }

            var facilityLinks = await _censusMap.GetAllFacilityLinks();

            if (facilityLinks != null)
            {
                await _mapRepository.UpsertRangeAsync(facilityLinks.Select(ConvertToDbModel));
            }
        }

        private MapHex ConvertToDbModel(CensusMapHexModel censusModel)
        {
            return new MapHex
            {
                MapRegionId = censusModel.MapRegionId,
                HexType = censusModel.HexType,
                TypeName = censusModel.TypeName,
                ZoneId = censusModel.ZoneId,
                XPos = censusModel.X,
                YPos = censusModel.Y
            };
        }

        private MapRegion ConvertToDbModel(CensusMapRegionModel censusModel)
        {
            return new MapRegion
            {
                Id = censusModel.MapRegionId,
                ZoneId = censusModel.ZoneId,
                FacilityId = censusModel.FacilityId,
                FacilityName = censusModel.FacilityName,
                FacilityTypeId = censusModel.FacilityTypeId,
                FacilityType = censusModel.FacilityType,
                XPos = censusModel.LocationX,
                YPos = censusModel.LocationY,
                ZPos = censusModel.LocationZ
            };
        }

        private FacilityLink ConvertToDbModel(CensusFacilityLinkModel censusModel)
        {
            return new FacilityLink
            {
                ZoneId = censusModel.ZoneId,
                FacilityIdA = censusModel.FacilityIdA,
                FacilityIdB = censusModel.FacilityIdB,
                Description = censusModel.Description
            };
        }

        private string GetCacheKey(string id)
        {
            return $"{_cacheKeyPrefix}-{id}";
        }
    }
}
