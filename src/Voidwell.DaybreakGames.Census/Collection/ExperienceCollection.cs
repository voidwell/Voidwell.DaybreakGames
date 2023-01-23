using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ExperienceCollection : CensusCollection, ICensusStaticCollection<CensusExperienceModel>
    {
        public override string CollectionName => "experience";

        public ExperienceCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusExperienceModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("experience_id", "description", "xp");

                return query.GetBatchAsync<CensusExperienceModel>();
            });
        }
    }
}
