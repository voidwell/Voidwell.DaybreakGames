namespace Voidwell.DaybreakGames.Models
{
    public class InfantryStats
    {
        public int? Weapons { get; set; }
        public int? UnsanctionedWeapons { get; set; }
        public int? Kills { get; set; }
        public double? Accuracy { get; set; }
        public double? HeadshotRatio { get; set; }
        public double? KillDeathRatio { get; set; }
        public double? KDRPadding { get; set; }
        public double? AccuracyDelta { get; set; }
        public double? HeadshotRatioDelta { get; set; }
        public int? IVIScore { get; set; }
    }
}
