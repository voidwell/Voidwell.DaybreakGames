using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services;

public class WorldStore : IWorldStore
{
    private readonly IWorldRepository _worldRepository;
    private readonly ICache _cache;

    private const string _cacheKeyPrefix = "ps2.worldstore";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);


    public WorldStore(IWorldRepository worldRepository, ICache cache)
    {
        _worldRepository = worldRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<World>?> GetAllWorlds()
    {
        return await _cache.GetOrSetAsync(
            _cacheKeyPrefix,
            ct => _worldRepository.GetAllWorldsAsync(),
            _cacheExpiration);
    }

    public async Task<IEnumerable<DailyPopulation>?> GetWorldPopulationHistory(int worldId, DateTime start, DateTime end)
    {
        return await _cache.GetOrSetAsync(
            $"{_cacheKeyPrefix}_{worldId}_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}",
            ct => _worldRepository.GetDailyPopulationsByWorldIdAsync(worldId),
            _cacheExpiration);
    }
}
