namespace Voidwell.DaybreakGames.Models
{
    public class WeaponLeaderboardRow
    {
        public string CharacterId { get; set; }
        public string Name { get; set; }
        public string FactionId { get; set; }
        public string WorldId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int ShotsFired { get; set; }
        public int ShotsHit { get; set; }
        public int PlayTime { get; set; }
        public int Score { get; set; }
        public int VehicleKills { get; set; }
        public double KillDeathRatio { get; set; }
        public double HeadshotRatio { get; set; }
        public double Accuracy { get; set; }
        public double ScorePerMinute { get; set; }
        public double KillsPerHour { get; set; }
        public double VehicleKillsPerHour { get; set; }
    }
}
