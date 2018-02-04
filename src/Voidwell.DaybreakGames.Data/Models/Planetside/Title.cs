using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Title
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
