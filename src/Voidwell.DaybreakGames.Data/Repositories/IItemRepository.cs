using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IItemRepository : IRepository<DbItem>
    {
        Task UpsertRangeAsync(IEnumerable<DbItem> entities);
        Task UpsertRangeAsync(IEnumerable<DbItemCategory> entities);
        Task<IEnumerable<DbItem>> FindItemsByIdsAsync(IEnumerable<string> itemIds);
        Task<IEnumerable<DbItem>> FindItemsByNameAsync(string name, int limit);
    }
}
