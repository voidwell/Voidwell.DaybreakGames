using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IExperienceRepository
    {
        Task<Experience> GetExperienceById(int experienceId);
        Task UpsertRangeAsync(IEnumerable<Experience> entities);
    }
}