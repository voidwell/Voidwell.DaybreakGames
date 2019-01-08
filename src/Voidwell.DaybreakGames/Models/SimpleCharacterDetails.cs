using System;

namespace Voidwell.DaybreakGames.Models
{
    public class SimpleCharacterDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string World { get; set; }
        public int FactionId { get; set; }
        public string FactionName { get; set; }
        public int? FactionImageId { get; set; }
        public int BattleRank { get; set; }
        public string OutfitAlias { get; set; }
        public string OutfitName { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int PlayTime { get; set; }
        public int TotalPlayTimeMinutes { get; set; }
        public int Score { get; set; }
        public double KillDeathRatio { get; set; }
        public double HeadshotRatio { get; set; }
        public double KillsPerHour { get; set; }
        public double TotalKillsPerHour { get; set; }
        public double SiegeLevel { get; set; }
        public int IVIScore { get; set; }
        public DateTime? LastSaved { get; set; }
        public int Prestige { get; set; }
    }
}
