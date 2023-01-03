using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices.Patcher.Services
{
    public class PatchItemCategory : ICensusItemCategory
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusItemCategory _censusClient;

        public PatchItemCategory(IPatchClient client, CensusItemCategory censusItemCategory)
        {
            _patchClient = client;
            _censusClient = censusItemCategory;
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var censusResults = await _censusClient.GetAllItemCategories();

            var query = _patchClient.CreateQuery("item_category");
            query.SetLanguage("en");

            query.ShowFields("item_category_id", "name");

            var patchResults = await query.GetBatchAsync<CensusItemCategoryModel>();

            return PatchUtil.PatchData<CensusItemCategoryModel>(x => x.ItemCategoryId, censusResults, patchResults);

        }
    }
}
