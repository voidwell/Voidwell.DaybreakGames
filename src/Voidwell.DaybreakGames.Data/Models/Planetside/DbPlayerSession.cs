using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("PlayerSession")]
    public class DbPlayerSession
    {
        [Required]
        public string CharacterId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
        public int Duration { get; set; }
    }
}
