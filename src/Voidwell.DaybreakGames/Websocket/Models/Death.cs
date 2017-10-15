namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class Death : PayloadBase
    {
        public string AttackerCharacterId { get; set; }
        public string AttackerFireModeId { get; set; }
        public string AttackerLoadoutId { get; set; }
        public string AttackerVehicleId { get; set; }
        public string AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public string CharacterLoadoutId { get; set; }
        public bool IsHeadshot { get; set; }
    }
}
