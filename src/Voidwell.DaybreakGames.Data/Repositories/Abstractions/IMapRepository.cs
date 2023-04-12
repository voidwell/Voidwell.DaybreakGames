﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface IMapRepository
    {
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId);
        Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<int> facilityIds);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId);
        Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp);
        Task InsertRangeAsync(IEnumerable<ZoneOwnershipSnapshot> entities);
        Task UpsertRangeAsync(IEnumerable<MapHex> entities);
        Task UpsertRangeAsync(IEnumerable<MapRegion> entities);
        Task UpsertRangeAsync(IEnumerable<FacilityLink> entities);
    }
}
