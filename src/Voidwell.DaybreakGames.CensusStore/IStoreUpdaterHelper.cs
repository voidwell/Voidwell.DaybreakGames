using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStore
{
    public interface IStoreUpdaterHelper
    {
        Task UpdateAsync<TCollectionEntity, TDataEntity>(Func<Task<IEnumerable<TCollectionEntity>>> collectionFunc)
            where TCollectionEntity : class
            where TDataEntity : class;
    }
}