using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ItemService : IItemService
    {
        private readonly IItemStore _itemStore;

        public ItemService(IItemStore itemStore)
        {
            _itemStore = itemStore;
        }

        public Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds)
        {
            return _itemStore.FindItemsByIdsAsync(itemIds);
        }

        public async Task<IEnumerable<Item>> LookupWeaponsByName(string name, int limit = 12)
        {
            var results = await _itemStore.FindWeaponsByNameAsync(name, limit);

            var orderedResults = new List<Item>();

            //Exact match
            foreach (var result in results.Except(orderedResults))
            {
                if (result.Name == name)
                {
                    orderedResults.Add(result);
                }
            }

            //Starts with match
            foreach (var result in results.Except(orderedResults))
            {
                if (result.Name.StartsWith(name))
                {
                    orderedResults.Add(result);
                }
            }

            //Case insensitive exact match
            foreach (var result in results.Except(orderedResults))
            {
                if (string.Equals(result.Name, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    orderedResults.Add(result);
                }
            }

            //Case insensitive starts with
            foreach (var result in results.Except(orderedResults))
            {
                if (result.Name.ToLower().StartsWith(name.ToLower()))
                {
                    orderedResults.Add(result);
                }
            }

            //Everything else
            orderedResults.AddRange(results.Except(orderedResults));

            return orderedResults;
        }

        public Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds)
        {
            return _itemStore.GetItemsByCategoryIdsAsync(categoryIds);
        }

        public Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId)
        {
            return _itemStore.GetWeaponInfoAsync(weaponItemId);
        }
    }
}
