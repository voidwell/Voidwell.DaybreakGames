using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class ZoneService : IUpdatable
    {
        private readonly IZoneService _service;

        public ZoneService(IZoneService service)
        {
            _service = service;
        }

        public string ServiceName => "ZoneService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
