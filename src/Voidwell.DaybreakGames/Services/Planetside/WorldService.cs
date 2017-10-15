using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldService : IWorldService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusWorld _censusWorld;

        public WorldService(PS2DbContext ps2DbContext, CensusWorld censusWorld)
        {
            _ps2DbContext = ps2DbContext;
            _censusWorld = censusWorld;
        }

        public async Task<IEnumerable<DbWorld>> GetAllWorlds()
        {
            return await _ps2DbContext.Worlds.ToListAsync();
        }

        public async Task RefreshStore()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds != null)
            {
                _ps2DbContext.Worlds.UpdateRange(worlds.Select(i => ConvertToDbModel(i)));
            }

            await _ps2DbContext.SaveChangesAsync();
        }

        private DbWorld ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new DbWorld
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
