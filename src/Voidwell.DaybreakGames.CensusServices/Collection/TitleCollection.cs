using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class TitleCollection : CensusCollection, ICensusStaticCollection<CensusTitleModel>
    {
        public override string CollectionName => "title";

        public TitleCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusTitleModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("title_id", "name");

                return query.GetBatchAsync<CensusTitleModel>();
            });
        }
    }
}
