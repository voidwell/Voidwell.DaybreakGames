using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.Microservice.Cache;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class FactionStore : IFactionStore
    {
        private readonly IFactionRepository _repository;
        private readonly ICache _cache;

        private const string _factionCacheKey = "ps2.faction";
        private readonly TimeSpan _factionCacheExpiration = TimeSpan.FromHours(12);

        public FactionStore(IFactionRepository factionRepository, ICache cache)
        {
            _repository = factionRepository;
            _cache = cache;
        }

        public async Task<Faction> GetFactionByIdAsync(int factionId)
        {
            var cacheKey = $"{_factionCacheKey}_{factionId}";

            var faction = await _cache.GetAsync<Faction>(cacheKey);
            if (faction != null)
            {
                return faction;
            }

            faction = await _repository.GetFactionByIdAsync(factionId);
            if (faction != null)
            {
                await _cache.SetAsync(cacheKey, faction, _factionCacheExpiration);
            }

            return faction;
        }
    }
}
