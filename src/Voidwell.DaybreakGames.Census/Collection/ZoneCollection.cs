using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class ZoneCollection : CensusPatchCollection, ICensusStaticCollection<CensusZoneModel>
    {
        public string CollectionName => "zone";

        public ZoneCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient) : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusZoneModel>> GetCollectionAsync()
        {
            return await QueryAsync(CollectionName, query =>
                query.SetLanguage("en")
                    .ShowFields("zone_id", "code", "name", "description", "hex_size")
                    .GetBatchAsync<CensusZoneModel>());
        }
    }
}
