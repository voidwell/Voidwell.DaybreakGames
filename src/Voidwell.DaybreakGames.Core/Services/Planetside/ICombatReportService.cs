using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface ICombatReportService
    {
        Task<CombatReport> GetCombatReport(int worldId, int zoneId, DateTime startDate, DateTime? endDate);
        Task<CombatReportStats> GetCombatStats(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null);
    }
}
