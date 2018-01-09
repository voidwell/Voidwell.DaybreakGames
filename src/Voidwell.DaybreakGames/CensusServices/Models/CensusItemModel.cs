namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusItemModel
    {
        public string ItemId { get; set; }
        public string ItemTypeId { get; set; }
        public int? ItemCategoryId { get; set; }
        public bool IsVehicleWeapon { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public string FactionId { get; set; }
        public int MaxStackSize { get; set; }
        public string ImageId { get; set; }
    }
}
