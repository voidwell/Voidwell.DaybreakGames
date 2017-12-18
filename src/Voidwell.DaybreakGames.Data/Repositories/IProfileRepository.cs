using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<DbProfile>> GetAllProfilesAsync();
        Task UpsertRangeAsync(IEnumerable<DbProfile> entities);
    }
}
