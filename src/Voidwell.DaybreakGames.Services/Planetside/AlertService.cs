using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ICombatReportService _combatReportService;
    private readonly IMapService _mapService;
    private readonly ICache _cache;

    private enum METAGAME_EVENT_STATE
    {
        STARTED = 135,
        RESTARTED = 136,
        CANCELED = 137,
        ENDED = 138,
        XPCHANGE = 139
    }

    private const string _cacheKey = "ps2.alert";
    private readonly TimeSpan _cacheAlertsExpiration = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _cacheAlertExpiration = TimeSpan.FromMinutes(5);

    public AlertService(IAlertRepository alertRepository, ICombatReportService combatReportService,
        IMapService mapService, ICache cache)
    {
        _alertRepository = alertRepository;
        _combatReportService = combatReportService;
        _mapService = mapService;
        _cache = cache;
    }

    public async Task<IEnumerable<Alert>?> GetAlerts(int pageNumber, int? worldId = null, int limit = 10)
    {
        var cacheKey = $"{_cacheKey}_alerts_{pageNumber}:{worldId}";

        return await _cache.GetOrSetAsync(
            cacheKey,
            ct => _alertRepository.GetAlerts(pageNumber, limit, worldId),
            _cacheAlertsExpiration,
            alerts => alerts != null && alerts.Any());
    }

    public Task<IEnumerable<Alert>> GetActiveAlertsByWorldId(int worldId)
    {
        return _alertRepository.GetActiveAlertsByWorldId(worldId)!;
    }

    public async Task<AlertResult?> GetAlert(int worldId, int instanceId)
    {
        var cacheKey = $"{_cacheKey}_alert_{worldId}_{instanceId}";

        return await _cache.GetOrSetIfNotNullAsync<AlertResult>(cacheKey, async ct =>
        {
            var alert = await _alertRepository.GetAlert(worldId, instanceId);
            if (alert?.ZoneId == null || !alert.StartDate.HasValue)
            {
                return null;
            }

            var combatReportTask = _combatReportService.GetCombatReport(alert.WorldId, alert.ZoneId.Value, alert.StartDate.Value, alert.EndDate);
            var zoneSnapshotTask = _mapService.GetZoneSnapshotByMetagameEvent(worldId, instanceId);

            await Task.WhenAll(combatReportTask, zoneSnapshotTask);

            if (combatReportTask.Result == null)
            {
                return null;
            }

            var neuturalScore = 0.0f;
            if (alert.MetagameEvent?.Type == 1 || alert.MetagameEvent?.Type == 8 || alert.MetagameEvent?.Type == 9)
            {
                neuturalScore = 100 - (alert.LastFactionVs.GetValueOrDefault() + alert.LastFactionVs.GetValueOrDefault() + alert.LastFactionTr.GetValueOrDefault() + alert.LastFactionNs.GetValueOrDefault());
            }

            var alertResult = new AlertResult
            {
                WorldId = alert.WorldId,
                ZoneId = alert.ZoneId,
                MetagameInstanceId = alert.MetagameInstanceId,
                MetagameEventId = alert.MetagameEventId,
                StartDate = alert.StartDate,
                EndDate = alert.EndDate,
                StartFactionVS = alert.StartFactionVs.GetValueOrDefault(),
                StartFactionNC = alert.StartFactionNc.GetValueOrDefault(),
                StartFactionTR = alert.StartFactionTr.GetValueOrDefault(),
                StartFactionNS = alert.StartFactionNs.GetValueOrDefault(),
                LastFactionVS = alert.LastFactionVs.GetValueOrDefault(),
                LastFactionNC = alert.LastFactionNc.GetValueOrDefault(),
                LastFactionTR = alert.LastFactionTr.GetValueOrDefault(),
                LastFactionNS = alert.LastFactionNs.GetValueOrDefault(),
                MetagameEvent = alert.MetagameEvent,
                Log = combatReportTask.Result,
                Score = new[] {
                    neuturalScore,
                    alert.LastFactionVs.GetValueOrDefault(),
                    alert.LastFactionNc.GetValueOrDefault(),
                    alert.LastFactionTr.GetValueOrDefault(),
                    alert.LastFactionNs.GetValueOrDefault()
                },
                ServerId = alert.WorldId.ToString(),
                MapId = alert.ZoneId.ToString(),
                ZoneSnapshot = zoneSnapshotTask.Result?.Ownership
            };

            return alertResult;
        }, _cacheAlertExpiration);
    }
}
