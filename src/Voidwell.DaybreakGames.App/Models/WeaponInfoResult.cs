using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class WeaponInfoResult
    {
        public string Name { get; set; }
        public int ItemId { get; set; }
        public string Category { get; set; }
        public int? FactionId { get; set; }
        public int? ImageId { get; set; }
        public string Description { get; set; }
        public int MaxStackSize { get; set; }
        public string Range { get; set; }
        public int? FireRateMs { get; set; }
        public int? ClipSize { get; set; }
        public int? Capacity { get; set; }
        public int? MuzzleVelocity { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int? MinDamageRange { get; set; }
        public int? MaxDamageRange { get; set; }
        public int? IndirectMaxDamage { get; set; } 
        public int? IndirectMinDamage { get; set; }
        public float? IndirectMaxDamageRange { get; set; }
        public float? IndirectMinDamageRange { get; set; }
        public int MinReloadSpeed { get; set; }
        public int MaxReloadSpeed { get; set; }
        public float? IronSightZoom { get; set; }
        public IEnumerable<string> FireModes { get; set; }
        public AccuracyState HipAcc { get; set; }
        public AccuracyState AimAcc { get; set; }
        public bool IsVehicleWeapon { get; set; }
        public int? DamageRadius { get; set; }
    }

    public class AccuracyState
    {
        public float? Crouching { get; set; }
        public float? CrouchWalking { get; set; }
        public float? Standing { get; set; }
        public float? Running { get; set; }
        public float? Cof { get; set; }
    }
}
