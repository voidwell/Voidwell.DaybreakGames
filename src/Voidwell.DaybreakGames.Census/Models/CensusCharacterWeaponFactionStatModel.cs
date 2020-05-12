namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusCharacterWeaponFactionStatModel
    {
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public int ItemId { get; set; }
        public int VehicleId { get; set; }
        public int ValueVs { get; set; }
        public int ValueNc { get; set; }
        public int ValueTr { get; set; }
    }
}
