namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterDirectiveObjectiveModel
    {
        public string CharacterId { get; set; }
        public int DirectiveId { get; set; }
        public int ObjectiveId { get; set; }
        public int ObjectiveGroupId { get; set; }
        public int Status { get; set; }
        public int? StateData { get; set; }
    }
}
