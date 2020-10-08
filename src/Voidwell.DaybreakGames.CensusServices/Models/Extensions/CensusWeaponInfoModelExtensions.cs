using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.CensusServices.Models.Extensions
{
    public static class CensusWeaponInfoModelExtensions
    {
        public enum FireModeType
        {
            Primary,
            Secondary
        }

        public enum FireModeState {
            Crouching,
            CrouchWalking,
            Standing,
            Running
        }

        public static string GetName(this CensusWeaponInfoModel model)
        {
            return model.Name?.English;
        }

        public static string GetCategory(this CensusWeaponInfoModel model)
        {
            return model.Category?.Name?.English;
        }

        public static string GetDescription(this CensusWeaponInfoModel model)
        {
            return model.Description?.English;
        }

        public static string GetRange(this CensusWeaponInfoModel model)
        {
            return model.Datasheet?.Range?.English;
        }

        public static int? GetWeaponSpeed(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.FirstOrDefault()?.Speed;
        }

        public static int? GetMinDamage(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Min(m => m.DamageMin)
                ?? model.Datasheet?.DamageMin
                ?? model.FireMode.Min(m => m.Damage);
        }

        public static int? GetMaxDamage(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(m => m.DamageMax)
                ?? model.Datasheet?.DamageMax
                ?? model.FireMode.Max(m => m.Damage);
        }

        public static int? GetMinDamageRange(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Min(m => m.DamageMinRange);
        }

        public static int? GetMaxDamageRange(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(m => m.DamageMaxRange);
        }

        public static int? GetMinReloadSpeed(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Min(a => GetReloadSum(a))
                ?? model.Datasheet?.ReloadMsMin
                ?? model.FireMode?.Min(a => a.ReloadTimeMs);
        }

        public static int? GetMaxReloadSpeed(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(a => GetReloadSum(a))
                ?? model.Datasheet?.ReloadMsMax
                ?? model.FireMode?.Max(a => a.ReloadTimeMs);
        }

        public static IEnumerable<CensusWeaponInfoModel.WeaponFireMode> GetFireModesOfType(this CensusWeaponInfoModel model, FireModeType type)
        {
            return model.FireMode?.Where(a => a.Type == type.ToString().ToLower());
        }

        public static int? GetIndirectMinDamage(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Min(m => m.IndirectDamageMin);
        }

        public static int? GetIndirectMaxDamage(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(m => m.IndirectDamageMax);
        }

        public static float? GetIndirectMinDamageRange(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Min(m => m.IndirectDamageMinRange);
        }

        public static float? GetIndirectMaxDamageRange(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(m => m.IndirectDamageMaxRange);
        }

        public static int? GetDamageRadius(this CensusWeaponInfoModel model)
        {
            return model.FireMode?.Max(m => m.DamageRadius);
        }

        private static int? GetReloadSum(CensusWeaponInfoModel.WeaponFireMode fm)
        {
            return fm.ReloadTimeMs.HasValue && fm.ReloadChamberTimeMs.HasValue
                    ? fm.ReloadTimeMs.Value + fm.ReloadChamberTimeMs.Value
                    : null as int?;
        }

        public static float? GetFireModeStateCoF(this CensusWeaponInfoModel.WeaponFireMode mode, FireModeState state)
        {
            return mode
                ?.States
                ?.FirstOrDefault(a => a.PlayerState == state.ToString())
                ?.MinConeOfFire;
        }

        public static float? GetDefaultZoom(this IEnumerable<CensusWeaponInfoModel.WeaponFireMode> modes)
        {
            return modes
                ?.FirstOrDefault()
                ?.DefaultZoom;
        }

        public static IEnumerable<string> GetFireModeNames(this IEnumerable<CensusWeaponInfoModel.WeaponFireMode> modes)
        {
            return modes
                ?.Select(m => m.Description?.English)
                ?.Distinct();
        }
    }
}
