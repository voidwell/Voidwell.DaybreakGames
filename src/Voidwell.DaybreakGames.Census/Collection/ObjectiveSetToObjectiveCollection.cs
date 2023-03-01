using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ObjectiveSetToObjectiveCollection : CensusCollection, ICensusStaticCollection<CensusObjectiveSetToObjectiveModel>
    {
        public override string CollectionName => "objective_set_to_objective";

        public ObjectiveSetToObjectiveCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusObjectiveSetToObjectiveModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                return query.GetBatchAsync<CensusObjectiveSetToObjectiveModel>();
            });
        }
    }
}
