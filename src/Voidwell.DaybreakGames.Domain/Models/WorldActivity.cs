namespace Voidwell.DaybreakGames.Domain.Models;

public class WorldActivity
{
    public DateTime ActivityPeriodStart { get; set; }
    public DateTime ActivityPeriodEnd { get; set; }
    public IEnumerable<PopulationPeriod>? HistoricalPopulations { get; set; }
    public WorldActivityStats? Stats { get; set; }
    public IEnumerable<CombatReportClassStats>? ClassStats { get; set; }
    public IEnumerable<CombatReportVehicleStats>? TopVehicles { get; set; }
    public IEnumerable<CombatReportParticipantStats>? TopPlayers { get; set; }
    public IEnumerable<CombatReportOutfitStats>? TopOutfits { get; set; }
    public IEnumerable<CombatReportWeaponStats>? TopWeapons { get; set; }
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
