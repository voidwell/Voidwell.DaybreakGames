using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusItemCategory
    {
        public static async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var query = new CensusQuery.Query("item_category");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "item_category_id",
                "name"
            });

            return await query.GetBatch<CensusItemCategoryModel>();
        }
    }
}
