using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ObjectiveCollection : CensusCollection, ICensusStaticCollection<CensusObjectiveModel>
    {
        public override string CollectionName => "objective";

        public ObjectiveCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusObjectiveModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                return query.GetBatchAsync<CensusObjectiveModel>();
            });
        }
    }
}
