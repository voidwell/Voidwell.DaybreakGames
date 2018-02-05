using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICombatReportService
    {
        Task<CombatReport> GetCombatReport(int worldId, int zoneId, DateTime startDate, DateTime endDate);
    }
}
