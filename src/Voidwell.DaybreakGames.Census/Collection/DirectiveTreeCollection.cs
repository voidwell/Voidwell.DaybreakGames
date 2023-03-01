using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveTreeCollection : CensusCollection, ICensusStaticCollection<CensusDirectiveTreeModel>
    {
        public override string CollectionName => "directive_tree";

        public DirectiveTreeCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusDirectiveTreeModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                return query.GetBatchAsync<CensusDirectiveTreeModel>();
            });
        }
    }
}
