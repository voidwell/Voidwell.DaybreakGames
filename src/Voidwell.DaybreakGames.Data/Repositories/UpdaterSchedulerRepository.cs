using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.Microservice.EntityFramework;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class UpdaterSchedulerRepository : IUpdaterSchedulerRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public UpdaterSchedulerRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<UpdaterScheduler> GetUpdaterHistoryByServiceNameAsync(string serviceName)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.UpdaterScheduler.SingleOrDefaultAsync(u => u.Id == serviceName);
            }
        }

        public async Task UpsertAsync(UpdaterScheduler entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entity);
            }
        }
    }
}
