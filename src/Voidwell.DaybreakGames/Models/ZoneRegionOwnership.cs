namespace Voidwell.DaybreakGames.Models
{
    public class ZoneRegionOwnership : ZoneRegion
    {
        public ZoneRegionOwnership(ZoneRegion region, int? factionId)
        {
            RegionId = region.RegionId;
            FacilityId = region.FacilityId;
            FacilityName = region.FacilityName;
            FacilityType = region.FacilityType;
            XPos = region.XPos;
            YPos = region.YPos;
            ZPos = region.ZPos;
            FactionId = factionId;
        }

        public int? FactionId { get; set; }
    }
}
