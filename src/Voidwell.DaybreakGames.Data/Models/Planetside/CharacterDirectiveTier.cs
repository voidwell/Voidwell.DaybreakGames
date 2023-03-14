using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterDirectiveTier
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int DirectiveTreeId { get; set; }
        [Required]
        public int DirectiveTierId { get; set; }
        public DateTime? CompletionTimeDate { get; set; }
        
        public IEnumerable<CharacterDirective> CharacterDirectives { get; set; }
    }
}
