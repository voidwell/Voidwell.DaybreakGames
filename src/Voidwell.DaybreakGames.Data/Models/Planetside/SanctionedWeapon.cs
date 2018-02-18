using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class SanctionedWeapon
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}
