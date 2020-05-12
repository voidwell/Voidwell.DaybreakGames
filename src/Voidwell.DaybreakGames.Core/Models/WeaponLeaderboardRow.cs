namespace Voidwell.DaybreakGames.Core.Models
{
    public class WeaponLeaderboardRow
    {
        public string CharacterId { get; set; }
        public string Name { get; set; }
        public int FactionId { get; set; }
        public int WorldId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int ShotsFired { get; set; }
        public int ShotsHit { get; set; }
        public int PlayTime { get; set; }
        public int Score { get; set; }
        public int VehicleKills { get; set; }
        public double? KdrDelta { get; set; }
        public double? AccuracyDelta { get; set; }
        public double? HsrDelta { get; set; }
        public double? KphDelta { get; set; }
    }
}
