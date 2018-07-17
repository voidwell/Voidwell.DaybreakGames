using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusExperience
    {
        private readonly ICensusClient _censusClient;

        public CensusExperience(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusExperienceModel>> GetAllExperience()
        {
            var query = _censusClient.CreateQuery("experience");

            query.ShowFields(new[]
            {
                "experience_id",
                "description",
                "xp"
            });

            return await query.GetBatch<CensusExperienceModel>();
        }
    }
}
