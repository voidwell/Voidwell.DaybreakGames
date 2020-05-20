using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.GameState
{
    public interface IZoneMonitor
    {
        Task ZoneLayoutSetup();
        Task LoadAllWorldOwnerships(int worldId);
        Task LoadWorldOwnership(int worldId, int zoneId);
        void ClearZoneStates(int worldId);
        void ClearAllZoneStates();
        void UpdateRegionOwnership(int worldId, int regionId, int newFactionId);
        IEnumerable<(int regionId, int factionId)> GetZoneOwnership(int worldId, int zoneId);
        MapScore GetZoneScore(int worldId, int zoneId);
    }
}