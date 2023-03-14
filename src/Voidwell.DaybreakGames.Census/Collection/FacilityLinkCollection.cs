using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class FacilityLinkCollection : CensusPatchCollection, ICensusStaticCollection<CensusFacilityLinkModel>
    {
        public string CollectionName => "facility_link";

        public FacilityLinkCollection(ICensusPatchClient censusPatchClient, ICensusClient censusClient)
            : base(censusPatchClient, censusClient)
        {
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetCollectionAsync()
        {
            return await QueryAsync(CollectionName, query =>
                query.Where("zone_id", a => a.IsLessThan(300))
                    .ShowFields("zone_id", "facility_id_a", "facility_id_b", "description")
                    .GetBatchAsync<CensusFacilityLinkModel>());
        }
    }
}
