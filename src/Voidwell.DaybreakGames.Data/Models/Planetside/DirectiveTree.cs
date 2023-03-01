using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class DirectiveTree
    {
        [Required]
        public int Id { get; set; }
        public int? DirectiveTreeCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ImageId { get; set; }
        public int? ImageSetId { get; set; }

        public IEnumerable<DirectiveTier> Tiers { get; set; }
    }
}
