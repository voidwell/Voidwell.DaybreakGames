using System;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class CombatReportRequest
    {
        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
