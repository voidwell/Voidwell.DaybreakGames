using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusTitle
    {
        public static async Task<IEnumerable<CensusTitleModel>> GetAllTitles()
        {
            var query = new CensusQuery.Query("title");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "title_id",
                "name"
            });

            return await query.GetBatch<CensusTitleModel>();
        }
    }
}
