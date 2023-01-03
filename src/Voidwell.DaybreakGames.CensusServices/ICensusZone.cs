using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public interface ICensusZone
    {
        Task<IEnumerable<CensusZoneModel>> GetAllZones();
    }
}