using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class VehicleFactionStore : IVehicleFactionStore
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VehicleFactionCollection _vehicleFactionCollection;
        private readonly IMapper _mapper;

        public string StoreName => "VehicleFactionStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public VehicleFactionStore(IVehicleRepository vehicleRepository, VehicleFactionCollection vehicleFactionCollection, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _vehicleFactionCollection = vehicleFactionCollection;
            _mapper = mapper;
        }

        public async Task RefreshStore()
        {
            var vehicleFactions = await _vehicleFactionCollection.GetCollectionAsync();

            if (vehicleFactions != null)
            {
                await _vehicleRepository.UpsertRangeAsync(vehicleFactions.Select(_mapper.Map<VehicleFaction>));
            }
        }
    }
}
