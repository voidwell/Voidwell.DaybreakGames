using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Microsoft.EntityFrameworkCore;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ItemService : IItemService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusItem _censusItem;
        private readonly CensusItemCategory _censusItemCategory;

        public ItemService(PS2DbContext ps2DbContext, CensusItem censusItem, CensusItemCategory censusItemCategory)
        {
            _ps2DbContext = ps2DbContext;
            _censusItem = censusItem;
            _censusItemCategory = censusItemCategory;
        }

        public async Task<IEnumerable<DbItem>> FindItems(IEnumerable<string> itemIds)
        {
            return await _ps2DbContext.Items.Where(i => itemIds.Contains(i.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<DbItem>> LookupItemsByName(string name, int limit = 12)
        {
            return await _ps2DbContext.Items.Where(i => i.Name.Contains(name))
                .Take(limit)
                .ToListAsync();
        }

        public async Task RefreshStore()
        {
            var items = await _censusItem.GetAllItems();
            var itemCategories = await _censusItemCategory.GetAllItemCategories();

            if (items != null)
            {
                _ps2DbContext.UpdateRange(items.Select(i => ConvertToDbModel(i)));
            }

            if (itemCategories != null)
            {
                _ps2DbContext.UpdateRange(itemCategories.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbItem ConvertToDbModel(CensusItemModel item)
        {
            return new DbItem
            {
                Id = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                ItemCategoryId = item.ItemCategoryId,
                IsVehicleWeapon = item.IsVehicleWeapon,
                Name = item.Name.English,
                Description = item.Description.English,
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
                Name = itemCat.Name.English
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
