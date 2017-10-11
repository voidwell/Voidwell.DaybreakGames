using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusFaction
    {
        public static async Task<IEnumerable<JToken>> GetAllFactions()
        {
            var query = new CensusQuery.Query("faction");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "factiion_id",
                "name",
                "image_id",
                "code_tag",
                "user_selectable"
            });

            return await query.GetBatch();
        }
    }
}
