using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class LoadoutCollection : CensusCollection, ICensusStaticCollection<CensusLoadoutModel>
    {
        public override string CollectionName => "loadout";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public LoadoutCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusLoadoutModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("loadout_id", "profile_id", "faction_id", "code_name");

                return query.GetBatchAsync<CensusLoadoutModel>();
            });
        }
    }
}
