using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services;

public class ZoneStore : IZoneStore
{
    private readonly IZoneRepository _zoneRepository;
    private readonly ICache _cache;

    private const string _cacheKeyPrefix = "ps2.zoneStore";
    private readonly string _playableZonesCacheKey = $"{_cacheKeyPrefix}-playable-zones";
    private readonly TimeSpan _zoneCacheExpiration = TimeSpan.FromMinutes(30);

    private readonly int[] _playableZoneIds = { 2, 4, 6, 8, 344 };

    public static TimeSpan UpdateInterval => TimeSpan.FromDays(7);

    public ZoneStore(IZoneRepository zoneRepository, ICache cache)
    {
        _zoneRepository = zoneRepository;
        _cache = cache;
    }

    public Task<IEnumerable<Zone>> GetAllZones()
    {
        return _zoneRepository.GetAllZonesAsync();
    }

    public async Task<Zone?> GetZoneAsync(int zoneId)
    {
        return await _cache.GetOrSetIfNotNullAsync(
            $"{_cacheKeyPrefix}_{zoneId}",
            async ct => (await _zoneRepository.GetZonesByIdsAsync(zoneId)).FirstOrDefault(),
            _zoneCacheExpiration);
    }

    public async Task<IEnumerable<Zone>?> GetPlayableZones()
    {
        return await _cache.GetOrSetAsync(
            _playableZonesCacheKey,
            ct => _zoneRepository.GetZonesByIdsAsync(_playableZoneIds),
            _zoneCacheExpiration);
    }
}
