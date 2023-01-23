using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ZoneCollection : CensusPatchCollection, ICensusStaticCollection<CensusZoneModel>
    {
        public override string CollectionName => "zone";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public ZoneCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient) : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusZoneModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.SetLanguage("en");

                query.ShowFields("zone_id", "code", "name", "description", "hex_size");

                return query.GetBatchAsync<CensusZoneModel>();
            });
        }
    }
}
