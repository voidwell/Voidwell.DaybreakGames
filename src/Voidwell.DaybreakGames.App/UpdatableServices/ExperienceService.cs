using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class ExperienceService : IUpdatable
    {
        private readonly IExperienceService _service;

        public ExperienceService(IExperienceService service)
        {
            _service = service;
        }

        public string ServiceName => "ExperienceService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(45);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
