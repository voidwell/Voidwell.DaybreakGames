using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICache _cache;
        private readonly CensusItem _censusItem;
        private readonly CensusItemCategory _censusItemCategory;

        private const string _categoryItemsCacheKey = "ps2.categoryItems";
        private readonly TimeSpan _categoryItemsCacheExpiration = TimeSpan.FromHours(1);

        public string ServiceName => "ItemService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(45);

        public ItemService(IItemRepository itemRepository, ICache cache, CensusItem censusItem, CensusItemCategory censusItemCategory)
        {
            _itemRepository = itemRepository;
            _cache = cache;
            _censusItem = censusItem;
            _censusItemCategory = censusItemCategory;
        }

        public Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds)
        {
            return _itemRepository.FindItemsByIdsAsync(itemIds);
        }

        public Task<IEnumerable<Item>> LookupItemsByName(string name, int limit = 12)
        {
            return _itemRepository.FindItemsByNameAsync(name, limit);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds)
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

        public async Task RefreshStore()
        {
            var itemCategories = await _censusItemCategory.GetAllItemCategories();

            if (itemCategories != null)
            {
                await _itemRepository.UpsertRangeAsync(itemCategories.Select(ConvertToDbModel));
            }

            var items = await _censusItem.GetAllItems();

            if (items != null)
            {
                await _itemRepository.UpsertRangeAsync(items.Select(ConvertToDbModel));
            }
        }

        private static Item ConvertToDbModel(CensusItemModel item)
        {
            return new Item
            {
                Id = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                ItemCategoryId = item.ItemCategoryId,
                IsVehicleWeapon = item.IsVehicleWeapon,
                Name = item.Name?.English,
                Description = item.Description?.English,
                FactionId = item.FactionId,
                MaxStackSize = item.MaxStackSize,
                ImageId = item.ImageId
            };
        }

        private static ItemCategory ConvertToDbModel(CensusItemCategoryModel itemCat)
        {
            return new ItemCategory
            {
                Id = itemCat.ItemCategoryId,
                Name = itemCat.Name?.English
            };
        }
    }
}
