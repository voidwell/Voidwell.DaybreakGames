using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class VehicleStore : IVehicleStore
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly CensusVehicle _censusVehicle;

        public string StoreName => "VehicleStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public VehicleStore(IVehicleRepository vehicleRepository, CensusVehicle censusVehicle)
        {
            _vehicleRepository = vehicleRepository;
            _censusVehicle = censusVehicle;
        }

        public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return _vehicleRepository.GetAllVehiclesAsync();
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

        private static Vehicle ConvertToDbModel(CensusVehicleModel censusModel)
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

        private static VehicleFaction ConvertToDbModel(CensusVehicleFactionModel censusModel)
        {
            return new VehicleFaction
            {
                VehicleId = censusModel.VehicleId,
                FactionId = censusModel.FactionId
            };
        }
    }
}
