using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ExperienceCollection : ICensusStaticCollection<CensusExperienceModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "experience";

        public ExperienceCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusExperienceModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .ShowFields("experience_id", "description", "xp")
                .GetBatchAsync<CensusExperienceModel>();
        }
    }
}
