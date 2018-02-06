using System.Collections.Generic;

namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusWeaponInfoModel
    {
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public WeaponCategory Category { get; set; }
        public int? FactionId { get; set; }
        public int? ImageId { get; set; }
        public int MaxStackSize { get; set; }
        public WeaponDatasheet Datasheet { get; set; }
        public IEnumerable<WeaponFireMode> FireMode { get; set; }
        public bool IsVehicleWeapon { get; set; }

        public class WeaponCategory
        {
            public MultiLanguageString Name { get; set; }
        }

        public class WeaponDatasheet
        {
            public MultiLanguageString Range { get; set; }
            public int FireRateMs { get; set; }
            public int ClipSize { get; set; }
            public int Capacity { get; set; }
        }

        public class WeaponFireMode
        {
            public string Type { get; set; }
            public MultiLanguageString Description { get; set; }
            public int Speed { get; set; }
            public int DamageMin { get; set; }
            public int DamageMax { get; set; }
            public int DamageMinRange { get; set; }
            public int DamageMaxRange { get; set; }
            public int ReloadTimeMs { get; set; }
            public int ReloadChamberTimeMs { get; set; }
            public float CofRecoil { get; set; }
            public IEnumerable<WeaponFireModeState> States { get; set; }
            public float DefaultZoom { get; set; }
        }

        public class WeaponFireModeState
        {
            public string PlayerState { get; set; }
            public float MinConeOfFire { get; set; }
        }
    }
}
