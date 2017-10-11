namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusCharacterWeaponFactionStatModel
    {
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public string ItemId { get; set; }
        public string VehicleId { get; set; }
        public int ValueVs { get; set; }
        public int ValueNc { get; set; }
        public int ValueTr { get; set; }
    }
}
