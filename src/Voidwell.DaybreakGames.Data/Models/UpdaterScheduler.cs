using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models
{
    public class UpdaterScheduler
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public DateTime LastUpdateDate { get; set; }
    }
}
