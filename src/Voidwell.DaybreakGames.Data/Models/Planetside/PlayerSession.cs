using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class PlayerSession
    {
        public int Id { get; set; }

        [Required]
        public string CharacterId { get; set; }
        
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
        public int Duration { get; set; }
    }
}
