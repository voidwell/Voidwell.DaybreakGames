using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ProfileCollection : CensusCollection, ICensusStaticCollection<CensusProfileModel>
    {
        public override string CollectionName => "profile";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public ProfileCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusProfileModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("profile_id", "profile_type_id", "faction_id", "name", "image_id");

                return query.GetBatchAsync<CensusProfileModel>();
            });
        }
    }
}
