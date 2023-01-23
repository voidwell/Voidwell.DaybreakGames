using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using System;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public abstract class CensusCollection : ICensusCollection
    {
        protected readonly ICensusClient _censusClient;

        public CensusCollection(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public abstract string CollectionName { get; }

        protected virtual Task<T> QueryAsync<T>(Func<CensusQuery, Task<T>> queryFunc)
        {
            return queryFunc(_censusClient.CreateQuery(CollectionName));
        }
    }
}
