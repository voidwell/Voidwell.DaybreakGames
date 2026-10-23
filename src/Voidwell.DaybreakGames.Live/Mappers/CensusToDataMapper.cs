using Riok.Mapperly.Abstractions;
using CensusModels = Voidwell.DaybreakGames.Live.CensusStream.Models;
using DataModels = Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Live.Mappers;

// Stream events and their stored counterparts differ in bookkeeping members (event names, navigation properties), so full member coverage is not required.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class CensusToDataMapper
{
    private static readonly TimeSpan _alertDuration = TimeSpan.FromMinutes(45);

    public static partial DataModels.Events.AchievementEarned Map(CensusModels.AchievementEarned source);

    public static partial DataModels.Events.BattlerankUp Map(CensusModels.BattlerankUp source);

    [MapProperty(nameof(CensusModels.ContinentLock.VsPopulation), nameof(DataModels.Events.ContinentLock.PopulationVs))]
    [MapProperty(nameof(CensusModels.ContinentLock.NcPopulation), nameof(DataModels.Events.ContinentLock.PopulationNc))]
    [MapProperty(nameof(CensusModels.ContinentLock.TrPopulation), nameof(DataModels.Events.ContinentLock.PopulationTr))]
    public static partial DataModels.Events.ContinentLock Map(CensusModels.ContinentLock source);

    public static partial DataModels.Events.ContinentUnlock Map(CensusModels.ContinentUnlock source);

    public static partial DataModels.Events.Death Map(CensusModels.Death source);

    public static partial DataModels.Events.FacilityControl Map(CensusModels.FacilityControl source);

    [MapProperty(nameof(CensusModels.MetagameEvent.FactionVs), nameof(DataModels.Events.MetagameEvent.ZoneControlVs))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionNc), nameof(DataModels.Events.MetagameEvent.ZoneControlNc))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionTr), nameof(DataModels.Events.MetagameEvent.ZoneControlTr))]
    public static partial DataModels.Events.MetagameEvent Map(CensusModels.MetagameEvent source);

    [MapProperty(nameof(CensusModels.MetagameEvent.InstanceId), nameof(DataModels.Alert.MetagameInstanceId))]
    [MapProperty(nameof(CensusModels.MetagameEvent.Timestamp), nameof(DataModels.Alert.StartDate))]
    [MapPropertyFromSource(nameof(DataModels.Alert.EndDate), Use = nameof(GetAlertEndDate))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionVs), nameof(DataModels.Alert.StartFactionVs))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionNc), nameof(DataModels.Alert.StartFactionNc))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionTr), nameof(DataModels.Alert.StartFactionTr))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionVs), nameof(DataModels.Alert.LastFactionVs))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionNc), nameof(DataModels.Alert.LastFactionNc))]
    [MapProperty(nameof(CensusModels.MetagameEvent.FactionTr), nameof(DataModels.Alert.LastFactionTr))]
    [MapperIgnoreTarget(nameof(DataModels.Alert.MetagameEvent))]
    public static partial DataModels.Alert MapToAlert(CensusModels.MetagameEvent source);

    [MapProperty(nameof(CensusModels.PlayerFacilityCapture.OutfitId), nameof(DataModels.Events.PlayerFacilityCapture.OutfitId), Use = nameof(NullIfZero))]
    public static partial DataModels.Events.PlayerFacilityCapture Map(CensusModels.PlayerFacilityCapture source);

    [MapProperty(nameof(CensusModels.PlayerFacilityDefend.OutfitId), nameof(DataModels.Events.PlayerFacilityDefend.OutfitId), Use = nameof(NullIfZero))]
    public static partial DataModels.Events.PlayerFacilityDefend Map(CensusModels.PlayerFacilityDefend source);

    public static partial DataModels.Events.PlayerLogin Map(CensusModels.PlayerLogin source);

    public static partial DataModels.Events.PlayerLogout Map(CensusModels.PlayerLogout source);

    public static partial DataModels.Events.VehicleDestroy Map(CensusModels.VehicleDestroy source);

    private static DateTime? GetAlertEndDate(CensusModels.MetagameEvent source) => source.Timestamp + _alertDuration;

    private static string? NullIfZero(string? outfitId) => outfitId == "0" ? null : outfitId;
}
