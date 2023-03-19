using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class ImageSet
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ImageId { get; set; }
        public string Description { get; set; }
        [Required]
        public int TypeId { get; set; }
        public string TypeDescription { get; set; }
    }
}
