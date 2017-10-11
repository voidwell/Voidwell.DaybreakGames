using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusWorld
    {
        public static async Task<IEnumerable<CensusWorldModel>> GetAllWorlds ()
        {
            var query = new CensusQuery.Query("world");
            query.SetLanguage("en");

            return await query.GetBatch<CensusWorldModel>();
        }
    }
}
