using System.Collections.Generic;
using System.Threading.Tasks;
using DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusItemCategory
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusItemCategory(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusItemCategoryModel>> GetAllItemCategories()
        {
            var query = _queryFactory.Create("item_category");
            query.SetLanguage("en");

            query.ShowFields(new[]
            {
                "item_category_id",
                "name"
            });

            return await query.GetBatchAsync<CensusItemCategoryModel>();
        }
    }
}
