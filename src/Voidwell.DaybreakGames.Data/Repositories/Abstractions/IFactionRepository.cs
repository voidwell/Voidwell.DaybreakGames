using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface IFactionRepository : IRepository<Faction>
{
    Task<Faction?> GetFactionByIdAsync(int factionId);
}
