using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.GameState
{
    public class WorldMonitor : IWorldMonitor
    {
        private readonly IZoneMonitor _zoneMonitor;
        private readonly IPlayerMonitor _playerMonitor;

        private readonly Dictionary<int, bool> _worldStates = new Dictionary<int, bool>();

        public WorldMonitor(IZoneMonitor zoneMonitor, IPlayerMonitor playerMonitor)
        {
            _zoneMonitor = zoneMonitor;
            _playerMonitor = playerMonitor;
        }

        public async Task SetWorldOnlineAsync(int worldId)
        {
            _worldStates[worldId] = true;
            await _zoneMonitor.LoadAllWorldOwnerships(worldId);
        }

        public async Task SetWorldOfflineAsync(int worldId)
        {
            _worldStates[worldId] = false;
            _zoneMonitor.ClearZoneStates(worldId);
            await _playerMonitor.ClearAllCharactersAsync(worldId);
        }

        public bool IsWorldOnline(int worldId)
        {
            return _worldStates.ContainsKey(worldId) ? _worldStates[worldId] : false;
        }
    }
}
