using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IMetagameEventRepository
    {
        Task UpsertRangeAsync(IEnumerable<DbMetagameEventCategory> entities);
        Task UpsertRangeAsync(IEnumerable<DbMetagameEventState> entities);
    }
}
