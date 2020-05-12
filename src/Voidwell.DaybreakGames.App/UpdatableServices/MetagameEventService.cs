using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class MetagameEventService : IUpdatable
    {
        private readonly IMetagameEventService _service;

        public MetagameEventService(IMetagameEventService service)
        {
            _service = service;
        }

        public string ServiceName => "MetagameEventService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
