using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MetagameEventStateCollection : CensusCollection, ICensusStaticCollection<CensusMetagameEventStateModel>
    {
        public override string CollectionName => "metagame_event_state";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public MetagameEventStateCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusMetagameEventStateModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("metagame_event_state_id", "name");

                return query.GetBatchAsync<CensusMetagameEventStateModel>();
            });
        }
    }
}
