using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Title")]
    public class DbTitle
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
