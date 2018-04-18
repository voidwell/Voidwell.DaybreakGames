namespace Voidwell.DaybreakGames.Models
{
    public class ZoneRegionOwnership : ZoneRegion
    {
        public ZoneRegionOwnership(ZoneRegion region, int? factionId)
        {
            RegionId = region.RegionId;
            FacilityId = region.FacilityId;
            FacilityName = region.FacilityName;
            FacilityType = region.FacilityType?.ToLower().Replace(" ", "_");
            X = region.X;
            Y = region.Y;
            Z = region.Z;
            FactionId = factionId;
        }

        public int? FactionId { get; set; }
    }
}
