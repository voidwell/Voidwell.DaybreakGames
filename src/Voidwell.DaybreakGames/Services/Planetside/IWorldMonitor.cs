using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWorldMonitor
    {
        Task SetWorldState(string worldId, string worldName, bool isOnline);
        Task<bool> TryResetWorld(string worldId);
        FacilityControlChange UpdateFacilityControl(DbEventFacilityControl facilityControl);
        MapScore GetTerritory(string worldId, string zoneId);
        Task<IEnumerable<float>> GetTerritoryFromDate(string worldId, string zoneId, DateTime date);
    }
}
