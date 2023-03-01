using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class ObjectiveRepository : IObjectiveRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ObjectiveRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Objective>> GetObjectivesByGroupIdAsync(IEnumerable<int> groupIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Objectives
                    .Where(a => groupIds.Contains(a.ObjectiveGroupId))
                    .ToListAsync();

            }
        }
    }
}
