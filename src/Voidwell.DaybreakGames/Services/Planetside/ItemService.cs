using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly CensusItem _censusItem;
        private readonly CensusItemCategory _censusItemCategory;

        public string ServiceName => "ItemService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ItemService(IItemRepository itemRepository, CensusItem censusItem, CensusItemCategory censusItemCategory)
        {
            _itemRepository = itemRepository;
            _censusItem = censusItem;
            _censusItemCategory = censusItemCategory;
        }

        public Task<IEnumerable<DbItem>> FindItems(IEnumerable<string> itemIds)
        {
            return _itemRepository.FindItemsByIdsAsync(itemIds);
        }

        public Task<IEnumerable<DbItem>> LookupItemsByName(string name, int limit = 12)
        {
            return _itemRepository.FindItemsByNameAsync(name, limit);
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

        private DbItem ConvertToDbModel(CensusItemModel item)
        {
            return new DbItem
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

        private DbItemCategory ConvertToDbModel(CensusItemCategoryModel itemCat)
        {
            return new DbItemCategory
            {
                Id = itemCat.ItemCategoryId,
                Name = itemCat.Name?.English
            };
        }
    }
}
