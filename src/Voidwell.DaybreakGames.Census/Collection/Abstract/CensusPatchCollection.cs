using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection.Abstract
{
    public abstract class CensusPatchCollection : CensusCollection
    {
        protected readonly ICensusPatchClient _patchClient;

        protected CensusPatchCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusClient)
        {
            _patchClient = censusPatchClient;
        }

        protected override async Task<T> QueryAsync<T>(Func<CensusQuery, Task<T>> queryFunc)
        {
            try
            {
                return await queryFunc(_patchClient.CreateQuery(CollectionName));
            }
            catch (Exception)
            {
                return await base.QueryAsync(queryFunc);
            }
        }
    }
}
