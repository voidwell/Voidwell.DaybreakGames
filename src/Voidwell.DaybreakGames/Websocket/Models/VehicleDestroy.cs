namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class VehicleDestroy : PayloadBase
    {
        public string AttackerCharacterId { get; set; }
        public string AttackerLoadoutId { get; set; }
        public string AttackerVehicleId { get; set; }
        public string AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public string FacilityId { get; set; }
        public string FactionId { get; set; }
        public string VehicleId { get; set; }
    }
}
