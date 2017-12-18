using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly CensusZone _censusZone;

        public string ServiceName => "ZoneService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ZoneService(IZoneRepository zoneRepository, CensusZone censusZone)
        {
            _zoneRepository = zoneRepository;
            _censusZone = censusZone;
        }

        public Task<IEnumerable<DbZone>> GetAllZones()
        {
            return _zoneRepository.GetAllZonesAsync();
        }

        public async Task RefreshStore()
        {
            var zones = await _censusZone.GetAllZones();

            if (zones != null)
            {
                await _zoneRepository.UpsertRangeAsync(zones.Select(ConvertToDbModel));
            }
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
    }
}
