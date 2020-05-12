using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class VehicleService : IUpdatable
    {
        private readonly IVehicleService _service;

        public VehicleService(IVehicleService service)
        {
            _service = service;
        }

        public string ServiceName => "VehicleService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
