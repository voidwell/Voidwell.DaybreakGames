using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusMetagameEvent
    {
        public static async Task<IEnumerable<JToken>> GetAllCategories()
        {
            var query = new CensusQuery.Query("metagame_event");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "metagame_event_id",
                "name",
                "description",
                "type",
                "experience_bonus"
            });

            return await query.GetBatch();
        }

        public static async Task<IEnumerable<JToken>> GetAllStates()
        {
            var query = new CensusQuery.Query("metagame_event_state");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "metagame_event_state_id",
                "name"
            });

            return await query.GetBatch();
        }
    }
}
