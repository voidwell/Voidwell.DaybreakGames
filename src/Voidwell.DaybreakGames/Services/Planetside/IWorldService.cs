using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWorldService : IUpdateable
    {
        Task<IEnumerable<World>> GetAllWorlds();
        Task<World> GetWorld(int worldId);
        Task<Dictionary<int, IEnumerable<DailyPopulation>>> GetWorldPopulationHistory(IEnumerable<int> worldIds, DateTime start, DateTime end);
        Task<WorldActivity> GetWorldActivity(int worldId, int periodHours);
    }
}
