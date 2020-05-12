using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IMapService
    {
        Task RefreshStore();
        Task<ZoneMap> GetZoneMap(int zoneId);
        Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(int worldId, int zoneId);
        Task<IEnumerable<MapRegion>> FindRegions(params int[] facilityIds);
        Task CreateZoneSnapshot(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, IEnumerable<ZoneRegionOwnership> zoneOwnership = null);
        Task<ZoneSnapshot> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId);
        Task<ZoneSnapshot> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp);
        Task<ZoneStateHistorical> GetZoneStateHistoricals();
        Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnershipFromHistory(int worldId, int zoneId);
    }
}