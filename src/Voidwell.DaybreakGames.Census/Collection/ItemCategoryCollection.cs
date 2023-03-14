using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ItemCategoryCollection : CensusPatchCollection, ICensusStaticCollection<CensusItemCategoryModel>
    {
        public string CollectionName => "item_category";

        public ItemCategoryCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetCollectionAsync()
        {
            return await QueryAsync(CollectionName, query =>
                query.SetLanguage("en")
                    .ShowFields("item_category_id", "name")
                    .GetBatchAsync<CensusItemCategoryModel>());
        }
    }
}
