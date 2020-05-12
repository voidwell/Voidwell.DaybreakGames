using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.UpdatableServices
{
    public class ItemService : IUpdatable
    {
        private readonly IItemService _service;

        public ItemService(IItemService service)
        {
            _service = service;
        }

        public string ServiceName => "ItemService";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(45);

        public Task UpdateAsync()
        {
            return _service.RefreshStore();
        }
    }
}
