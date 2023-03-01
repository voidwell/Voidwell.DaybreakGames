using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardCollection : CensusCollection, ICensusStaticCollection<CensusRewardModel>
    {
        public override string CollectionName => "reward";

        public RewardCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusRewardModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                return query.GetBatchAsync<CensusRewardModel>();
            });
        }
    }
}
