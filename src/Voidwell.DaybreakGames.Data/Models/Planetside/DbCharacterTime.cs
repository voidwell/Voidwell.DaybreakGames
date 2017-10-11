using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterTime")]
    public class DbCharacterTime
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastSaveDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int MinutesPlayed { get; set; }

        public string CharacterId { get; set; }
        public DbCharacter Character { get; set; }
    }
}
