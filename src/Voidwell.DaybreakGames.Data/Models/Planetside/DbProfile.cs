using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Profile")]
    public class DbProfile
    {
        [Required]
        public string Id { get; set; }

        public string ProfileTypeId { get; set; }
        public string FactionId { get; set; }
        public string Name { get; set; }
        public string ImageId { get; set; }
    }
}
