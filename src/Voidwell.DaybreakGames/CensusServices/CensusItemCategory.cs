using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusItemCategory
    {
        public static async Task<IEnumerable<JToken>> GetAllItemCategories()
        {
            var query = new CensusQuery.Query("item_category");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "item_category_id",
                "name"
            });

            return await query.GetBatch();
        }
    }
}
