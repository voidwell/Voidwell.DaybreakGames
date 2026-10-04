using Voidwell.DaybreakGames.Cache;
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
        var cacheKey = $"{_cacheKey}_{itemId}";

        return await _cache.GetOrSetIfNotNullAsync<WeaponAggregate>(cacheKey, ct => _weaponAggregateRepository.GetWeaponAggregateByItemId(itemId), _cacheExpiration);
    }

    public async Task<Dictionary<string, WeaponAggregate>> GetAggregates(IEnumerable<int> itemIds)
    {
        var aggregates = await Task.WhenAll(itemIds.Distinct().Select(GetAggregateForItem));

        return aggregates.Where(a => a != null).ToDictionary(a => $"{a!.ItemId}-{a.VehicleId}", a => a)!;
    }
}
