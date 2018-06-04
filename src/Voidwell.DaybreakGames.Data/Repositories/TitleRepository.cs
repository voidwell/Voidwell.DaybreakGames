using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public TitleRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task UpdateRangeAsync(IEnumerable<Title> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Titles.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
