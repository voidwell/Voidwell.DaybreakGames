using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class TitleService : IUpdatable
    {
        private readonly ITitleService _service;

        public TitleService(ITitleService service)
        {
            _service = service;
        }

        public string ServiceName => "TitleService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
