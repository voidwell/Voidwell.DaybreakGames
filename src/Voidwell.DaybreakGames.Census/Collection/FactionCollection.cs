using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class FactionCollection : CensusCollection, ICensusStaticCollection<CensusFactionModel>
    {
        public override string CollectionName => "faction";

        public FactionCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusFactionModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("faction_id", "name", "image_id", "code_tag", "user_selectable");

                return query.GetBatchAsync<CensusFactionModel>();
            });
        }
    }
}
