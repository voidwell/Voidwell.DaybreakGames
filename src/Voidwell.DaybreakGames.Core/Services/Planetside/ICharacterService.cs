using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System;
using Voidwell.DaybreakGames.Data;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface ICharacterService
    {
        Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12);
        Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds);
        Task<Character> GetCharacter(string characterId);
        Task<CharacterDetails> GetCharacterDetails(string characterId);
        Task<IEnumerable<SimpleCharacterDetails>> GetCharactersByName(IEnumerable<string> characterNames);
        Task UpdateAllCharacterInfo(string characterId, DateTime? LastLoginDate = null);
        Task<OutfitMember> GetCharactersOutfit(string characterId);
        Task<SimpleCharacterDetails> GetCharacterByName(string characterName);
        Task<CharacterWeaponDetails> GetCharacterWeaponByName(string characterName, string weaponName);
        Task<IEnumerable<WeaponLeaderboardRow>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50);
    }
}