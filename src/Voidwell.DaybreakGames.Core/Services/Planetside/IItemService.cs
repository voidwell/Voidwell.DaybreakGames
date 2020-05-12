using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IItemService
    {
        Task RefreshStore();
        Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> LookupWeaponsByName(string name, int limit = 12);
        Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds);
    }
}