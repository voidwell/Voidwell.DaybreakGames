using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterDirective
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int DirectiveId { get; set; }
        public int DirectiveTreeId { get; set; }
        public DateTime? CompletionTimeDate { get; set; }

        public IEnumerable<CharacterDirectiveObjective> CharacterDirectiveObjectives { get; set; }
        public Directive Directive { get; set; }
    }
}
