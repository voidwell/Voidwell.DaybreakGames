using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MapHexStore : IMapHexStore
    {
        private readonly IMapRepository _mapRepository;

        public MapHexStore(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId)
        {
            return _mapRepository.GetMapHexsByZoneIdAsync(zoneId);
        }
    }
}
