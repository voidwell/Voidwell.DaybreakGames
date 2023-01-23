using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class MetagameEventStateStore : IMetagameEventStateStore
    {
        private readonly IMetagameEventRepository _metagameEventRepository;
        private readonly MetagameEventStateCollection _metagameEventStateCollection;
        private readonly IMapper _mapper;

        public string StoreName => "MetagameEventStateStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public MetagameEventStateStore(IMetagameEventRepository metagameEventRepository,
            MetagameEventStateCollection metagameEventStateCollection, IMapper mapper)
        {
            _metagameEventRepository = metagameEventRepository;
            _metagameEventStateCollection = metagameEventStateCollection;
            _mapper = mapper;
        }

        public async Task RefreshStore()
        {
            var states = await _metagameEventStateCollection.GetCollectionAsync();

            if (states != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(states.Select(_mapper.Map<MetagameEventState>));
            }
        }
    }
}
