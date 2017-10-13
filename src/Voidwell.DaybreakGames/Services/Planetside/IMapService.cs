using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMapService
    {
        Task<IEnumerable<MapOwnership>> GetMapOwnership(string worldId, string zoneId);
        Task<IEnumerable<DbMapRegion>> GetMapRegions(string zoneId);
        Task<IEnumerable<DbFacilityLink>> GetFacilityLinks(string zoneId);
        Task<IEnumerable<DbMapRegion>> FindRegions(params string[] facilityIds);
        Task RefreshStore();
    }
}