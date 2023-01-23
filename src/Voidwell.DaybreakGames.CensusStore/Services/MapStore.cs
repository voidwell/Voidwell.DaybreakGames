using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MapStore : IMapStore
    {
        private readonly IMapRepository _mapRepository;
        private readonly MapCollection _mapCollection;
        private readonly WorldEventCollection _worldEventCollection;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.mapstore";
        private readonly Func<int, string> _getFacilityWorldEventsCacheKey = worldId => $"{_cacheKey}-facility_world_events-{worldId}";

        private readonly TimeSpan _facilityWorldEventCacheExpiration = TimeSpan.FromSeconds(10);

        public MapStore(IMapRepository mapRepository, MapCollection mapCollection, WorldEventCollection worldEventCollection, ICache cache)
        {
            _mapRepository = mapRepository;
            _mapCollection = mapCollection;
            _worldEventCollection = worldEventCollection;
            _cache = cache;
        }

        public async Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId)
        {
            var map = await _mapCollection.GetMapOwnershipAsync(worldId, zoneId);
            return map?.Regions.Row.ToDictionary(a => a.RowData.RegionId, a => a.RowData.FactionId);
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
            var events = (await _worldEventCollection.GetFacilityWorldEventsByWorldIdAsync(worldId))?.ToList();

            if (events != null && events.Any())
            {
                for (var i = 0; i < 12; i++)
                {
                    var lastEvent = events.OrderBy(a => a.Timestamp).First();
                    var additionalEvents = await _worldEventCollection.GetFacilityWorldEventsByWorldIdAsync(worldId, lastEvent.Timestamp);
                    events.AddRange(additionalEvents);
                }
            }

            return events;
        }
    }
}
