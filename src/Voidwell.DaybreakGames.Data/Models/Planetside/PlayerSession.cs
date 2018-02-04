using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class PlayerSession
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CharacterId { get; set; }
        
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
        public int Duration { get; set; }
    }
}
