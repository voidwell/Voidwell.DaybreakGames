using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusItemCategory
    {
        private readonly ICensusClient _censusClient;

        public CensusItemCategory(ICensusClient censusClient)
        {
            _censusClient = censusClient;
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var query = _censusClient.CreateQuery("item_category");
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
