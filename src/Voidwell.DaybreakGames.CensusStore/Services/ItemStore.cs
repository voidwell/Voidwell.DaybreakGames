using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
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

        public async Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(IEnumerable<int> categoryIds)
        {
            var cacheKey = $"{_categoryItemsCacheKey}_{string.Join("-", categoryIds)}";

            var items = await _cache.GetAsync<IEnumerable<Item>>(cacheKey);
            if (items != null)
            {
                return items;
            }

            items = await _itemRepository.GetItemsByCategoryIds(categoryIds);
            if (items != null)
            {
                await _cache.SetAsync(cacheKey, items, _categoryItemsCacheExpiration);
            }

            return items;
        }

        public Task<IEnumerable<Item>> FindWeaponsByNameAsync(string name, int limit = 12)
        {
            return _itemRepository.FindWeaponsByNameAsync(name, limit);
        }

        public Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId)
        {
            return _itemCollection.GetWeaponInfoAsync(weaponItemId);
        }
    }
}
