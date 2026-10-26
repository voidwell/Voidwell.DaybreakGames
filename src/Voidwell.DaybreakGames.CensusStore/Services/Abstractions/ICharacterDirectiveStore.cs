using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface ICharacterDirectiveStore
{
    Task<IEnumerable<CharacterDirectiveTree>?> GetCharacterDirectivesAsync(long characterId);
    Task UpdateCharacterDirectiveDataAsync(long characterId);
}
