using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class FactionCollection : ICensusStaticCollection<CensusFactionModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "faction";

        public FactionCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusFactionModel>> GetCollectionAsync()
        {
            return await _client.CreateQuery(CollectionName)
                .SetLanguage("en")
                .ShowFields("faction_id", "name", "image_id", "code_tag", "user_selectable")
                .GetBatchAsync<CensusFactionModel>();
        }
    }
}
