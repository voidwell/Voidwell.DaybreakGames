using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public abstract class CensusPatchCollection
    {
        protected readonly ICensusPatchClient _patchClient;
        protected readonly ICensusClient _censusClient;

        protected CensusPatchCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
        {
            _patchClient = censusPatchClient;
            _censusClient = censusClient;
        }

        protected async Task<T> QueryAsync<T>(string collectionName, Func<CensusQuery, Task<T>> queryFunc)
        {
            try
            {
                return await queryFunc(_patchClient.CreateQuery(collectionName));
            }
            catch (Exception)
            {
                return await queryFunc(_censusClient.CreateQuery(collectionName));
            }
        }
    }
}
