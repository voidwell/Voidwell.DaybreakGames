using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterTime")]
    public class DbCharacterTime
    {
        [Key]
        [Required]
        public string CharacterId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastSaveDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int MinutesPlayed { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
    }
}
