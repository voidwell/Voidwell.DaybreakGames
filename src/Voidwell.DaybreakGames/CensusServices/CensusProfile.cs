using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusProfile
    {
        public static async Task<IEnumerable<CensusProfileModel>> GetAllProfiles()
        {
            var query = new CensusQuery.Query("profile");
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
