using System;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterDirectiveModel
    {
        public string CharacterId { get; set; }
        public int DirectiveTreeId { get; set; }
        public int DirectiveId { get; set; }
        public DateTime CompletionTimeDate { get; set; }
    }
}
