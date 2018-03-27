namespace Voidwell.DaybreakGames.Data.Repositories.Models
{
    public class WeaponAggregate
    {
        public int ItemId { get; set; }
        public int? VehicleId { get; set; }
        public double? AVGKills { get; set; }
        public double? STDKills { get; set; }
        public double? AVGDeaths { get; set; }
        public double? STDDeaths { get; set; }
        public double? AVGFireCount { get; set; }
        public double? STDFireCount { get; set; }
        public double? AVGHitCount { get; set; }
        public double? STDHitCount { get; set; }
        public double? AVGHeadshots { get; set; }
        public double? STDHeadshots { get; set; }
        public double? AVGPlayTime { get; set; }
        public double? STDPlayTime { get; set; }
        public double? AVGScore { get; set; }
        public double? STDScore { get; set; }
        public double? AVGVehicleKills { get; set; }
        public double? STDVehicleKills { get; set; }

        public double? AVGKdr { get; set; }
        public double? STDKdr { get; set; }
        public double? AVGAccuracy { get; set; }
        public double? STDAccuracy { get; set; }
        public double? AVGHsr { get; set; }
        public double? STDHsr { get; set; }
        public double? AVGKph { get; set; }
        public double? STDKph { get; set; }
        public double? AVGVkph { get; set; }
        public double? STDVkph { get; set; }
    }
}
