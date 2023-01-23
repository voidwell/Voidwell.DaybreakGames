using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class WorldCollection : CensusCollection, ICensusStaticCollection<CensusWorldModel>
    {
        public override string CollectionName => "world";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public WorldCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusWorldModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                return query.GetBatchAsync<CensusWorldModel>();
            });
        }
    }
}
