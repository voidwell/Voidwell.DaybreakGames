using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IFactionStore
    {
        Task<Faction> GetFactionByIdAsync(int factionId);
    }
}
