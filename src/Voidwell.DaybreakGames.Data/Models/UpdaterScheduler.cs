using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("UpdaterScheduler")]
    public class UpdaterScheduler
    {
        [Key]
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public DateTime LastUpdateDate { get; set; }
    }
}
