using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.Microservice.EntityFramework;

namespace Voidwell.DaybreakGames.CensusStore
{
    public class StoreUpdaterHelper : IStoreUpdaterHelper
    {
        private readonly IDbContextHelper _dbContextHelper;
        private readonly IMapper _mapper;

        public StoreUpdaterHelper(IDbContextHelper dbContextHelper, IMapper mapper)
        {
            _dbContextHelper = dbContextHelper;
            _mapper = mapper;
        }

        public async Task UpdateAsync<TCollectionEntity, TDataEntity>(Func<Task<IEnumerable<TCollectionEntity>>> collectionFunc)
            where TCollectionEntity : class
            where TDataEntity : class
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var data = await collectionFunc();

                if (data == null)
                {
                    return;
                }

                var dataModels = _mapper.Map<IEnumerable<TDataEntity>>(data);

                await dbContext.UpsertAsync(dataModels);
            }
        }
    }
}
