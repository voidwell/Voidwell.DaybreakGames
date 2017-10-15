using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ZoneService : IZoneService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusZone _censusZone;

        public ZoneService(PS2DbContext ps2DbContext, CensusZone censusZone)
        {
            _ps2DbContext = ps2DbContext;
            _censusZone = censusZone;
        }

        public async Task<IEnumerable<DbZone>> GetAllZones()
        {
            return await _ps2DbContext.Zones.ToListAsync();
        }

        public async Task RefreshStore()
        {
            var zones = await _censusZone.GetAllZones();

            if (zones != null)
            {
                _ps2DbContext.Zones.UpdateRange(zones.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbZone ConvertToDbModel(CensusZoneModel censusModel)
        {
            return new DbZone
            {
                Id = censusModel.ZoneId,
                Name = censusModel.Name.English,
                Description = censusModel.Description.English,
                Code = censusModel.Code,
                HexSize = censusModel.HexSize
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
