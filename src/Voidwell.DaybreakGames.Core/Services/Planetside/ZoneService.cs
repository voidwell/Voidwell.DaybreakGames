using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Census.Services;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly CensusZone _censusZone;
        private readonly ICache _cache;

        private const string _cacheKeyPrefix = "ps2.zoneService";
        private readonly string _playableZonesCacheKey = $"{_cacheKeyPrefix}-playable-zones";
        private readonly TimeSpan _zoneCacheExpiration = TimeSpan.FromMinutes(30);

        private readonly int[] _playableZoneIds = { 2, 4, 6, 8 };

        public ZoneService(IZoneRepository zoneRepository, CensusZone censusZone, ICache cache)
        {
            _zoneRepository = zoneRepository;
            _censusZone = censusZone;
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

        public async Task RefreshStore()
        {
            var zones = await _censusZone.GetAllZones();

            if (zones != null)
            {
                await _zoneRepository.UpsertRangeAsync(zones.Select(ConvertToDbModel));
                await _cache.RemoveAsync(_playableZonesCacheKey);
            }
        }

        private static Zone ConvertToDbModel(CensusZoneModel censusModel)
        {
            return new Zone
            {
                Id = censusModel.ZoneId,
                Name = censusModel.Name.English,
                Description = censusModel.Description.English,
                Code = censusModel.Code,
                HexSize = censusModel.HexSize
            };
        }
    }
}
