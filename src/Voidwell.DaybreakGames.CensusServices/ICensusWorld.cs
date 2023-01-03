using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public interface ICensusWorld
    {
        Task<IEnumerable<CensusWorldModel>> GetAllWorlds();
    }
}