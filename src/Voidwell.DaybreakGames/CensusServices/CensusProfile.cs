using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusProfile
    {
        private readonly ICensusClient _censusClient;

        public CensusProfile(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusProfileModel>> GetAllProfiles()
        {
            var query = _censusClient.CreateQuery("profile");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "profile_id",
                "profile_type_id",
                "faction_id",
                "name",
                "image_id"
            });

            return await query.GetBatch<CensusProfileModel>();
        }
    }
}
