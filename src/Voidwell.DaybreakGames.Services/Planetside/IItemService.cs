using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> LookupWeaponsByName(string name, int limit = 12);
        Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds);
        Task<CensusWeaponInfoModel> GetWeaponInfoAsync(int weaponItemId);
    }
}