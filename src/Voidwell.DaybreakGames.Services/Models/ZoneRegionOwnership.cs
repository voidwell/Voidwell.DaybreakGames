namespace Voidwell.DaybreakGames.Services.Models
{
    public class ZoneRegionOwnership
    {
        public ZoneRegionOwnership(int zoneId, int regionId, int factionId)
        {
            ZoneId = zoneId;
            RegionId = regionId;
            FactionId = factionId;
        }

        public int ZoneId { get; set; }
        public int RegionId { get; set; }
        public int FactionId { get; set; }
    }
}
