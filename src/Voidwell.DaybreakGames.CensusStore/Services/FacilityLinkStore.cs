using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class FacilityLinkStore : IFacilityLinkStore
    {
        private readonly IMapRepository _mapRepository;

        public FacilityLinkStore(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
        }
    }
}
