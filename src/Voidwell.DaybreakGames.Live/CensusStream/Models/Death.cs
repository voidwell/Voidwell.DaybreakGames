namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class Death : PayloadBase
{
    public long AttackerCharacterId { get; set; }
    public int? AttackerFireModeId { get; set; }
    public int? AttackerLoadoutId { get; set; }
    public int? AttackerVehicleId { get; set; }
    public int? AttackerWeaponId { get; set; }
    public long CharacterId { get; set; }
    public int? CharacterLoadoutId { get; set; }
    public bool IsHeadshot { get; set; }
}
