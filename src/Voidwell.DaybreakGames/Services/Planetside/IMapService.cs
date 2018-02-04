using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMapService : IUpdateable
    {
        Task<IEnumerable<MapOwnership>> GetMapOwnership(string worldId, string zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegions(string zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinks(string zoneId);
        Task<IEnumerable<MapRegion>> FindRegions(params string[] facilityIds);
    }
}