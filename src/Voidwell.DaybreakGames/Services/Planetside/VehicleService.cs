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
    public class VehicleService : IVehicleService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusVehicle _censusVehicle;

        public string ServiceName => "VehicleService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public VehicleService(PS2DbContext ps2DbContext, CensusVehicle censusVehicle)
        {
            _ps2DbContext = ps2DbContext;
            _censusVehicle = censusVehicle;
        }

        public async Task<IEnumerable<DbVehicle>> GetAllVehicles()
        {
            return await _ps2DbContext.Vehicles.Where(p => p != null)
                .Include(i => i.Faction)
                .ToListAsync();
        }

        public async Task RefreshStore()
        {
            var vehicles = await _censusVehicle.GetAllVehicles();
            var vehicleFactions = await _censusVehicle.GetAllVehicleFactions();

            if (vehicles != null)
            {
                _ps2DbContext.Vehicles.UpdateRange(vehicles.Select(i => ConvertToDbModel(i)));
            }

            if (vehicleFactions != null)
            {
                _ps2DbContext.VehicleFactions.UpdateRange(vehicleFactions.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbVehicle ConvertToDbModel(CensusVehicleModel censusModel)
        {
            return new DbVehicle
            {
                Id = censusModel.VehicleId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                Cost = censusModel.Cost,
                CostResourceId = censusModel.CostResourceId,
                Description = censusModel.Description.English
            };
        }

        private DbVehicleFaction ConvertToDbModel(CensusVehicleFactionModel censusModel)
        {
            return new DbVehicleFaction
            {
                VehicleId = censusModel.VehicleId,
                FactionId = censusModel.FactionId
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
