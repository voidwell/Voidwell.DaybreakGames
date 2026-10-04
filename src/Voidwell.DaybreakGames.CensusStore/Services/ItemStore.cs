using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services;

public class ItemStore : IItemStore
{
    private readonly IItemRepository _itemRepository;
    private readonly ItemCollection _itemCollection;
    private readonly ICache _cache;

    private const string _categoryItemsCacheKey = "ps2.categoryItems";
    private readonly TimeSpan _categoryItemsCacheExpiration = TimeSpan.FromHours(1);

    public ItemStore(IItemRepository itemRepository, ItemCollection itemCollection, ICache cache)
    {
        _itemRepository = itemRepository;
        _itemCollection = itemCollection;
        _cache = cache;
    }

    public Task<IEnumerable<Item>> FindItemsByIdsAsync(IEnumerable<int> itemIds)
    {
        return _itemRepository.FindItemsByIdsAsync(itemIds);
    }

    public async Task<IEnumerable<Item>?> GetItemsByCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        return await _cache.GetOrSetAsync(
            $"{_categoryItemsCacheKey}_{string.Join("-", categoryIds)}",
            ct => _itemRepository.GetItemsByCategoryIds(categoryIds),
            _categoryItemsCacheExpiration);
    }

    public Task<IEnumerable<Item>> FindWeaponsByNameAsync(string name, int limit = 12)
    {
        return _itemRepository.FindWeaponsByNameAsync(name, limit);
    }

    public Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId)
    {
        return _itemCollection.GetWeaponInfoAsync(weaponItemId)!;
    }
}
