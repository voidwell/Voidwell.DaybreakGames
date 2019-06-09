namespace Voidwell.DaybreakGames.Api.Models
{
    public class ZoneRegionOwnership
    {
        public ZoneRegionOwnership(int regionId, int factionId)
        {
            RegionId = regionId;
            FactionId = factionId;
        }
        
        public int RegionId { get; set; }
        public int FactionId { get; set; }
    }
}
