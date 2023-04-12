using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class ZoneStore : IZoneStore
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly ICache _cache;

        private const string _cacheKeyPrefix = "ps2.zoneStore";
        private readonly string _playableZonesCacheKey = $"{_cacheKeyPrefix}-playable-zones";
        private readonly TimeSpan _zoneCacheExpiration = TimeSpan.FromMinutes(30);

        private readonly int[] _playableZoneIds = { 2, 4, 6, 8, 344 };

        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public ZoneStore(IZoneRepository zoneRepository, ICache cache)
        {
            _zoneRepository = zoneRepository;
            _cache = cache;
        }

        public Task<IEnumerable<Zone>> GetAllZones()
        {
            return _zoneRepository.GetAllZonesAsync();
        }

        public async Task<Zone> GetZone(int zoneId)
        {
            var cacheKey = $"{_cacheKeyPrefix}_{zoneId}";

            var zone = await _cache.GetAsync<Zone>(cacheKey);
            if (zone != null)
            {
                return zone;
            }

            zone = (await _zoneRepository.GetZonesByIdsAsync(zoneId)).FirstOrDefault();
            if (zone != null)
            {
                await _cache.SetAsync(cacheKey, zone, _zoneCacheExpiration);
            }

            return zone;
        }

        public async Task<IEnumerable<Zone>> GetPlayableZones()
        {
            var zones = await _cache.GetAsync<IEnumerable<Zone>>(_playableZonesCacheKey);
            if (zones != null)
            {
                return zones;
            }

            zones = await _zoneRepository.GetZonesByIdsAsync(_playableZoneIds);
            if (zones != null)
            {
                await _cache.SetAsync(_playableZonesCacheKey, zones, _zoneCacheExpiration);
            }

            return zones;
        }
    }
}
