using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Faction")]
    public class DbFaction
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
        public string ImageId { get; set; }
        public string CodeTag { get; set; }
        public bool UserSelectable { get; set; }
    }
}
