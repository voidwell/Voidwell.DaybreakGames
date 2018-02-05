using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        private readonly CensusMap _censusMap;

        public string ServiceName => "MapService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public MapService(IMapRepository mapRepository, CensusMap censusMap)
        {
            _mapRepository = mapRepository;
            _censusMap = censusMap;
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

        public Task<IEnumerable<MapRegion>> GetMapRegions(int zoneId)
        {
            return _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<FacilityLink>> GetFacilityLinks(int zoneId)
        {
            return _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
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
                mapRegions = mapRegions.Where(a => a.LocationX != 0);
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
    }
}
