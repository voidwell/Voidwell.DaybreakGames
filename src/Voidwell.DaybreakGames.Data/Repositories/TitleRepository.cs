using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

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

            await dbContext.UpsertAsync(entities);
        }
    }
}
