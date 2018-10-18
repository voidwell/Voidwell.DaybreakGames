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
