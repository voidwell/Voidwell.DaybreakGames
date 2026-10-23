namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class VehicleDestroy : PayloadBase
{
    public long AttackerCharacterId { get; set; }
    public int? AttackerLoadoutId { get; set; }
    public int? AttackerVehicleId { get; set; }
    public int? AttackerWeaponId { get; set; }
    public long CharacterId { get; set; }
    public int? FacilityId { get; set; }
    public int? FactionId { get; set; }
    public int VehicleId { get; set; }
}
