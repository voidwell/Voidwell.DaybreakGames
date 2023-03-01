using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Objective
    {
        [Required]
        public int Id { get; set; }
        public int ObjectiveTypeId { get; set; }
        public int ObjectiveGroupId { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string Param7 { get; set; }
        public string Param8 { get; set; }
        public string Param9 { get; set; }

        public Achievement Achievement { get; set; }
        public Item Item { get; set; }
    }
}
