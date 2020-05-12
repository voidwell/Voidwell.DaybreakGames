using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IZoneService
    {
        Task RefreshStore();
        Task<IEnumerable<Zone>> GetAllZones();
        Task<Zone> GetZone(int zoneId);
        Task<IEnumerable<Zone>> GetPlayableZones();
    }
}
