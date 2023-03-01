using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.Microservice.Cache;
using System.Threading;
using Voidwell.Microservice.Utility;
using Voidwell.DaybreakGames.CensusStore.Services;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService
    {
        private readonly IMapStore _mapStore;
        private readonly IMapHexStore _mapHexStore;
        private readonly IMapRegionStore _mapRegionStore;
        private readonly IFacilityLinkStore _facilityLinkStore;
        private readonly IWorldEventsService _worldEventsService;
        private readonly ICache _cache;

        private readonly KeyedSemaphoreSlim _zoneMapLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _zoneHistoryLock = new KeyedSemaphoreSlim();
        private readonly SemaphoreSlim _zoneStateLock = new SemaphoreSlim(1);

        private const string _cacheKey = "ps2.map_service";
        private readonly Func<int, string> _getZoneMapCacheKey = zoneId => $"{_cacheKey}-zonemap-{zoneId}";
        private readonly string _getZoneStateHistoricalCacheKey = $"{_cacheKey}__zonestatehistorical";

        private readonly TimeSpan _zoneMapCacheExpiration = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _zoneStateCacheExpiration = TimeSpan.FromSeconds(30);

        public MapService(IMapStore mapStore, IMapHexStore mapHexStore, IMapRegionStore mapRegionStore,
            IFacilityLinkStore facilityLinkStore, IWorldEventsService worldEventsService, ICache cache)
        {
            _mapStore = mapStore;
            _mapHexStore = mapHexStore;
            _mapRegionStore = mapRegionStore;
            _facilityLinkStore = facilityLinkStore;
            _worldEventsService = worldEventsService;
            _cache = cache;
        }

        public async Task<ZoneMap> GetZoneMapAsync(int zoneId)
        {
            using (await _zoneMapLock.WaitAsync(zoneId.ToString()))
            {
                var zoneMap = await _cache.GetAsync<ZoneMap>(_getZoneMapCacheKey(zoneId));
                if (zoneMap != null)
                {
                    return zoneMap;
                }

                var linksTask = _facilityLinkStore.GetFacilityLinksByZoneIdAsync(zoneId);
                var hexsTask = _mapHexStore.GetMapHexsByZoneIdAsync(zoneId);
                var regionsTask = _mapRegionStore.GetMapRegionsByZoneIdAsync(zoneId);

                await Task.WhenAll(linksTask, hexsTask, regionsTask);

                var links = linksTask.Result.Select(a => new ZoneLink
                {
                    FacilityIdA = a.FacilityIdA,
                    FacilityIdB = a.FacilityIdB
                });

                var hexs = hexsTask.Result.Select(a => new ZoneHex
                {
                    MapRegionId = a.MapRegionId,
                    X = a.XPos,
                    Y = a.YPos
                });

                var regions = regionsTask.Result
                    .Where(a => links.Any(b => a.FacilityId == b.FacilityIdA || a.FacilityId == b.FacilityIdB))
                    .Select(a => new ZoneRegion
                    {
                        RegionId = a.Id,
                        FacilityId = a.FacilityId,
                        FacilityName = a.FacilityName,
                        FacilityType = a.FacilityType,
                        FacilityTypeId = a.FacilityTypeId,
                        X = a.XPos,
                        Y = a.YPos,
                        Z = a.ZPos
                    });

                zoneMap = new ZoneMap
                {
                    Regions = regions,
                    Hexs = hexs,
                    Links = links
                };

                await _cache.SetAsync(_getZoneMapCacheKey(zoneId), zoneMap, _zoneMapCacheExpiration);

                return zoneMap;
            }
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(int worldId, int zoneId)
        {
            var mapOwnership = await _mapStore.GetMapOwnershipAsync(worldId, zoneId);

            return mapOwnership?.Select(o => new ZoneRegionOwnership(o.Key, o.Value));
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnershipFromHistory(int worldId, int zoneId)
        {
            using (await _zoneHistoryLock.WaitAsync(worldId.ToString()))
            {
                var regionsTask = _mapRegionStore.GetMapRegionsByZoneIdAsync(zoneId);
                var eventsTask = _mapStore.GetCensusFacilityWorldEventsByZoneIdAsync(worldId, zoneId);

                await Task.WhenAll(regionsTask, eventsTask);

                var regions = regionsTask.Result;
                var facilityEvents = eventsTask.Result;

                var regionMapping = new Dictionary<int, ZoneRegionOwnership>();

                foreach (var facilityEvent in facilityEvents.OrderBy(a => a.Timestamp))
                {
                    var region = regions.FirstOrDefault(a => a.FacilityId == facilityEvent.FacilityId);
                    if (region == null)
                    {
                        continue;
                    }

                    regionMapping[region.Id] = new ZoneRegionOwnership(region.Id, facilityEvent.FactionNew);
                }

                foreach (var region in regions)
                {
                    if (!regionMapping.ContainsKey(region.Id))
                    {
                        regionMapping.Add(region.Id, new ZoneRegionOwnership(region.Id, 0));
                    }
                }

                return regionMapping.Values;
            }
        }

        public Task<IEnumerable<MapRegion>> FindRegionsAsync(params int[] facilityIds)
        {
            return _mapRegionStore.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public  Task CreateZoneSnapshot(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, IEnumerable<ZoneRegionOwnership> zoneOwnership = null)
        {
            return _mapStore.CreateZoneSnapshotAsync(worldId, zoneId, timestamp, metagameInstanceId, zoneOwnership?.ToDictionary(a => a.RegionId, a => a.FactionId));
        }

        public async Task<ZoneSnapshot> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
        {
            var snapshotRegions = await _mapStore.GetZoneSnapshotByMetagameEventAsync(worldId, metagameInstanceId);
            if (snapshotRegions == null || !snapshotRegions.Any())
            {
                return null;
            }

            return new ZoneSnapshot
            {
                Timestamp = snapshotRegions.First().Timestamp,
                WorldId = snapshotRegions.First().WorldId,
                ZoneId = snapshotRegions.First().ZoneId,
                MetagameInstanceId = snapshotRegions.First().MetagameInstanceId,
                Ownership = snapshotRegions.Select(a => new ZoneRegionOwnership(a.RegionId, a.FactionId))
            };
        }

        public async Task<ZoneSnapshot> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp)
        {
            var snapshotRegions = await _mapStore.GetZoneSnapshotByDateTimeAsync(worldId, zoneId, timestamp);
            if (snapshotRegions == null || !snapshotRegions.Any())
            {
                return null;
            }

            return new ZoneSnapshot
            {
                Timestamp = snapshotRegions.First().Timestamp,
                WorldId = snapshotRegions.First().WorldId,
                ZoneId = snapshotRegions.First().ZoneId,
                Ownership = snapshotRegions.Select(a => new ZoneRegionOwnership(a.RegionId, a.FactionId))
            };
        }

        public async Task<ZoneStateHistorical> GetZoneStateHistoricals()
        {
            await _zoneStateLock.WaitAsync();

            try
            {
                var results = await _cache.GetAsync<ZoneStateHistorical>(_getZoneStateHistoricalCacheKey);
                if (results != null)
                {
                    return results;
                }

                var zoneLocks = _worldEventsService.GetAllLatestZoneLocks();
                var zoneUnlocks = _worldEventsService.GetAllLatestZoneUnlocks();

                await Task.WhenAll(zoneLocks, zoneUnlocks);

                results = new ZoneStateHistorical(zoneLocks.Result, zoneUnlocks.Result);
                await _cache.SetAsync(_getZoneStateHistoricalCacheKey, results, _zoneStateCacheExpiration);

                return results;
            }
            finally
            {
                _zoneStateLock.Release();
            }
        }
    }
}
