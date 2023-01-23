using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public interface IMapHexStore : IUpdateable
    {
        Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId);
    }
}