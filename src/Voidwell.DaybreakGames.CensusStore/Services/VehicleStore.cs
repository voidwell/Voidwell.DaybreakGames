using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class VehicleStore : IVehicleStore
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleStore(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return _vehicleRepository.GetAllVehiclesAsync();
        }
    }
}
