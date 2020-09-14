using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public static class StatValueMapper
    {
        public static void AssignStatValue(ref CharacterLifetimeStat dbModel, string statName, int value)
        {
            switch (statName)
            {
                case "achievement_count":
                    dbModel.AchievementCount = value;
                    break;
                case "assist_count":
                    dbModel.AssistCount = value;
                    break;
                case "facility_defended_count":
                    dbModel.FacilityDefendedCount = value;
                    break;
                case "medal_count":
                    dbModel.MedalCount = value;
                    break;
                case "skill_points":
                    dbModel.SkillPoints = value;
                    break;
                case "weapon_deaths":
                    dbModel.WeaponDeaths = value;
                    break;
                case "weapon_fire_count":
                    dbModel.WeaponFireCount = value;
                    break;
                case "weapon_hit_count":
                    dbModel.WeaponHitCount = value;
                    break;
                case "weapon_play_time":
                    dbModel.WeaponPlayTime = value;
                    break;
                case "weapon_score":
                    dbModel.WeaponScore = value;
                    break;
                case "domination_count":
                    dbModel.DominationCount = value;
                    break;
                case "facility_capture_count":
                    dbModel.FacilityCaptureCount = value;
                    break;
                case "revenge_count":
                    dbModel.RevengeCount = value;
                    break;
                case "weapon_damage_given":
                    dbModel.WeaponDamageGiven = value;
                    break;
                case "weapon_damage_taken_by":
                    dbModel.WeaponDamageTakenBy = value;
                    break;
                case "weapon_headshots":
                    dbModel.WeaponHeadshots = value;
                    break;
                case "weapon_kills":
                    dbModel.WeaponKills = value;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.WeaponVehicleKills = value;
                    break;
            }
        }

        public static void AssignStatValue(ref CharacterStat dbModel, string statName, int value)
        {
            switch (statName)
            {
                case "deaths":
                    dbModel.Deaths = value;
                    break;
                case "fire_count":
                    dbModel.FireCount = value;
                    break;
                case "hit_count":
                    dbModel.HitCount = value;
                    break;
                case "play_time":
                    dbModel.PlayTime = value;
                    break;
                case "score":
                    dbModel.Score = value;
                    break;
                case "killed_by":
                    dbModel.KilledBy = value;
                    break;
                case "kills":
                    dbModel.Kills = value;
                    break;
            }
        }

        public static void AssignStatValue(ref CharacterLifetimeStatByFaction dbModel, string statName, int valueVs, int valueNc, int valueTr)
        {
            switch (statName)
            {
                case "domination_count":
                    dbModel.DominationCountVS = valueVs;
                    dbModel.DominationCountNC = valueNc;
                    dbModel.DominationCountTR = valueTr;
                    break;
                case "facility_capture_count":
                    dbModel.FacilityCaptureCountVS = valueVs;
                    dbModel.FacilityCaptureCountNC = valueNc;
                    dbModel.FacilityCaptureCountTR = valueTr;
                    break;
                case "revenge_count":
                    dbModel.RevengeCountVS = valueVs;
                    dbModel.RevengeCountNC = valueNc;
                    dbModel.RevengeCountTR = valueTr;
                    break;
                case "weapon_damage_given":
                    dbModel.WeaponDamageGivenVS = valueVs;
                    dbModel.WeaponDamageGivenNC = valueNc;
                    dbModel.WeaponDamageGivenTR = valueTr;
                    break;
                case "weapon_damage_taken_by":
                    dbModel.WeaponDamageTakenByVS = valueVs;
                    dbModel.WeaponDamageTakenByNC = valueNc;
                    dbModel.WeaponDamageTakenByTR = valueTr;
                    break;
                case "weapon_headshots":
                    dbModel.WeaponHeadshotsVS = valueVs;
                    dbModel.WeaponHeadshotsNC = valueNc;
                    dbModel.WeaponHeadshotsTR = valueTr;
                    break;
                case "weapon_killed_by":
                    dbModel.WeaponKilledByVS = valueVs;
                    dbModel.WeaponKilledByNC = valueNc;
                    dbModel.WeaponKilledByTR = valueTr;
                    break;
                case "weapon_kills":
                    dbModel.WeaponKillsVS = valueVs;
                    dbModel.WeaponKillsNC = valueNc;
                    dbModel.WeaponKillsTR = valueTr;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.WeaponVehicleKillsVS = valueVs;
                    dbModel.WeaponVehicleKillsNC = valueNc;
                    dbModel.WeaponVehicleKillsTR = valueTr;
                    break;
            }
        }

        public static void AssignStatValue(ref CharacterStatByFaction dbModel, string statName, int valueVs, int valueNc, int valueTr)
        {
            switch (statName)
            {
                case "killed_by":
                    dbModel.KilledByVS = valueVs;
                    dbModel.KilledByNC = valueNc;
                    dbModel.KilledByTR = valueTr;
                    break;
                case "kills":
                    dbModel.KillsVS = valueVs;
                    dbModel.KillsNC = valueNc;
                    dbModel.KillsTR = valueTr;
                    break;
            }
        }

        public static void AssignStatValue(ref CharacterWeaponStat dbModel, string statName, int value)
        {
            switch (statName)
            {
                case "weapon_deaths":
                    dbModel.Deaths = value;
                    break;
                case "weapon_fire_count":
                    dbModel.FireCount = value;
                    break;
                case "weapon_hit_count":
                    dbModel.HitCount = value;
                    break;
                case "weapon_play_time":
                    dbModel.PlayTime = value;
                    break;
                case "weapon_score":
                    dbModel.Score = value;
                    break;
                case "weapon_damage_given":
                    dbModel.DamageGiven = value;
                    break;
                case "weapon_headshots":
                    dbModel.Headshots = value;
                    break;
                case "weapon_killed_by":
                    dbModel.KilledBy = value;
                    break;
                case "weapon_kills":
                    dbModel.Kills = value;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.VehicleKills = value;
                    break;
                case "weapon_damage_taken_by":
                    dbModel.DamageTakenBy = value;
                    break;
            }
        }

        public static void AssignStatValue(ref CharacterWeaponStatByFaction dbModel, string statName, int valueVs, int valueNc, int valueTr)
        {
            switch (statName)
            {
                case "weapon_damage_taken_by":
                    dbModel.DamageTakenByVS = valueVs;
                    dbModel.DamageTakenByNC = valueNc;
                    dbModel.DamageTakenByTR = valueTr;
                    break;
                case "weapon_damage_given":
                    dbModel.DamageGivenVS = valueVs;
                    dbModel.DamageGivenNC = valueNc;
                    dbModel.DamageGivenTR = valueTr;
                    break;
                case "weapon_headshots":
                    dbModel.HeadshotsVS = valueVs;
                    dbModel.HeadshotsNC = valueNc;
                    dbModel.HeadshotsTR = valueTr;
                    break;
                case "weapon_killed_by":
                    dbModel.KilledByVS = valueVs;
                    dbModel.KilledByNC = valueNc;
                    dbModel.KilledByTR = valueTr;
                    break;
                case "weapon_kills":
                    dbModel.KillsVS = valueVs;
                    dbModel.KillsNC = valueNc;
                    dbModel.KillsTR = valueTr;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.VehicleKillsVS = valueVs;
                    dbModel.VehicleKillsNC = valueNc;
                    dbModel.VehicleKillsTR = valueTr;
                    break;
            }
        }
    }
}
