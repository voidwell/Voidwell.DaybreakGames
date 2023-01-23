using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class LoadoutStore : ILoadoutStore
    {
        private readonly ILoadoutRepository _loadoutRepository;
        private readonly LoadoutCollection _loadoutCollection;
        private readonly IMapper _mapper;

        public LoadoutStore(ILoadoutRepository loadoutRepository, LoadoutCollection loadoutCollection, IMapper mapper)
        {
            _loadoutRepository = loadoutRepository;
            _loadoutCollection = loadoutCollection;
            _mapper = mapper;
        }

        public string StoreName => "LoadoutStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            return _loadoutRepository.GetAllLoadoutsAsync();
        }

        public async Task RefreshStore()
        {
            var loadouts = await _loadoutCollection.GetCollectionAsync();

            if (loadouts != null)
            {
                await _loadoutRepository.UpsertRangeAsync(loadouts.Select(_mapper.Map<Loadout>));
            }
        }
    }
}
