using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public static class PlayerSessionEventMapper
    {
        public static IEnumerable<PlayerSessionEvent> ToPlayerSessionEvent(IEnumerable<Death> events)
        {
            return events.Select(e => new PlayerSessionDeathEvent
            {
                Timestamp = e.Timestamp,
                ZoneId = e.ZoneId,
                Attacker = new CombatReportItemDetail
                {
                    Id = e.AttackerCharacterId,
                    Name = e.AttackerCharacter?.Name,
                    FactionId = e.AttackerCharacter?.FactionId
                },
                Victim = new CombatReportItemDetail
                {
                    Id = e.CharacterId,
                    Name = e.Character?.Name,
                    FactionId = e.Character?.FactionId
                },
                Weapon = new PlayerSessionWeapon
                {
                    Id = e.AttackerWeaponId.Value,
                    ImageId = e.AttackerWeapon?.ImageId,
                    Name = e.AttackerWeapon?.Name
                },
                IsHeadshot = e.IsHeadshot,
                AttackerFireModeId = e.AttackerFireModeId,
                AttackerLoadoutId = e.AttackerLoadoutId,
                AttackerOutfitId = e.AttackerOutfitId,
                AttackerVehicleId = e.AttackerVehicleId,
                CharacterLoadoutId = e.CharacterLoadoutId,
                CharacterOutfitId = e.CharacterOutfitId
            });
        }

        public static IEnumerable<PlayerSessionEvent> ToPlayerSessionEvent(IEnumerable<PlayerFacilityCapture> events)
        {
            return events.Select(e => new PlayerSessionFacilityCaptureEvent
            {
                Timestamp = e.Timestamp,
                ZoneId = e.ZoneId,
                Facility = new PlayerSessionFacility
                {
                    Id = e.FacilityId,
                    Name = e.Facility?.FacilityName,
                    TypeId = e.Facility?.FacilityTypeId,
                    TypeName = e.Facility?.FacilityType
                }
            });
        }

        public static IEnumerable<PlayerSessionEvent> ToPlayerSessionEvent(IEnumerable<PlayerFacilityDefend> events)
        {
            return events.Select(e => new PlayerSessionFacilityDefendEvent
            {
                Timestamp = e.Timestamp,
                ZoneId = e.ZoneId,
                Facility = new PlayerSessionFacility
                {
                    Id = e.FacilityId,
                    Name = e.Facility?.FacilityName,
                    TypeId = e.Facility?.FacilityTypeId,
                    TypeName = e.Facility?.FacilityType
                }
            });
        }

        public static IEnumerable<PlayerSessionEvent> ToPlayerSessionEvent(IEnumerable<BattlerankUp> events)
        {
            return events.Select(e => new PlayerSessionBattleRankUpEvent
            {
                Timestamp = e.Timestamp,
                ZoneId = e.ZoneId,
                BattleRank = e.BattleRank
            });
        }

        public static IEnumerable<PlayerSessionEvent> ToPlayerSessionEvent(IEnumerable<VehicleDestroy> events)
        {
            return events.Select(e =>
            {
                var sessionEvent = new PlayerSessionVehicleDestroyEvent
                {
                    Timestamp = e.Timestamp,
                    ZoneId = e.ZoneId,
                    Attacker = new CombatReportItemDetail
                    {
                        Id = e.AttackerCharacterId,
                        Name = e.AttackerCharacter?.Name,
                        FactionId = e.AttackerCharacter?.FactionId
                    },
                    Victim = new CombatReportItemDetail
                    {
                        Id = e.CharacterId,
                        Name = e.Character?.Name,
                        FactionId = e.Character?.FactionId
                    },
                    Weapon = new PlayerSessionWeapon
                    {
                        Id = e.AttackerWeaponId.Value,
                        ImageId = e.AttackerWeapon?.ImageId,
                        Name = e.AttackerWeapon?.Name
                    },
                    VictimVehicle = new PlayerSessionVehicle
                    {
                        Id = e.VehicleId.Value,
                        ImageId = e.VictimVehicle?.ImageId,
                        Name = e.VictimVehicle?.Name
                    },
                    AttackerLoadoutId = e.AttackerLoadoutId,
                    AttackerVehicleId = e.AttackerVehicleId,
                    FactionId = e.FactionId
                };

                if (e.FacilityId.HasValue && e.FacilityId != 0)
                {
                    sessionEvent.Facility = new PlayerSessionFacility
                    {
                        Id = e.FacilityId.Value,
                        Name = e.Facility?.FacilityName,
                        TypeId = e.Facility?.FacilityTypeId,
                        TypeName = e.Facility?.FacilityType
                    };
                }

                return sessionEvent;
            });
        }
    }
}
