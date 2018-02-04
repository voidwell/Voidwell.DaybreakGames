using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class UpdaterSchedulerRepository : IUpdaterSchedulerRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public UpdaterSchedulerRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public UpdaterScheduler GetUpdaterHistoryByServiceName(string serviceName)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return dbContext.UpdaterScheduler.SingleOrDefault(u => u.ServiceName == serviceName);
            }
        }

        public async Task UpsertAsync(UpdaterScheduler entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.UpdaterScheduler;

                var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.ServiceName == entity.ServiceName);
                if (storeEntity == null)
                {
                    dbSet.Add(entity);
                }
                else
                {
                    storeEntity = entity;
                    dbSet.Update(storeEntity);
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
