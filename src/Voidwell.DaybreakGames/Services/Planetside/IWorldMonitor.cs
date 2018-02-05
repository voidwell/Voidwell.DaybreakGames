using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWorldMonitor
    {
        Task SetWorldState(int worldId, string worldName, bool isOnline);
        Task<bool> TryResetWorld(int worldId);
        FacilityControlChange UpdateFacilityControl(FacilityControl facilityControl);
        MapScore GetTerritory(int worldId, int zoneId);
        Task<IEnumerable<float>> GetTerritoryFromDate(int worldId, int zoneId, DateTime date);
        Task SetPlayerOnlineState(string characterId, DateTime timestamp, bool isOnline);
        IEnumerable<OnlineCharacter> GetOnlineCharactersByWorld(int worldId);
        IEnumerable<WorldOnlineState> GetWorldStates();
    }
}
