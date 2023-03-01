using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class CharacterDirectivesOutline
    {
        public List<CharacterDirectivesOutlineTree> Trees { get; set; }
    }

    public class CharacterDirectivesOutlineTree
    {
        public int Id { get; set; }
        public int CurrentDirectiveTierId { get; set; }
        public int CurrentLevel { get; set; }
        public DateTime? CompletionDate { get; set; }

        public List<CharacterDirectivesOutlineTier> Tiers { get; set; }
        public List<CharacterDirectivesOutlineDirective> Directives { get; set; }
    }

    public class CharacterDirectivesOutlineTier
    {
        public int TierId { get; set; }
        public int TreeId { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    public class CharacterDirectivesOutlineDirective
    {
        public int Id { get; set; }
        public DateTime? CompletionDate { get; set; }

        public List<CharacterDirectivesOutlineObjective> Objectives { get; set; }
    }

    public class CharacterDirectivesOutlineObjective
    {
        public int Id { get; set; }
        public int? Progress { get; set; }
    }
}
