using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ILoadoutRepository
    {
        Task<IEnumerable<Loadout>> GetAllLoadoutsAsync();
        Task UpsertRangeAsync(IEnumerable<Loadout> entities);
    }
}
