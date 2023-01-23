using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MetagameEventCollection : CensusCollection, ICensusStaticCollection<CensusMetagameEventCategoryModel>
    {
        public override string CollectionName => "metagame_event";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public MetagameEventCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusMetagameEventCategoryModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("metagame_event_id", "name", "description", "type", "experience_bonus");

                return query.GetBatchAsync<CensusMetagameEventCategoryModel>();
            });
        }
    }
}
