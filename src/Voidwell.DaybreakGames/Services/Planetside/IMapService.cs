using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMapService : IUpdateable
    {
        Task<IEnumerable<MapOwnership>> GetMapOwnership(int worldId, int zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegions(int zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinks(int zoneId);
        Task<IEnumerable<MapRegion>> FindRegions(params int[] facilityIds);
    }
}