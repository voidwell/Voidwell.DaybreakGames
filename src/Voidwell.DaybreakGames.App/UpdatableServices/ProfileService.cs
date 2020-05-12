using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class ProfileService : IUpdatable
    {
        private readonly IProfileService _service;

        public ProfileService(IProfileService service)
        {
            _service = service;
        }

        public string ServiceName => "ProfileService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
