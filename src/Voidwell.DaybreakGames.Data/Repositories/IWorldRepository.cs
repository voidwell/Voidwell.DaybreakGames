using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IWorldRepository
    {
        Task UpsertRangeAsync(IEnumerable<DbWorld> entities);
        Task<IEnumerable<DbWorld>> GetAllWorldsAsync();
    }
}
