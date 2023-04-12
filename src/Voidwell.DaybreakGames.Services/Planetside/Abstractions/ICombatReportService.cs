using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface ICombatReportService
    {
        Task<CombatReport> GetCombatReport(int worldId, int zoneId, DateTime startDate, DateTime? endDate);
        Task<CombatReportStats> GetCombatStats(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
    }
}
