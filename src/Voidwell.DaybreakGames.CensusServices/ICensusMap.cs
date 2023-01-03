using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public interface ICensusMap
    {
        Task<CensusMapModel> GetMapOwnership(int worldId, int zoneId);
        Task<IEnumerable<CensusMapHexModel>> GetAllMapHexs();
        Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions();
        Task<IEnumerable<CensusFacilityLinkModel>> GetAllFacilityLinks();
    }
}