namespace Voidwell.DaybreakGames.Messages.Models
{
    public class PlayerDeathMessage : PlanetsideCharacterMessage
    {
        public PlayerDeathMessage()
        {
            Type = "PlayerDeath";
        }

        public string AttackerCharacterId { get; set; }
        public int? AttackerFactionId { get; set; }
        public string AttackerCharacterName { get; set; }
        public string AttackerOutfitId { get; set; }
        public string VictimCharacterId { get; set; }
        public int? VictimFactionId { get; set; }
        public string VictimCharacterName { get; set; }
        public string VictimOutfitId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public bool IsHeadshot { get; set; }
    }
}
