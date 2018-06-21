using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterStatHistory
    {
        [Required]
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public int AllTime { get; set; }
        public int OneLifeMax { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Week { get; set; }

        public Character Character { get; set; }
    }
}
