using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
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
        public CombatReportParticipantStats(string characterId, string outfitId = null)
        {
            Character = new CombatReportCharacterDetail(characterId);
            Character.Name = characterId;

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
    }

    public class CombatReportOutfitStats
    {
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
    }

    public class CombatReportClassStats
    {
        public CombatReportClassStats(int profileId)
        {
            Profile = new CombatReportClassDetail(profileId);
        }

        public CombatReportClassDetail Profile { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Headshots { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int VehicleKills { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class CombatReportWeaponStats
    {
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
        public CombatReportCharacterDetail(string characterId) : base(characterId)
        { }

        public int? BattleRank { get; set; }
        public int? PrestigeLevel { get; set; }
    }

    public class CombatReportOutfitDetail : CombatReportItemDetail
    {
        public CombatReportOutfitDetail(string outfitId) : base(outfitId)
        { }

        public string Alias { get; set; }
    }

    public class CombatReportClassDetail : CombatReportItemDetail
    {
        public CombatReportClassDetail(int profileId) : base(profileId)
        { }

        public int? TypeId { get; set; }
    }

    public class CombatReportItemDetail
    {
        public CombatReportItemDetail()
        {
        }

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
