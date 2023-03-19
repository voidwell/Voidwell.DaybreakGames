using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class DirectivesOutline
    {
        public List<DirectivesOutlineTreeCategory> Categories { get; set; }
    }

    public class DirectivesOutlineTreeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DirectivesOutlineTree> Trees { get; set; }
    }

    public class DirectivesOutlineTree
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }

        public List<DirectivesOutlineTier> Tiers { get; set; }
    }

    public class DirectivesOutlineTier
    {
        public int TierId { get; set; }
        public int TreeId { get; set; }
        public int? DirectivePoints { get; set; }
        public int CompletionCount { get; set; }
        public string Name { get; set; }
        public int? ImageId { get; set; }

        public List<DirectivesOutlineReward> Rewards { get; set; }
        public List<DirectivesOutlineDirective> Directives { get; set; }
    }

    public class DirectivesOutlineReward
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ImageId { get; set; }
    }

    public class DirectivesOutlineDirective
    {
        public int Id { get; set; }
        public int TreeId { get; set; }
        public int TierId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }

        public IEnumerable<DirectivesOutlineObjective> Objectives { get; set; }
    }

    public class DirectivesOutlineObjective
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int? GoalValue { get; set; }
    }
}
