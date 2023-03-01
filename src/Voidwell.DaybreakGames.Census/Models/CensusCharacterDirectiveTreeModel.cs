using System;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterDirectiveTreeModel
    {
        public string CharacterId { get; set; }
        public int DirectiveTreeId { get; set; }
        public int CurrentDirectiveTierId { get; set; }
        public int CurrentLevel { get; set; }
        public DateTime CompletionTimeDate { get; set; }
    }
}
