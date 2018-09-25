namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class WeaponAggregate
    {
        public int ItemId { get; set; }
        public int? VehicleId { get; set; }
        public float? AVGKills { get; set; }
        public float? STDKills { get; set; }
        public long? SumKills { get; set; }
        public float? AVGDeaths { get; set; }
        public float? STDDeaths { get; set; }
        public long? SumDeaths { get; set; }
        public float? AVGFireCount { get; set; }
        public float? STDFireCount { get; set; }
        public long? SumFireCount { get; set; }
        public float? AVGHitCount { get; set; }
        public float? STDHitCount { get; set; }
        public long? SumHitCount { get; set; }
        public float? AVGHeadshots { get; set; }
        public float? STDHeadshots { get; set; }
        public long? SumHeadshots { get; set; }
        public float? AVGPlayTime { get; set; }
        public float? STDPlayTime { get; set; }
        public long? SumPlayTime { get; set; }
        public float? AVGScore { get; set; }
        public float? STDScore { get; set; }
        public long? SumScore { get; set; }
        public float? AVGVehicleKills { get; set; }
        public float? STDVehicleKills { get; set; }
        public long? SumVehicleKills { get; set; }

        public float? AVGKdr { get; set; }
        public float? STDKdr { get; set; }
        public float? AVGAccuracy { get; set; }
        public float? STDAccuracy { get; set; }
        public float? AVGHsr { get; set; }
        public float? STDHsr { get; set; }
        public float? AVGKph { get; set; }
        public float? STDKph { get; set; }
        public float? AVGVkph { get; set; }
        public float? STDVkph { get; set; }
    }
}
