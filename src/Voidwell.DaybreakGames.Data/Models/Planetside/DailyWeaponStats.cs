using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class DailyWeaponStats
    {
        [Required]
        public int WeaponId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int Kills { get; set; }
        public int Uniques { get; set; }
        public float Kpu { get; set; }
        public float AvgBr { get; set; }
        public float Q1Kpu { get; set; }
        public float Q2Kpu { get; set; }
        public float Q3Kpu { get; set; }
        public float Q4Kpu { get; set; }
        public int Headshots { get; set; }
        public int Q4Kills { get; set; }
        public int Q4Uniques { get; set; }
        public int Q4Headshots { get; set; }
        public int VehicleKills { get; set; }
        public int AircraftKills { get; set; }
        public float VehicleKpu { get; set; }
        public float AircraftKpu { get; set; }
    }
}
