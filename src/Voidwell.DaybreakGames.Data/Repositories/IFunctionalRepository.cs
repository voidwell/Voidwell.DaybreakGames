using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Data.Repositories;

public interface IFunctionalRepository
{
    Task<IEnumerable<CharacterLastSession>> GetPSBLastOnline();
}
