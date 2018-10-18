using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldService : IWorldService
    {
        private readonly IWorldRepository _worldRepository;
        private readonly CensusWorld _censusWorld;
        private readonly ICache _cache;

        public string ServiceName => "WorldService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private const string _cacheKeyPrefix = "ps2.worldService";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public WorldService(IWorldRepository worldRepository, CensusWorld censusWorld, ICache cache)
        {
            _worldRepository = worldRepository;
            _censusWorld = censusWorld;
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

        public async Task<Dictionary<int, IEnumerable<DailyPopulation>>> GetWorldPopulationHistory(IEnumerable<int> worldIds, DateTime start, DateTime end)
        {
            var popTasks = worldIds.Select(id => GetWorldPopulationHistory(id, start, end)).ToArray();
            await Task.WhenAll(popTasks);

            var popDict = new Dictionary<int, IEnumerable<DailyPopulation>>();
            for (var i = 0; i < worldIds.Count(); i++)
            {
                popDict[worldIds.ToArray()[i]] = popTasks.ToArray()[i].Result;
            }

            return popDict;
        }

        private readonly KeyedSemaphoreSlim _worldPopulationLock = new KeyedSemaphoreSlim();

        private async Task<IEnumerable<DailyPopulation>> GetWorldPopulationHistory(int worldId, DateTime start, DateTime end)
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

        public async Task RefreshStore()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds != null)
            {
                await _worldRepository.UpsertRangeAsync(worlds.Select(ConvertToDbModel));
                await _cache.RemoveAsync(_cacheKeyPrefix);
            }
        }

        private World ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new World
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }
    }
}
