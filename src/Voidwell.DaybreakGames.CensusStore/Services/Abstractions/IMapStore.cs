using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IMapStore
    {
        Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId);
        Task CreateZoneSnapshotAsync(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, Dictionary<int, int> mapOwnership = null);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEventAsync(int worldId, int metagameInstanceId);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTimeAsync(int worldId, int zoneId, DateTime timestamp);
        Task<IEnumerable<CensusFacilityWorldEventModel>> GetCensusFacilityWorldEventsByZoneIdAsync(int worldId, int zoneId);
    }
}