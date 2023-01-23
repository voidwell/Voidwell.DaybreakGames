using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class VehicleStore : IVehicleStore
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleCollection _vehicleCollection;
        private readonly IMapper _mapper;

        public string StoreName => "VehicleStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public VehicleStore(IVehicleRepository vehicleRepository, VehicleCollection vehicleCollection, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleCollection = vehicleCollection;
            _mapper = mapper;
        }

        public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return _vehicleRepository.GetAllVehiclesAsync();
        }

        public async Task RefreshStore()
        {
            var vehicles = await _vehicleCollection.GetCollectionAsync();

            if (vehicles != null)
            {
                await _vehicleRepository.UpsertRangeAsync(vehicles.Select(_mapper.Map<Vehicle>));
            }
        }
    }
}
