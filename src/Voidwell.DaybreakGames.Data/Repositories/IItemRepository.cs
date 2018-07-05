using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task UpsertRangeAsync(IEnumerable<Item> entities);
        Task UpsertRangeAsync(IEnumerable<ItemCategory> entities);
        Task<IEnumerable<Item>> FindItemsByIdsAsync(IEnumerable<int> itemIds);
        Task<IEnumerable<Item>> FindItemsByNameAsync(string name, int limit);
        Task<IEnumerable<Item>> GetItemsByCategoryIds(IEnumerable<int> categoryIds);
    }
}
