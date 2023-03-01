using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public interface IZoneStore
    {
        Task<IEnumerable<Zone>> GetAllZones();
        Task<Zone> GetZone(int zoneId);
        Task<IEnumerable<Zone>> GetPlayableZones();
    }
}