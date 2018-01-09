using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("ItemCategory")]
    public class DbItemCategory
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
