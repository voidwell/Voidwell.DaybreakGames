using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MetagameEventStore : IMetagameEventStore
    {
        private readonly IMetagameEventRepository _metagameEventRepository;
        private readonly MetagameEventCollection _metagameEventCollection;
        private readonly IMapper _mapper;

        public string StoreName => "MetagameEventStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public MetagameEventStore(IMetagameEventRepository metagameEventRepository,
            MetagameEventCollection metagameEventCollection, IMapper mapper)
        {
            _metagameEventRepository = metagameEventRepository;
            _metagameEventCollection = metagameEventCollection;
            _mapper = mapper;
        }

        public Task<MetagameEventCategory> GetMetagameEventCategoryAsync(int metagameEventId)
        {
            return _metagameEventRepository.GetMetagameEventCategory(metagameEventId);
        }

        public Task<int?> GetMetagameCategoryZoneIdAsync(int metagameEventId)
        {
            return _metagameEventRepository.GetMetagameCategoryZoneId(metagameEventId);
        }

        public async Task RefreshStore()
        {
            var categories = await _metagameEventCollection.GetCollectionAsync();

            if (categories != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(categories.Select(_mapper.Map<MetagameEventCategory>));
            }
        }
    }
}
