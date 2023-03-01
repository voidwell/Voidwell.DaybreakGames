﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public interface IItemStore
    {
        Task<IEnumerable<Item>> FindItemsByIdsAsync(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> GetItemsByCategoryIdsAsync(IEnumerable<int> categoryIds);
        Task<IEnumerable<Item>> FindWeaponsByNameAsync(string name, int limit = 12);
        Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId);
    }
}