using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface ICharacterDirectiveService
{
    Task<CharacterDirectivesOutline?> GetCharacterDirectivesAsync(long characterId);
    Task UpdateCharacterDirectivesAsync(long characterId);
}
