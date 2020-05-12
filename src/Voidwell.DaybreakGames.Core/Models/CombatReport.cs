using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Core.Models
{
    public class CombatReport
    {
        public CombatReportStats Stats { get; set; }
        public IEnumerable<CaptureLogRow> CaptureLog { get; set; }
    }
}
