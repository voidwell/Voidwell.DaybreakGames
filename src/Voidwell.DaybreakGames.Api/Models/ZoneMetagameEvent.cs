using System;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class ZoneMetagameEvent
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ZoneId { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
