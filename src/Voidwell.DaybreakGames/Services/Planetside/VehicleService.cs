using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly CensusVehicle _censusVehicle;

        public string ServiceName => "VehicleService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public VehicleService(IVehicleRepository vehicleRepository, CensusVehicle censusVehicle)
        {
            _vehicleRepository = vehicleRepository;
            _censusVehicle = censusVehicle;
        }

        public async Task<IEnumerable<VehicleInfo>> GetAllVehicles()
        {
            var vehicles = await _vehicleRepository.GetAllVehiclesAsync();

            return vehicles.Select(a => new VehicleInfo
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                ImageId =   a.ImageId,
                Factions = a.Faction?.Select(b => b.FactionId)
            });
        }

        public async Task RefreshStore()
        {
            var vehicles = await _censusVehicle.GetAllVehicles();
            var vehicleFactions = await _censusVehicle.GetAllVehicleFactions();

            if (vehicles != null)
            {
                await _vehicleRepository.UpsertRangeAsync(vehicles.Select(ConvertToDbModel));
            }

            if (vehicleFactions != null)
            {
                await _vehicleRepository.UpsertRangeAsync(vehicleFactions.Select(ConvertToDbModel));
            }
        }

        private Vehicle ConvertToDbModel(CensusVehicleModel censusModel)
        {
            return new Vehicle
            {
                Id = censusModel.VehicleId,
                Name = censusModel.Name?.English,
                ImageId = censusModel.ImageId,
                Cost = censusModel.Cost,
                CostResourceId = censusModel.CostResourceId,
                Description = censusModel.Description?.English
            };
        }

        private VehicleFaction ConvertToDbModel(CensusVehicleFactionModel censusModel)
        {
            return new VehicleFaction
            {
                VehicleId = censusModel.VehicleId,
                FactionId = censusModel.FactionId
            };
        }
    }
}
