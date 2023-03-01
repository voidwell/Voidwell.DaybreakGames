using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterDirectiveObjective
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int DirectiveId { get; set; }
        [Required]
        public int ObjectiveId { get; set; }
        public int ObjectiveGroupId { get; set; }
        public int Status { get; set; }
        public int? StateData { get; set; }
        
        public Objective Objective { get; set; }
    }
}
