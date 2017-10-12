using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusMetagameEvent
    {
        public static async Task<IEnumerable<CensusMetagameEventCategoryModel>> GetAllCategories()
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

            return await query.GetBatch<CensusMetagameEventCategoryModel>();
        }

        public static async Task<IEnumerable<CensusMetagameEventStateModel>> GetAllStates()
        {
            var query = new CensusQuery.Query("metagame_event_state");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "metagame_event_state_id",
                "name"
            });

            return await query.GetBatch<CensusMetagameEventStateModel>();
        }
    }
}
