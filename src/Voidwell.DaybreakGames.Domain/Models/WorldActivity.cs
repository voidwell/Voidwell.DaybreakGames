using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class WorldActivity
    {
        public DateTime ActivityPeriodStart { get; set; }
        public DateTime ActivityPeriodEnd { get; set; }
        public IEnumerable<PopulationPeriod> HistoricalPopulations { get; set; }
        public WorldActivityStats Stats { get; set; }
        public IEnumerable<CombatReportClassStats> ClassStats { get; set; }
        public IEnumerable<CombatReportVehicleStats> TopVehicles { get; set; }
        public IEnumerable<CombatReportParticipantStats> TopPlayers { get; set; }
        public IEnumerable<CombatReportOutfitStats> TopOutfits { get; set; }
        public IEnumerable<CombatReportWeaponStats> TopWeapons { get; set; }
        public WorldActivityExperience TopExperience { get; set; }
    }

    public class WorldActivityStats
    {
        public FactionValues Kills { get; set; } = new FactionValues();
        public FactionValues Deaths { get; set; } = new FactionValues();
        public FactionValues Headshots { get; set; } = new FactionValues();
        public FactionValues TeamKills { get; set; } = new FactionValues();
        public FactionValues Suicides { get; set; } = new FactionValues();
        public FactionValues VehicleKills { get; set; } = new FactionValues();
        public FactionValues KDR { get; set; } = new FactionValues();
        public FactionValues HSR { get; set; } = new FactionValues();
    }

    public class WorldActivityExperience
    {
        public WorldActivityExperienceFactions Heals { get; set; }
        public WorldActivityExperienceFactions Revives { get; set; }
        public WorldActivityExperienceFactions Roadkills { get; set; }
        public WorldActivityExperienceFactions SquadBeaconKills { get; set; }
    }

    public class WorldActivityExperienceFactions
    {
        public IEnumerable<WorldActivityExperienceItem> VS { get; set; }
        public IEnumerable<WorldActivityExperienceItem> NC { get; set; }
        public IEnumerable<WorldActivityExperienceItem> TR { get; set; }
        public IEnumerable<WorldActivityExperienceItem> NS { get; set; }
    }

    public class WorldActivityExperienceItem
    {
        public WorldActivityExperienceItem(string characterId, int ticks)
        {
            CharacterId = characterId;
            Ticks = ticks;
        }

        public string CharacterId { get; set; }
        public string CharacterName { get; set; }
        public int? CharacterBattleRank { get; set; }
        public int? CharacterFactionId { get; set; }
        public int? CharacterPrestigeLevel { get; set; }
        public string CharacterOutfitAlias { get; set; }
        public int Ticks { get; set; }
    }
}
