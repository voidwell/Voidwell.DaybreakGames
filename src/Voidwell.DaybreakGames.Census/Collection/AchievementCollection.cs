using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class AchievementCollection : ICensusStaticCollection<CensusAchievementModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "achievement";

        public AchievementCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusAchievementModel>> GetCollectionAsync()
        {
            return await _client
                .CreateQuery(CollectionName)
                .SetLanguage("en")
                .GetBatchAsync<CensusAchievementModel>();
        }
    }
}
