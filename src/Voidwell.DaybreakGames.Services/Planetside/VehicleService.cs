using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleStore _vehicleStore;

        public VehicleService(IVehicleStore vehicleStore)
        {
            _vehicleStore = vehicleStore;
        }

        public async Task<IEnumerable<VehicleInfo>> GetAllVehicles()
        {
            var vehicles = await _vehicleStore.GetAllVehiclesAsync();

            return vehicles.Select(a => new VehicleInfo
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                ImageId =   a.ImageId,
                Factions = a.Faction?.Select(b => b.FactionId)
            });
        }
    }
}
