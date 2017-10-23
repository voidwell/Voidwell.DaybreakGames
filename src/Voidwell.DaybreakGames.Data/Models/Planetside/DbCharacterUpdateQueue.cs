using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterUpdateQueue")]
    public class DbCharacterUpdateQueue
    {
        [Key]
        [Required]
        public string CharacterId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}