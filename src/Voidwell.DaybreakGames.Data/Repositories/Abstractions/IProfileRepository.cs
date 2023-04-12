using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IProfileRepository
    {
        Task<IEnumerable<Profile>> GetAllProfilesAsync();
        Task UpsertRangeAsync(IEnumerable<Profile> entities);
    }
}
