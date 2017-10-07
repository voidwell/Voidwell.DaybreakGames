using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Profile
    {
        public static async Task<IEnumerable<JToken>> GetAllProfiles()
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

            return await query.GetBatch();
        }
    }
}
