namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusDirectiveModel
    {
        public int DirectiveId { get; set; }
        public int DirectiveTreeId { get; set; }
        public int DirectiveTierId { get; set; }
        public int ObjectiveSetId { get; set; }
        public int? QualifyRequirementId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }
    }
}
