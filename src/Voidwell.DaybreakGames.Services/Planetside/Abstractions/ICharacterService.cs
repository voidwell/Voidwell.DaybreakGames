using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface ICharacterService
{
    Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12);
    Task<IEnumerable<Character>> FindCharacters(IEnumerable<long> characterIds);
    Task<Character> GetCharacter(long characterId);
    Task<CharacterDetails?> GetCharacterDetails(long characterId);
    Task<IEnumerable<SimpleCharacterDetails>?> GetCharactersByName(IEnumerable<string> characterNames);
    Task UpdateAllCharacterInfo(long characterId, DateTime? LastLoginDate = null);
    Task<OutfitMember> GetCharactersOutfit(long characterId);
    Task<SimpleCharacterDetails?> GetCharacterByName(string characterName);
    Task<CharacterWeaponDetails?> GetCharacterWeaponByName(string characterName, string weaponName);
    Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(long characterId);
    Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(long characterId);
}
