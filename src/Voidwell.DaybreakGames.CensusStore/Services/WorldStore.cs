using AsyncKeyedLock;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class WorldStore : IWorldStore
    {
        private readonly IWorldRepository _worldRepository;
        private readonly WorldCollection _worldCollection;
        private readonly ICache _cache;
        private readonly IMapper _mapper;

        private const string _cacheKeyPrefix = "ps2.worldstore";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public string StoreName => "WorldStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(7);

        public WorldStore(IWorldRepository worldRepository, WorldCollection worldCollection, AsyncKeyedLocker<string> asyncKeyedLocker, ICache cache, IMapper mapper)
        {
            _worldRepository = worldRepository;
            _worldCollection = worldCollection;
            _asyncKeyedLocker = asyncKeyedLocker;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<IEnumerable<World>> GetAllWorlds()
        {
            var worlds = await _cache.GetAsync<IEnumerable<World>>(_cacheKeyPrefix);
            if (worlds != null)
            {
                return worlds;
            }

            worlds = await _worldRepository.GetAllWorldsAsync();
            if (worlds != null)
            {
                await _cache.SetAsync(_cacheKeyPrefix, worlds, _cacheExpiration);
            }

            return worlds;
        }

        public async Task<IEnumerable<DailyPopulation>> GetWorldPopulationHistory(int worldId, DateTime start, DateTime end)
        {
            var cacheKey = $"{_cacheKeyPrefix}_{worldId}_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}";

            using (await _asyncKeyedLocker.LockAsync(cacheKey).ConfigureAwait(false))
            {

                var populations = await _cache.GetAsync<IEnumerable<DailyPopulation>>(cacheKey);
                if (populations != null)
                {
                    return populations;
                }

                populations = await _worldRepository.GetDailyPopulationsByWorldIdAsync(worldId);
                if (populations != null)
                {
                    await _cache.SetAsync(cacheKey, populations, _cacheExpiration);
                }

                return populations;
            }
        }

        public async Task RefreshStore()
        {
            var worlds = await _worldCollection.GetCollectionAsync();

            if (worlds != null)
            {
                await _worldRepository.UpsertRangeAsync(worlds.Select(_mapper.Map<World>));
                await _cache.RemoveAsync(_cacheKeyPrefix);
            }
        }
    }
}
