using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveTreeCategoryCollection : ICensusStaticCollection<CensusDirectiveTreeCategoryModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "directive_tree_category";

        public DirectiveTreeCategoryCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusDirectiveTreeCategoryModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .GetBatchAsync<CensusDirectiveTreeCategoryModel>();
        }
    }
}
