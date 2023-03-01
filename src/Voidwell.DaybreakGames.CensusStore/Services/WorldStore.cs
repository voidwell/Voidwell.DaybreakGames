using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Microservice.Utility;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class WorldStore : IWorldStore
    {
        private readonly IWorldRepository _worldRepository;
        private readonly ICache _cache;

        private const string _cacheKeyPrefix = "ps2.worldstore";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        private readonly KeyedSemaphoreSlim _worldPopulationLock = new KeyedSemaphoreSlim();

        public WorldStore(IWorldRepository worldRepository, ICache cache)
        {
            _worldRepository = worldRepository;
            _cache = cache;
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

            using (await _worldPopulationLock.WaitAsync(cacheKey))
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
    }
}
