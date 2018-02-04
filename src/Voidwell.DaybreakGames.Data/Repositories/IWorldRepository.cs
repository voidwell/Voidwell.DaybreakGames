using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IWorldRepository
    {
        Task UpsertRangeAsync(IEnumerable<World> entities);
        Task<IEnumerable<World>> GetAllWorldsAsync();
    }
}
