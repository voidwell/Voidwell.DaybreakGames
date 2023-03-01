using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveTreeCategoryCollection : CensusCollection, ICensusStaticCollection<CensusDirectiveTreeCategoryModel>
    {
        public override string CollectionName => "directive_tree_category";

        public DirectiveTreeCategoryCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusDirectiveTreeCategoryModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                return query.GetBatchAsync<CensusDirectiveTreeCategoryModel>();
            });
        }
    }
}
