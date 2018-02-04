using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IMetagameEventRepository
    {
        Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities);
        Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities);
    }
}
