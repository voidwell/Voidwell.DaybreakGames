using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MapStore : IMapStore
    {
        private readonly IMapRepository _mapRepository;
        private readonly ICensusMap _censusMap;
        private readonly CensusWorldEvent _censusWorldEvent;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.mapstore";
        private readonly Func<int, string> _getFacilityWorldEventsCacheKey = worldId => $"{_cacheKey}-facility_world_events-{worldId}";

        private readonly TimeSpan _facilityWorldEventCacheExpiration = TimeSpan.FromSeconds(10);

        public string StoreName => "MapStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public MapStore(IMapRepository mapRepository, ICensusMap censusMap, CensusWorldEvent censusWorldEvent, ICache cache)
        {
            _mapRepository = mapRepository;
            _censusMap = censusMap;
            _censusWorldEvent = censusWorldEvent;
            _cache = cache;
        }

        public async Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId)
        {
            var map = await _censusMap.GetMapOwnership(worldId, zoneId);
            return map?.Regions.Row.ToDictionary(a => a.RowData.RegionId, a => a.RowData.FactionId);
        }

        public Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapHexsByZoneIdAsync(zoneId);
        }

        public Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task CreateZoneSnapshotAsync(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, Dictionary<int, int> zoneOwnership = null)
        {
            if (timestamp == null)
            {
                timestamp = DateTime.UtcNow;
            }

            if (zoneOwnership == null || !zoneOwnership.Any())
            {
                zoneOwnership = await GetMapOwnershipAsync(worldId, zoneId);
            }

            var snapshotRegions = zoneOwnership.Select(a => new ZoneOwnershipSnapshot
            {
                Timestamp = timestamp.Value,
                WorldId = worldId,
                ZoneId = zoneId,
                MetagameInstanceId = metagameInstanceId,
                RegionId = a.Key,
                FactionId = a.Value
            });

            await _mapRepository.InsertRangeAsync(snapshotRegions);
        }

        public Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEventAsync(int worldId, int metagameInstanceId)
        {
            return _mapRepository.GetZoneSnapshotByMetagameEvent(worldId, metagameInstanceId);
        }

        public Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTimeAsync(int worldId, int zoneId, DateTime timestamp)
        {
            return _mapRepository.GetZoneSnapshotByDateTime(worldId, zoneId, timestamp);
        }

        public async Task<IEnumerable<CensusFacilityWorldEventModel>> GetCensusFacilityWorldEventsByZoneIdAsync(int worldId, int zoneId)
        {
            var events = await _cache.GetAsync<IEnumerable<CensusFacilityWorldEventModel>>(_getFacilityWorldEventsCacheKey(worldId));
            if (events == null)
            {
                events = await GetAllCensusFacilityWorldEvents(worldId);
                if (events != null)
                {
                    await _cache.SetAsync(_getFacilityWorldEventsCacheKey(worldId), events, _facilityWorldEventCacheExpiration);
                }
            }

            return events?.Where(a => a.ZoneId == zoneId) ?? Enumerable.Empty<CensusFacilityWorldEventModel>();
        }

        private async Task<IEnumerable<CensusFacilityWorldEventModel>> GetAllCensusFacilityWorldEvents(int worldId)
        {
            var events = (await _censusWorldEvent.GetFacilityWorldEventsByWorldId(worldId))?.ToList();

            if (events != null && events.Any())
            {
                for (var i = 0; i < 12; i++)
                {
                    var lastEvent = events.OrderBy(a => a.Timestamp).First();
                    var additionalEvents = await _censusWorldEvent.GetFacilityWorldEventsByWorldId(worldId, lastEvent.Timestamp);
                    events.AddRange(additionalEvents);
                }
            }

            return events;
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
                await _mapRepository.UpsertRangeAsync(mapRegions.Select(ConvertToDbModel));
            }

            var facilityLinks = await _censusMap.GetAllFacilityLinks();

            if (facilityLinks != null)
            {
                await _mapRepository.UpsertRangeAsync(facilityLinks.Select(ConvertToDbModel));
            }
        }

        private static MapHex ConvertToDbModel(CensusMapHexModel censusModel)
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

        private static MapRegion ConvertToDbModel(CensusMapRegionModel censusModel)
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

        private static FacilityLink ConvertToDbModel(CensusFacilityLinkModel censusModel)
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
