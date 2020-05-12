using System;

namespace Voidwell.DaybreakGames.Core.Models
{
    public class CaptureLogRow
    {
        public float? FactionVs { get; set; }
        public float? FactionNc { get; set; }
        public float? FactionTr { get; set; }
        public float? FactionNs { get; set; }
        public int? ZonePopVs { get; set; }
        public int? ZonePopNc { get; set; }
        public int? ZonePopTr { get; set; }
        public int? ZonePopNs { get; set; }
        public int? NewFactionId { get; set; }
        public int? OldFactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public CaptureLogRowOutfit Outfit { get; set; }
        public CaptureLogRowMapRegion MapRegion { get; set; }
    }

    public class CaptureLogRowOutfit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class CaptureLogRowMapRegion
    {
        public int Id { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
    }
}
