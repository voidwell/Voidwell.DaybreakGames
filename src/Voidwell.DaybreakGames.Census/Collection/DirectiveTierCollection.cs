using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class DirectiveTierCollection : CensusCollection, ICensusStaticCollection<CensusDirectiveTierModel>
    {
        public override string CollectionName => "directive_tier";

        public DirectiveTierCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusDirectiveTierModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                return query.GetBatchAsync<CensusDirectiveTierModel>();
            });
        }
    }
}
