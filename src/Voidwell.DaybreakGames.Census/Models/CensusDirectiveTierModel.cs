namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusDirectiveTierModel
    {
        public int DirectiveTierId { get; set; }
        public int DirectiveTreeId { get; set; }
        public int? RewardSetId { get; set; }
        public int? DirectivePoints { get; set; }
        public int CompletionCount { get; set; }
        public MultiLanguageString Name { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }
    }
}
