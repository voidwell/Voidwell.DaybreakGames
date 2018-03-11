using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IItemService : IUpdateable
    {
        Task<IEnumerable<Item>> FindItems(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> LookupItemsByName(string name, int limit = 12);
    }
}