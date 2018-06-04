using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class FactionRepository : IFactionRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public FactionRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task UpsertRangeAsync(IEnumerable<Faction> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Factions.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
