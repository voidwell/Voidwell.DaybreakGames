using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Extensions;

namespace Voidwell.DaybreakGames.CensusStore;

public class StoreUpdaterHelper : IStoreUpdaterHelper
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public StoreUpdaterHelper(IDbContextFactory<PS2DbContext> dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task UpdateAsync<TCollectionEntity, TDataEntity>(Func<Task<IEnumerable<TCollectionEntity>>> collectionFunc)
        where TCollectionEntity : class
        where TDataEntity : class
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var data = await collectionFunc();

        if (data == null)
        {
            return;
        }

        var dataModels = _mapper.Map<IEnumerable<TDataEntity>>(data);

        await dbContext.UpsertAsync(dataModels);
    }
}
