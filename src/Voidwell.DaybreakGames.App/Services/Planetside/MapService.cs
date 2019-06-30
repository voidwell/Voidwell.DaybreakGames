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
using System.Threading;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        private readonly IWorldEventsService _worldEventsService;
        private readonly CensusMap _censusMap;
        private readonly CensusWorldEvent _censusWorldEvent;
        private readonly ICache _cache;

        public string ServiceName => "MapService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private readonly KeyedSemaphoreSlim _zoneMapLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _zoneHistoryLock = new KeyedSemaphoreSlim();
        private readonly SemaphoreSlim _zoneStateLock = new SemaphoreSlim(1);

        private const string _cacheKeyPrefix = "ps2.map_service";
        private readonly TimeSpan _zoneMapCacheExpiration = TimeSpan.FromHours(24);
        private readonly TimeSpan _facilityWorldEventCacheExpiration = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _zoneStateCacheExpiration = TimeSpan.FromSeconds(30);

        public MapService(IMapRepository mapRepository, IWorldEventsService worldEventsService, CensusMap censusMap, CensusWorldEvent censusWorldEvent, ICache cache)
        {
            _mapRepository = mapRepository;
            _worldEventsService = worldEventsService;
            _censusMap = censusMap;
            _censusWorldEvent = censusWorldEvent;
            _cache = cache;
        }

        public async Task<ZoneMap> GetZoneMap(int zoneId)
        {
            using (await _zoneMapLock.WaitAsync(zoneId.ToString()))
            {
                var cacheKey = GetCacheKey($"zoneMap-{zoneId}");

                var zoneMap = await _cache.GetAsync<ZoneMap>(cacheKey);
                if (zoneMap != null)
                {
                    return zoneMap;
                }

                var linksTask = _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
                var hexsTask = _mapRepository.GetMapHexsByZoneIdAsync(zoneId);
                var regionsTask = _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);

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

                await _cache.SetAsync(cacheKey, zoneMap, _zoneMapCacheExpiration);

                return zoneMap;
            }
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(int worldId, int zoneId)
        {
            var ownership = await _censusMap.GetMapOwnership(worldId, zoneId);

            return ownership?.Regions.Row.Select(o => new ZoneRegionOwnership(o.RowData.RegionId, o.RowData.FactionId));
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnershipFromHistory(int worldId, int zoneId)
        {
            using (await _zoneHistoryLock.WaitAsync(worldId.ToString()))
            {
                var regionsTask = _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);
                var eventsTask = GetCensusFacilityWorldEventsByZoneId(worldId, zoneId);

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

        public Task<IEnumerable<MapRegion>> FindRegions(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task CreateZoneSnapshot(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, IEnumerable<ZoneRegionOwnership> zoneOwnership = null)
        {
            if (timestamp == null)
            {
                timestamp = DateTime.UtcNow;
            }

            if (zoneOwnership == null || !zoneOwnership.Any())
            {
                zoneOwnership = await GetMapOwnership(worldId, zoneId);
            }

            if (zoneOwnership == null)
            {
                return;
            }

            var snapshotRegions = zoneOwnership.Select(a => new ZoneOwnershipSnapshot
            {
                Timestamp = timestamp.Value,
                WorldId = worldId,
                ZoneId = zoneId,
                MetagameInstanceId = metagameInstanceId,
                RegionId = a.RegionId,
                FactionId = a.FactionId
            });

            await _mapRepository.InsertRangeAsync(snapshotRegions);
        }

        public async Task<ZoneSnapshot> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
        {
            var snapshotRegions = await _mapRepository.GetZoneSnapshotByMetagameEvent(worldId, metagameInstanceId);
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
            var snapshotRegions = await _mapRepository.GetZoneSnapshotByDateTime(worldId, zoneId, timestamp);
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
            var cacheKey = $"{_cacheKeyPrefix}_zonestatehistorical";

            await _zoneStateLock.WaitAsync();

            try
            {
                var results = await _cache.GetAsync<ZoneStateHistorical>(cacheKey);
                if (results != null)
                {
                    return results;
                }

                var zoneLocks = _worldEventsService.GetAllLatestZoneLocks();
                var zoneUnlocks = _worldEventsService.GetAllLatestZoneUnlocks();

                await Task.WhenAll(zoneLocks, zoneUnlocks);

                results = new ZoneStateHistorical(zoneLocks.Result, zoneUnlocks.Result);
                await _cache.SetAsync(cacheKey, results, _zoneStateCacheExpiration);

                return results;
            }
            finally
            {
                _zoneStateLock.Release();
            }
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

        private async Task<IEnumerable<CensusFacilityWorldEventModel>> GetCensusFacilityWorldEventsByZoneId(int worldId, int zoneId)
        {
            var cacheKey = $"{_cacheKeyPrefix}_facility_world_events_{worldId}";

            var events = await _cache.GetAsync<IEnumerable<CensusFacilityWorldEventModel>>(cacheKey);
            if (events == null)
            {
                events = await GetAllCensusFacilityWorldEvents(worldId);
                if (events != null)
                {
                    await _cache.SetAsync(cacheKey, events, _facilityWorldEventCacheExpiration);
                }
            }

            return events?.Where(a => a.ZoneId == zoneId) ?? Enumerable.Empty<CensusFacilityWorldEventModel>();
        }

        private async Task<IEnumerable<CensusFacilityWorldEventModel>> GetAllCensusFacilityWorldEvents(int worldId)
        {
            var events = (await _censusWorldEvent.GetFacilityWorldEventsByWorldId(worldId))?.ToList();

            if (events != null && events.Any())
            {
                for(var i = 0; i < 12; i++)
                {
                    var lastEvent = events.OrderBy(a => a.Timestamp).First();
                    var additionalEvents = await _censusWorldEvent.GetFacilityWorldEventsByWorldId(worldId, lastEvent.Timestamp);
                    events.AddRange(additionalEvents);
                }
            }

            return events;
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
