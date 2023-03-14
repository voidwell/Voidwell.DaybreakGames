using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class CharacterDirectivesOutline
    {
        public List<CharacterDirectivesOutlineCategory> Categories { get; set; } = new();
    }

    public class CharacterDirectivesOutlineCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<CharacterDirectivesOutlineTree> Trees { get; set; } = new();
    }

    public class CharacterDirectivesOutlineTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int CurrentDirectiveTierId { get; set; }
        public int CurrentLevel { get; set; }
        public DateTime? CompletionDate { get; set; }

        public List<CharacterDirectivesOutlineTier> Tiers { get; set; } = new();
    }

    public class CharacterDirectivesOutlineTier
    {
        public int TierId { get; set; }
        public int TreeId { get; set; }
        public int? DirectivePoints { get; set; }
        public int CompletionCount { get; set; }
        public string Name { get; set; }
        public int? ImageId { get; set; }
        public DateTime? CompletionDate { get; set; }

        public List<DirectivesOutlineReward> Rewards { get; set; } = new();
        public List<CharacterDirectivesOutlineDirective> Directives { get; set; } = new();
    }

    public class CharacterDirectivesOutlineDirective
    {
        public int Id { get; set; }
        public int TreeId { get; set; }
        public int TierId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public DateTime? CompletionDate { get; set; }

        public int? Progress { get; set; }

        public DirectivesOutlineObjective Objective { get; set; } = new();
    }
}
