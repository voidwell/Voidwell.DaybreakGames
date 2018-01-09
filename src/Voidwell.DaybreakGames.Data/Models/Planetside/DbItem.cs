using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Item")]
    public class DbItem
    {
        [Required]
        public string Id { get; set; }

        public string ItemTypeId { get; set; }
        public int? ItemCategoryId { get; set; }
        public bool IsVehicleWeapon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FactionId { get; set; }
        public int MaxStackSize { get; set; }
        public string ImageId { get; set; }

        [ForeignKey("ItemCategoryId")]
        public DbItemCategory ItemCategory { get; set; }
    }
}
