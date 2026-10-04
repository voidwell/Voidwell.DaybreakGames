using AsyncKeyedLock;
using AutoMapper;
using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class MapService : IMapService
{
    private readonly IMapStore _mapStore;
    private readonly IMapHexStore _mapHexStore;
    private readonly IMapRegionStore _mapRegionStore;
    private readonly IFacilityLinkStore _facilityLinkStore;
    private readonly IWorldEventsService _worldEventsService;
    private readonly IZoneStore _zoneStore;
    private readonly ICache _cache;
    private readonly IMapper _mapper;

    private readonly AsyncKeyedLocker<string> _zoneMapLock = new();
    private readonly AsyncKeyedLocker<string> _zoneHistoryLock = new();
    private readonly SemaphoreSlim _zoneStateLock = new SemaphoreSlim(1);

    private const string _cacheKey = "ps2.map_service";
    private readonly Func<int, string> _getZoneMapCacheKey = zoneId => $"{_cacheKey}-zonemap-{zoneId}";
    private readonly string _getZoneStateHistoricalCacheKey = $"{_cacheKey}__zonestatehistorical";

    private readonly TimeSpan _zoneMapCacheExpiration = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _zoneStateCacheExpiration = TimeSpan.FromSeconds(30);

    public MapService(IMapStore mapStore, IMapHexStore mapHexStore, IMapRegionStore mapRegionStore,
        IFacilityLinkStore facilityLinkStore, IWorldEventsService worldEventsService,
        IZoneStore zoneStore, ICache cache, IMapper mapper)
    {
        _mapStore = mapStore;
        _mapHexStore = mapHexStore;
        _mapRegionStore = mapRegionStore;
        _facilityLinkStore = facilityLinkStore;
        _worldEventsService = worldEventsService;
        _zoneStore = zoneStore;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<ZoneMap> GetZoneMapAsync(int zoneId)
    {
        using (await _zoneMapLock.LockAsync(zoneId.ToString()))
        {
            return await _cache.GetOrSetAsync<ZoneMap>(_getZoneMapCacheKey(zoneId), async ct =>
            {
                var zoneTask = _zoneStore.GetZoneAsync(zoneId);
                var linksTask = _facilityLinkStore.GetFacilityLinksByZoneIdAsync(zoneId);
                var hexesTask = _mapHexStore.GetMapHexsByZoneIdAsync(zoneId);
                var regionsTask = _mapRegionStore.GetMapRegionsByZoneIdAsync(zoneId);

                await Task.WhenAll(zoneTask, linksTask, hexesTask, regionsTask);

                var zone = zoneTask.Result;
                var links = _mapper.Map<IEnumerable<ZoneLink>>(linksTask.Result);
                var hexes = _mapper.Map<IEnumerable<ZoneHex>>(hexesTask.Result);

                var filteredRegions = regionsTask.Result?.Where(a => links.Any(b => a.FacilityId == b.FacilityIdA || a.FacilityId == b.FacilityIdB)).ToList();
                var regions = _mapper.Map<IEnumerable<ZoneRegion>>(filteredRegions);

                var zoneMap = new ZoneMap
                {
                    Regions = regions,
                    Hexs = hexes,
                    Links = links,
                    HexSize = zone?.HexSize
                };

                return zoneMap;
            }, _zoneMapCacheExpiration);
        }
    }

    public async Task<IEnumerable<ZoneRegionOwnership>?> GetMapOwnership(int worldId, int zoneId)
    {
        var mapOwnership = await _mapStore.GetMapOwnershipAsync(worldId, zoneId);

        return mapOwnership?.Select(o => new ZoneRegionOwnership(o.Key, o.Value));
    }

    public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnershipFromHistory(int worldId, int zoneId)
    {
        using (await _zoneHistoryLock.LockAsync(worldId.ToString()))
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

    public Task CreateZoneSnapshot(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, IEnumerable<ZoneRegionOwnership>? zoneOwnership = null)
    {
        return _mapStore.CreateZoneSnapshotAsync(worldId, zoneId, timestamp, metagameInstanceId, zoneOwnership?.ToDictionary(a => a.RegionId, a => a.FactionId));
    }

    public async Task<ZoneSnapshot?> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
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

    public async Task<ZoneSnapshot?> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp)
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
            return await _cache.GetOrSetAsync<ZoneStateHistorical>(_getZoneStateHistoricalCacheKey, async ct =>
            {
                var zoneLocks = _worldEventsService.GetAllLatestZoneLocks();
                var zoneUnlocks = _worldEventsService.GetAllLatestZoneUnlocks();

                await Task.WhenAll(zoneLocks, zoneUnlocks);

                var results = new ZoneStateHistorical(zoneLocks.Result, zoneUnlocks.Result);
                return results;
            }, _zoneStateCacheExpiration);
        }
        finally
        {
            _zoneStateLock.Release();
        }
    }
}
