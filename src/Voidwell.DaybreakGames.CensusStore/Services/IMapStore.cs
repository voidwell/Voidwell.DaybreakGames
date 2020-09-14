using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public interface IMapStore : IUpdateable
    {
        Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId);
        Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(params int[] facilityIds);
        Task CreateZoneSnapshotAsync(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null, Dictionary<int, int> mapOwnership = null);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEventAsync(int worldId, int metagameInstanceId);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTimeAsync(int worldId, int zoneId, DateTime timestamp);
        Task<IEnumerable<CensusFacilityWorldEventModel>> GetCensusFacilityWorldEventsByZoneIdAsync(int worldId, int zoneId);
    }
}