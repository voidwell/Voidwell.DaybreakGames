using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveCollection : CensusCollection, ICensusStaticCollection<CensusDirectiveModel>
    {
        public override string CollectionName => "directive";

        public DirectiveCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusDirectiveModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                return query.GetBatchAsync<CensusDirectiveModel>();
            });
        }
    }
}
