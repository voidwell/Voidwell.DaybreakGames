using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class MapRegionCollection : CensusPatchCollection, ICensusStaticCollection<CensusMapRegionModel>
    {
        public override string CollectionName => "map_region";

        public MapRegionCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetCollectionAsync()
        {
            return await QueryAsync(query =>
            {
                query.ShowFields("map_region_id", "zone_id", "facility_id", "facility_name", "facility_type_id", "facility_type", "location_x", "location_y", "location_z");

                return query.GetBatchAsync<CensusMapRegionModel>();
            });
        }
    }
}
