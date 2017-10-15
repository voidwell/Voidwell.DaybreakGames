using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWorldMonitor
    {
        Task SetWorldState(string worldId, string worldName, bool isOnline);
        Task<bool> TryResetWorld(string worldId);
        FacilityControlChange UpdateFacilityControl(FacilityControl facilityControl);
        MapScore GetTerritory(string worldId, string zoneId);
        Task<IEnumerable<float>> GetTerritoryFromDate(string worldId, string zoneId, DateTime date);
        Task SetPlayerOnlineState(string characterId, DateTime timestamp, bool isOnline);
        IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(string worldId);
    }
}
