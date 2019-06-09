using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IItemService : IUpdateable
    {
        Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> LookupItemsByName(string name, int limit = 12);
        Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds);
    }
}