using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class MapService : IMapService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;

        public MapService(PS2DbContext ps2DbContext)
        {
            _ps2DbContext = ps2DbContext;
        }

        public async Task<IEnumerable<MapOwnershipModel>> GetMapOwnership(string worldId, string zoneId)
        {
            var ownership = await CensusMap.GetMapOwnership(worldId, zoneId);

            if (ownership == null)
            {
                return null;
            }

            return ownership.Regions.Row.Select(o => new MapOwnershipModel(o.RowData.RegionId, o.RowData.FactionId));
        }

        public async Task<IEnumerable<DbMapRegion>> GetMapRegions(string zoneId)
        {
            return await _ps2DbContext.MapRegions.Where(r=> r.ZoneId == zoneId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DbFacilityLink>> GetFacilityLinks(string zoneId)
        {
            return await _ps2DbContext.FacilityLinks.Where(l => l.ZoneId == zoneId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DbMapRegion>> FindRegions(params string[] facilityIds)
        {
            return await _ps2DbContext.MapRegions.Where(r => facilityIds.Contains(r.FacilityId))
                .ToListAsync();
        }

        public async Task RefreshStore()
        {
            var mapHexs = await CensusMap.GetAllMapHexs();
            var mapRegions = await CensusMap.GetAllMapRegions();
            var facilityLinks = await CensusMap.GetAllFacilityLinks();

            if (mapHexs != null)
            {
                _ps2DbContext.MapHexs.UpdateRange(mapHexs.Select(m => ConvertToDbModel(m)));
            }

            if (mapRegions != null)
            {
                _ps2DbContext.MapRegions.UpdateRange(mapRegions.Select(m => ConvertToDbModel(m)));
            }

            if (facilityLinks != null)
            {
                _ps2DbContext.FacilityLinks.UpdateRange(facilityLinks.Select(m => ConvertToDbModel(m)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbMapHex ConvertToDbModel(CensusMapHexModel censusModel)
        {
            return new DbMapHex
            {
                MapRegionId = censusModel.MapRegionId,
                HexType = censusModel.HexType,
                TypeName = censusModel.TypeName,
                ZoneId = censusModel.ZoneId,
                XPos = censusModel.X,
                YPos = censusModel.Y
            };
        }

        private DbMapRegion ConvertToDbModel(CensusMapRegionModel censusModel)
        {
            return new DbMapRegion
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

        private DbFacilityLink ConvertToDbModel(CensusFacilityLinkModel censusModel)
        {
            return new DbFacilityLink
            {
                ZoneId = censusModel.ZoneId,
                FacilityIdA = censusModel.FacilityIdA,
                FacilityIdB = censusModel.FacilityIdB,
                Description = censusModel.Description
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
