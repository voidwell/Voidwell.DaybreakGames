using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class ObjectiveSetToObjective
    {
        [Required]
        public int ObjectiveSetId { get; set; }
        [Required]
        public int ObjectiveGroupId { get; set; }

        public IEnumerable<Objective> Objectives { get; set; }
    }
}