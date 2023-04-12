using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IMetagameEventRepository
    {
        Task<MetagameEventCategory> GetMetagameEventCategory(int metagameEventId);
        Task<int?> GetMetagameCategoryZoneId(int metagameEventId);
        Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities);
        Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities);
    }
}
