using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventGainExperience")]
    public class DbEventGainExperience
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ExperienceId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public int Amount { get; set; }
        public string LoadoutId { get; set; }
        public string OtherId { get; set; }
    }
}
