using System;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class CharacterSearchResult
    {
        public string Name { get; set; }
        public int BattleRank { get; set; }
        public string Id { get; set; }
        public int FactionId { get; set; }
        public DateTime LastLogin { get; set; }
        public int WorldId { get; set; }
        public bool OnlineStatus { get; set; }

        public static CharacterSearchResult LoadFromCensusCharacter(CensusCharacterModel censusCharacter)
        {
            return new CharacterSearchResult
            {
                Name = censusCharacter.Name.First,
                BattleRank = censusCharacter.BattleRank.Value,
                Id = censusCharacter.CharacterId,
                FactionId = censusCharacter.FactionId,
                LastLogin = censusCharacter.Times.LastLoginDate,
                WorldId = censusCharacter.WorldId,
                OnlineStatus = censusCharacter.OnlineStatus
            };
        }
    }
}
