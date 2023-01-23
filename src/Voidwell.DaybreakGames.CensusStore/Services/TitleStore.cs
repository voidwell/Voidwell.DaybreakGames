using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class TitleStore : ITitleStore
    {
        private readonly ITitleRepository _titleRepository;
        private readonly TitleCollection _titleCollection;
        private readonly IMapper _mapper;

        public string StoreName => "TitleStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public TitleStore(ITitleRepository titleRepository, TitleCollection titleCollection, IMapper mapper)
        {
            _titleRepository = titleRepository;
            _titleCollection = titleCollection;
            _mapper = mapper;
        }

        public async Task RefreshStore()
        {
            var titles = await _titleCollection.GetCollectionAsync();

            if (titles != null)
            {
                await _titleRepository.UpdateRangeAsync(titles.Select(_mapper.Map<Title>));
            }
        }
    }
}
