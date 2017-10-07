using Newtonsoft.Json;
using System;

namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class Character
    {
        public string CharacterId { get; set; }
        public CharacterName Name { get; set; }
        public string FactionId { get; set; }
        public string TitleId { get; set; }
        public CharacterTimes Times { get; set; }
        public CharacterBattleRank BattleRank { get; set; }
        public CharacterCerts Certs { get; set; }
        public string WorldId { get; set; }
        public bool OnlineStatus { get; set; }

        public class CharacterName
        {
            public string First { get; set; }
            public string FirstLower { get; set; }
        }

        public class CharacterTimes
        {
            public DateTime CreationDate { get; set; }
            public DateTime LastSaveDate { get; set; }
            public DateTime LastLoginDate { get; set; }
            public string MinutesPlayed { get; set; }
        }

        public class CharacterBattleRank
        {
            public string PercentToNext { get; set; }
            public string Value { get; set; }
        }

        public class CharacterCerts
        {
            public string EarnedPoints { get; set; }
            public string GiftedPoints { get; set; }
            public string SpentPoints { get; set; }
            public string AvailablePoints { get; set; }
            public string PercentToNext { get; set; }
        }
    }
}
