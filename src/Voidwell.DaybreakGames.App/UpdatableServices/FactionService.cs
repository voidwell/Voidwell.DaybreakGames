using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class FactionService : IUpdatable
    {
        private readonly IFactionService _service;

        public FactionService(IFactionService service)
        {
            _service = service;
        }

        public string ServiceName => "FactionService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
