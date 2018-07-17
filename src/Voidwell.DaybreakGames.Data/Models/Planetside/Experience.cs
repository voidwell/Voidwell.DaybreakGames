using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Experience
    {
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public float Xp { get; set; }
    }
}
