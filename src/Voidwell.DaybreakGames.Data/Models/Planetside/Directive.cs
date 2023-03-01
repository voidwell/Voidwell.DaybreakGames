using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Directive
    {
        [Required]
        public int Id { get; set; }
        public int DirectiveTreeId { get; set; }
        public int DirectiveTierId { get; set; }
        public int ObjectiveSetId { get; set; }
        public int? QualifyRequirementId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }

        public ObjectiveSetToObjective ObjectiveSet { get; set; }
    }
}
