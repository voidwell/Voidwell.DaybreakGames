using System.Collections.Concurrent;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class WeaponAggregateService : IWeaponAggregateService
{
    private readonly IWeaponAggregateRepository _weaponAggregateRepository;
    private readonly ICache _cache;

    private const string _cacheKey = "ps2.weaponAggregates";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(8);
    private const int _maxConcurrentLookups = 8;

    public WeaponAggregateService(IWeaponAggregateRepository weaponAggregateRepository, ICache cache)
    {
        _weaponAggregateRepository = weaponAggregateRepository;
        _cache = cache;
    }

    public async Task<WeaponAggregate?> GetAggregateForItem(int itemId)
    {
        var cacheKey = $"{_cacheKey}_{itemId}";

        return await _cache.GetOrSetIfNotNullAsync<WeaponAggregate>(cacheKey, ct => _weaponAggregateRepository.GetWeaponAggregateByItemId(itemId), _cacheExpiration);
    }

    public async Task<Dictionary<string, WeaponAggregate>> GetAggregates(IEnumerable<int> itemIds)
    {
        var aggregates = new ConcurrentBag<WeaponAggregate>();

        // Every cache miss opens a database connection. Looking up all of a character's weapons at once can exhaust
        // the connection pool, so limit how many run at a time.
        var options = new ParallelOptions { MaxDegreeOfParallelism = _maxConcurrentLookups };
        await Parallel.ForEachAsync(itemIds.Distinct(), options, async (itemId, _) =>
        {
            var aggregate = await GetAggregateForItem(itemId);
            if (aggregate != null)
            {
                aggregates.Add(aggregate);
            }
        });

        return aggregates.ToDictionary(a => $"{a.ItemId}-{a.VehicleId}", a => a);
    }
}
