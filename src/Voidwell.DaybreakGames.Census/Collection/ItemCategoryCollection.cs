using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ItemCategoryCollection : CensusPatchCollection, ICensusStaticCollection<CensusItemCategoryModel>
    {
        public override string CollectionName => "item_category";

        public ItemCategoryCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("item_category_id", "name");

                return query.GetBatchAsync<CensusItemCategoryModel>();
            });
        }
    }
}
