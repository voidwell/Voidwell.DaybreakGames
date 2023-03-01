namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusDirectiveTreeModel
    {
        public int DirectiveTreeId { get; set; }
        public int? DirectiveTreeCategoryId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }
    }
}
