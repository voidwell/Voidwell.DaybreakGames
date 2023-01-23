namespace Voidwell.DaybreakGames.Live.CensusStream.Models
{
    public class VehicleDestroy : PayloadBase
    {
        public string AttackerCharacterId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public int? FacilityId { get; set; }
        public int? FactionId { get; set; }
        public int VehicleId { get; set; }
    }
}
