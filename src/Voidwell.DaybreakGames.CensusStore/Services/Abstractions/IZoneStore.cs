using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IZoneStore
    {
        Task<IEnumerable<Zone>> GetAllZones();
        Task<Zone> GetZoneAsync(int zoneId);
        Task<IEnumerable<Zone>> GetPlayableZones();
    }
}