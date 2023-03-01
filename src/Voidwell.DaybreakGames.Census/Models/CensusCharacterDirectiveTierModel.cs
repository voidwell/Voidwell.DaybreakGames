using System;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterDirectiveTierModel
    {
        public string CharacterId { get; set; }
        public int DirectiveTreeId { get; set; }
        public int DirectiveTierId { get; set; }
        public DateTime CompletionTimeDate { get; set; }
    }
}
