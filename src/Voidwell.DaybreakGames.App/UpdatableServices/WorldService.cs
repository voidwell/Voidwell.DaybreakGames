using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class WorldService : IUpdatable
    {
        private readonly IWorldService _service;

        public WorldService(IWorldService service)
        {
            _service = service;
        }

        public string ServiceName => "WorldService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
