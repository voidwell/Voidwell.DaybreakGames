using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterDirectiveTree
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int DirectiveTreeId { get; set; }
        public int CurrentDirectiveTierId { get; set; }
        public int CurrentLevel { get; set; }
        public DateTime? CompletionTimeDate { get; set; }

        public IEnumerable<CharacterDirectiveTier> CharacterDirectiveTiers { get; set; }
        public IEnumerable<CharacterDirective> CharacterDirectives { get; set; }
    }
}
