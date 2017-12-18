using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public string ServiceName => "WorldService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public WorldService(IWorldRepository worldRepository, CensusWorld censusWorld)
        {
            _worldRepository = worldRepository;
            _censusWorld = censusWorld;
        }

        public Task<IEnumerable<DbWorld>> GetAllWorlds()
        {
            return _worldRepository.GetAllWorldsAsync();
        }

        public async Task RefreshStore()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds != null)
            {
                await _worldRepository.UpsertRangeAsync(worlds.Select(ConvertToDbModel));
            }
        }

        private DbWorld ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new DbWorld
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }
    }
}
