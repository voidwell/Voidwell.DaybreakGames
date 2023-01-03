using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusItemCategory : ICensusItemCategory
    {
        private readonly ICensusClient _client;

        public CensusItemCategory(ICensusClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var query = _client.CreateQuery("item_category");
            query.SetLanguage("en");

            query.ShowFields("item_category_id", "name");

            return await query.GetBatchAsync<CensusItemCategoryModel>();
        }
    }
}
