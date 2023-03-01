using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MetagameEventStore : IMetagameEventStore
    {
        private readonly IMetagameEventRepository _metagameEventRepository;

        public MetagameEventStore(IMetagameEventRepository metagameEventRepository)
        {
            _metagameEventRepository = metagameEventRepository;
        }

        public Task<MetagameEventCategory> GetMetagameEventCategoryAsync(int metagameEventId)
        {
            return _metagameEventRepository.GetMetagameEventCategory(metagameEventId);
        }

        public Task<int?> GetMetagameCategoryZoneIdAsync(int metagameEventId)
        {
            return _metagameEventRepository.GetMetagameCategoryZoneId(metagameEventId);
        }
    }
}
