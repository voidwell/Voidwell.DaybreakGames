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

    public WeaponAggregateService(IWeaponAggregateRepository weaponAggregateRepository, ICache cache)
    {
        _weaponAggregateRepository = weaponAggregateRepository;
        _cache = cache;
    }

    public async Task<WeaponAggregate?> GetAggregateForItem(int itemId)
    {
        return await _cache.GetOrSetIfNotNullAsync<WeaponAggregate>(
            GetCacheKey(itemId),
            async ct => (await _weaponAggregateRepository.GetWeaponAggregatesByItemIds(itemId)).FirstOrDefault(),
            _cacheExpiration);
    }

    public async Task<Dictionary<string, WeaponAggregate>> GetAggregates(IEnumerable<int> itemIds)
    {
        var distinctItemIds = itemIds.Distinct().ToList();

        var cached = await Task.WhenAll(distinctItemIds.Select(itemId => _cache.GetAsync<WeaponAggregate>(GetCacheKey(itemId))));
        var aggregates = cached.OfType<WeaponAggregate>().ToList();

        var cachedItemIds = aggregates.Select(a => a.ItemId).ToHashSet();
        var missingItemIds = distinctItemIds.Where(a => !cachedItemIds.Contains(a)).ToArray();

        if (missingItemIds.Length > 0)
        {
            // One query for every item that is not cached, instead of one per item. As in GetAggregateForItem, an item
            // with no aggregate is not cached.
            var found = (await _weaponAggregateRepository.GetWeaponAggregatesByItemIds(missingItemIds))
                .GroupBy(a => a.ItemId)
                .Select(a => a.First())
                .ToList();

            await Task.WhenAll(found.Select(a => _cache.SetAsync(GetCacheKey(a.ItemId), a, _cacheExpiration)));

            aggregates.AddRange(found);
        }

        return aggregates.ToDictionary(a => $"{a.ItemId}-{a.VehicleId}", a => a);
    }

    private static string GetCacheKey(int itemId) => $"{_cacheKey}_{itemId}";
}
