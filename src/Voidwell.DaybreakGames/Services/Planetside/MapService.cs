using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService
    {
        private readonly IMapRepository _mapRepository;
        private readonly CensusMap _censusMap;
        private readonly ICache _cache;

        public string ServiceName => "MapService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private readonly KeyedSemaphoreSlim _zoneMapLock = new KeyedSemaphoreSlim();

        private const string _cacheKeyPrefix = "ps2.map_service";
        private TimeSpan _zoneMapCacheExpiration = TimeSpan.FromHours(24);

        public MapService(IMapRepository mapRepository, CensusMap censusMap, ICache cache)
        {
            _mapRepository = mapRepository;
            _censusMap = censusMap;
            _cache = cache;
        }

        public async Task<ZoneMap> GetZoneMap(int zoneId)
        {
            using (await _zoneMapLock.WaitAsync(zoneId.ToString()))
            {
                var cacheKey = GetCacheKey($"zoneMap-{zoneId}");

                var zoneMap = await _cache.GetAsync<ZoneMap>(cacheKey);
                if (zoneMap != null)
                {
                    return zoneMap;
                }

                var linksTask = _mapRepository.GetFacilityLinksByZoneIdAsync(zoneId);
                var hexsTask = _mapRepository.GetMapHexsByZoneIdAsync(zoneId);
                var regionsTask = _mapRepository.GetMapRegionsByZoneIdAsync(zoneId);

                await Task.WhenAll(linksTask, hexsTask, regionsTask);

                var links = linksTask.Result.Select(a => new ZoneLink
                {
                    FacilityIdA = a.FacilityIdA,
                    FacilityIdB = a.FacilityIdB
                });

                var hexs = hexsTask.Result.Select(a => new ZoneHex
                {
                    MapRegionId = a.MapRegionId,
                    X = a.XPos,
                    Y = a.YPos
                });

                var regions = regionsTask.Result
                    .Where(a => links.Any(b => a.FacilityId == b.FacilityIdA || a.FacilityId == b.FacilityIdB))
                    .Select(a => new ZoneRegion
                    {
                        RegionId = a.Id,
                        FacilityId = a.FacilityId,
                        FacilityName = a.FacilityName,
                        FacilityType = a.FacilityType,
                        FacilityTypeId = a.FacilityTypeId,
                        X = a.XPos,
                        Y = a.YPos,
                        Z = a.ZPos
                    });

                zoneMap = new ZoneMap
                {
                    Regions = regions,
                    Hexs = hexs,
                    Links = links
                };

                await _cache.SetAsync(cacheKey, zoneMap, _zoneMapCacheExpiration);

                return zoneMap;
            }
        }

        public async Task<IEnumerable<MapOwnership>> GetMapOwnership(int worldId, int zoneId)
        {
            var ownership = await _censusMap.GetMapOwnership(worldId, zoneId);

            return ownership?.Regions.Row.Select(o => new MapOwnership(o.RowData.RegionId, o.RowData.FactionId));
        }

        public Task<IEnumerable<MapRegion>> FindRegions(params int[] facilityIds)
        {
            return _mapRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task CreateZoneSnapshot(int worldId, int zoneId, DateTime? timestamp = null, int? metagameInstanceId = null)
        {
            if (timestamp == null)
            {
                timestamp = DateTime.UtcNow;
            }

            var zoneOwnership = await GetMapOwnership(worldId, zoneId);
            if (zoneOwnership == null)
            {
                return;
            }

            var snapshotRegions = zoneOwnership.Select(a =>
            {
                return new ZoneOwnershipSnapshot
                {
                    Timestamp = timestamp.Value,
                    WorldId = worldId,
                    ZoneId = zoneId,
                    MetagameInstanceId = metagameInstanceId,
                    RegionId = a.RegionId,
                    FactionId = a.FactionId
                };
            });

            await _mapRepository.InsertRangeAsync(snapshotRegions);
        }

        public async Task<ZoneSnapshot> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
        {
            var snapshotRegions = await _mapRepository.GetZoneSnapshotByMetagameEvent(worldId, metagameInstanceId);
            if (snapshotRegions == null || snapshotRegions.Count() == 0)
            {
                return null;
            }

            return new ZoneSnapshot
            {
                Timestamp = snapshotRegions.First().Timestamp,
                WorldId = snapshotRegions.First().WorldId,
                ZoneId = snapshotRegions.First().ZoneId,
                MetagameInstanceId = snapshotRegions.First().MetagameInstanceId,
                Ownership = snapshotRegions.Select(a => new MapOwnership(a.RegionId, a.FactionId))
            };
        }

        public async Task RefreshStore()
        {
            var mapHexs = await _censusMap.GetAllMapHexs();

            if (mapHexs != null)
            {
                await _mapRepository.UpsertRangeAsync(mapHexs.Select(ConvertToDbModel));
            }

            /*
            var mapRegions = await _censusMap.GetAllMapRegions();

            if (mapRegions != null)
            {
                await _mapRepository.UpsertRangeAsync(mapRegions.Select(ConvertToDbModel));
            }

            var facilityLinks = await _censusMap.GetAllFacilityLinks();

            if (facilityLinks != null)
            {
                await _mapRepository.UpsertRangeAsync(facilityLinks.Select(ConvertToDbModel));
            }
            */
        }

        private MapHex ConvertToDbModel(CensusMapHexModel censusModel)
        {
            return new MapHex
            {
                MapRegionId = censusModel.MapRegionId,
                HexType = censusModel.HexType,
                TypeName = censusModel.TypeName,
                ZoneId = censusModel.ZoneId,
                XPos = censusModel.X,
                YPos = censusModel.Y
            };
        }

        private MapRegion ConvertToDbModel(CensusMapRegionModel censusModel)
        {
            return new MapRegion
            {
                Id = censusModel.MapRegionId,
                ZoneId = censusModel.ZoneId,
                FacilityId = censusModel.FacilityId,
                FacilityName = censusModel.FacilityName,
                FacilityTypeId = censusModel.FacilityTypeId,
                FacilityType = censusModel.FacilityType,
                XPos = censusModel.LocationX,
                YPos = censusModel.LocationY,
                ZPos = censusModel.LocationZ
            };
        }

        private FacilityLink ConvertToDbModel(CensusFacilityLinkModel censusModel)
        {
            return new FacilityLink
            {
                ZoneId = censusModel.ZoneId,
                FacilityIdA = censusModel.FacilityIdA,
                FacilityIdB = censusModel.FacilityIdB,
                Description = censusModel.Description
            };
        }

        private string GetCacheKey(string id)
        {
            return $"{_cacheKeyPrefix}-{id}";
        }
    }
}
