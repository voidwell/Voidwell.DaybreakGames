using System;

namespace Voidwell.DaybreakGames.Models
{
    public class CaptureLogRow
    {
        public float FactionVs { get; set; }
        public float FactionNc { get; set; }
        public float FactionTr { get; set; }
        public string NewFactionId { get; set; }
        public string OldFactionId { get; set; }
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
        public string Id { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
    }
}
