using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IItemService
    {
        Task<IEnumerable<DbItem>> FindItems(params string[] itemIds);
        Task<IEnumerable<DbItem>> LookupItemsByName(string name, int limit = 12);
        Task RefreshStore();
    }
}