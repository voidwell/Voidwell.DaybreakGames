using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Services.Models
{
    public class CombatReportStats
    {
        public IEnumerable<CombatReportParticipantStats> Participants { get; set; }
        public IEnumerable<CombatReportOutfitStats> Outfits { get; set; }
        public IEnumerable<CombatReportWeaponStats> Weapons { get; set; }
        public IEnumerable<CombatReportVehicleStats> Vehicles { get; set; }
        public IEnumerable<CombatReportClassStats> Classes { get; set; }
    }

    public class CombatReportParticipantStats
    {
        public CombatReportParticipantStats() { }

        public CombatReportParticipantStats(string characterId, string outfitId = null)
        {
            Character = new CombatReportCharacterDetail(characterId) {Name = characterId};

            if (!string.IsNullOrEmpty(outfitId))
            {
                Outfit = new CombatReportOutfitDetail(outfitId);
            }
        }

        public CombatReportCharacterDetail Character { get; set; }
        public CombatReportOutfitDetail Outfit { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int VehicleKills { get; set; }
        public int? TopWeaponId { get; set; }
        public string TopWeaponName { get; set; }
        public int? TopProfileId { get; set; }
        public string TopProfileName { get; set; }
        public int? TopProfileImageId { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public int SessionKills { get; set; }
    }

    public class CombatReportOutfitStats
    {
        public CombatReportOutfitStats() { }

        public CombatReportOutfitStats(string outfitId)
        {
            Outfit = new CombatReportOutfitDetail(outfitId);
        }

        public CombatReportOutfitDetail Outfit { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int VehicleKills { get; set; }
        public int ParticipantCount { get; set; }
        public int FacilityCaptures { get; set; }
    }

    public class CombatReportClassStats
    {
        public CombatReportClassStats() { }

        public CombatReportClassStats(int loadoutId)
        {
            Profile = new CombatReportClassDetail(loadoutId);
        }

        public CombatReportClassDetail Profile { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int VehicleKills { get; set; }
    }

    public class CombatReportWeaponStats
    {
        public CombatReportWeaponStats() { }

        public CombatReportWeaponStats(int weaponItemId)
        {
            Item = new CombatReportItemDetail(weaponItemId);
        }

        public CombatReportItemDetail Item { get; set; }
        public int Kills { get; set; }
        public int Teamkills { get; set; }
        public int Headshots { get; set; }
    }

    public class CombatReportVehicleStats
    {
        public CombatReportVehicleStats() { }

        public CombatReportVehicleStats(int vehicleId)
        {
            Vehicle = new CombatReportItemDetail(vehicleId);
        }

        public CombatReportItemDetail Vehicle { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Teamkills { get; set; }
    }

    public class CombatReportCharacterDetail : CombatReportItemDetail
    {
        public CombatReportCharacterDetail() { }

        public CombatReportCharacterDetail(string characterId) : base(characterId)
        { }

        public int? BattleRank { get; set; }
        public int? PrestigeLevel { get; set; }
        public int? WorldId { get; set; }
    }

    public class CombatReportOutfitDetail : CombatReportItemDetail
    {
        public CombatReportOutfitDetail() { }

        public CombatReportOutfitDetail(string outfitId) : base(outfitId)
        { }

        public string Alias { get; set; }
    }

    public class CombatReportClassDetail : CombatReportItemDetail
    {
        public CombatReportClassDetail() { }

        public CombatReportClassDetail(int loadoutId) : base(loadoutId)
        { }

        public int? ProfileId { get; set; }
        public int? ProfileTypeId { get; set; }
        public int? ProfileImageId { get; set; }
    }

    public class CombatReportItemDetail
    {
        public CombatReportItemDetail() { }

        public CombatReportItemDetail(object id)
        {
            Id = id.ToString();
            Name = $"Unknown ({Id})";
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int? FactionId { get; set; }
    }
}
