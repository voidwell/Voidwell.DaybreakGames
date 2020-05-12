using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class MapService : IUpdatable
    {
        private readonly IMapService _service;

        public MapService(IMapService service)
        {
            _service = service;
        }

        public string ServiceName => "MapService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
