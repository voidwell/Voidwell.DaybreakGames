using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class World
    {
        public static async Task<IEnumerable<JToken>> GetAllWorlds ()
        {
            var query = new CensusQuery.Query("world");
            query.SetLanguage("en");

            return await query.GetBatch();
        }
    }
}
