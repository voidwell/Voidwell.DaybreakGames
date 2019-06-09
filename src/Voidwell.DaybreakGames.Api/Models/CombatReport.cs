using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class CombatReport
    {
        public CombatReportStats Stats { get; set; }
        public IEnumerable<CaptureLogRow> CaptureLog { get; set; }
    }
}
