using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class FactionStore : IFactionStore
    {
        public readonly IFactionRepository _factionRepository;
        private readonly FactionCollection _factionCollection;
        private readonly IMapper _mapper;

        public string StoreName => "FactionStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public FactionStore(IFactionRepository factionRepository, FactionCollection factionCollection, IMapper mapper)
        {
            _factionRepository = factionRepository;
            _factionCollection = factionCollection;
            _mapper = mapper; ;
        }

        public async Task RefreshStore()
        {
            var factions = await _factionCollection.GetCollectionAsync();

            if (factions != null)
            {
                await _factionRepository.UpsertRangeAsync(factions.Select(_mapper.Map<Faction>));
            }
        }
    }
}
