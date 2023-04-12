using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class LoadoutStore : ILoadoutStore
    {
        private readonly ILoadoutRepository _loadoutRepository;

        public LoadoutStore(ILoadoutRepository loadoutRepository)
        {
            _loadoutRepository = loadoutRepository;
        }

        public string StoreName => "LoadoutStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            return _loadoutRepository.GetAllLoadoutsAsync();
        }
    }
}
