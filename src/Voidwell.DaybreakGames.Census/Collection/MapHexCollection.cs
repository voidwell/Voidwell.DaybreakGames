using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MapHexCollection : CensusPatchCollection, ICensusStaticCollection<CensusMapHexModel>
    {
        public override string CollectionName => "map_hex";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public MapHexCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusMapHexModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("zone_id", "map_region_id", "x", "y", "hex_type", "type_name");

                return query.GetBatchAsync<CensusMapHexModel>();
            });
        }
    }
}
