using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class MetagameEventState
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
