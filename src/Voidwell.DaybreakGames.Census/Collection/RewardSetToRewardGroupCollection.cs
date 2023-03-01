using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class RewardSetToRewardGroupCollection : CensusCollection, ICensusStaticCollection<CensusRewardSetToRewardGroupModel>
    {
        public override string CollectionName => "reward_set_to_reward_group";

        public RewardSetToRewardGroupCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusRewardSetToRewardGroupModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                return query.GetBatchAsync<CensusRewardSetToRewardGroupModel>();
            });
        }
    }
}
