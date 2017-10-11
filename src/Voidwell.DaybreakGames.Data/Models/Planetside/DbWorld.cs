using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("World")]
    public class DbWorld
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
