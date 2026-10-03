using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services;

public class FactionStore : IFactionStore
{
    private readonly IFactionRepository _repository;
    private readonly ICache _cache;

    private const string _factionCacheKey = "ps2.faction";
    private readonly TimeSpan _factionCacheExpiration = TimeSpan.FromHours(12);

    public FactionStore(IFactionRepository factionRepository, ICache cache)
    {
        _repository = factionRepository;
        _cache = cache;
    }

    public async Task<Faction?> GetFactionByIdAsync(int factionId)
    {
        return await _cache.GetOrSetIfNotNullAsync(
            $"{_factionCacheKey}_{factionId}",
            ct => _repository.GetFactionByIdAsync(factionId),
            _factionCacheExpiration);
    }
}
