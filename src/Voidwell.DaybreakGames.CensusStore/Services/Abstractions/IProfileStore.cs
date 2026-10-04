using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface IProfileStore
{
    Task<IEnumerable<Profile>> GetAllProfilesAsync();
}
