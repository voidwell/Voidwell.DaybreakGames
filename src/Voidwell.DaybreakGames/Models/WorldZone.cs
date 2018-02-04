using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldZone
    {
        public List<WorldZoneRegion> Warpgates { get; private set; }
        public List<WorldZoneRegion> Regions { get; private set; }

        public WorldZone(IEnumerable<FacilityLink> facilityLinks, IEnumerable<MapRegion> mapRegions)
        {
            Warpgates = new List<WorldZoneRegion>();
            Regions = new List<WorldZoneRegion>();

            foreach(var region in mapRegions)
            {
                var stateRegion = new WorldZoneRegion
                {
                    RegionId = region.Id,
                    FacilityId = region.FacilityId,
                    FacilityName = region.FacilityName,
                    FacilityType = region.FacilityType,
                    XPos = region.XPos,
                    YPos = region.YPos,
                    ZPos = region.ZPos
                };

                if (region.FacilityType == "Warpgate")
                {
                    Warpgates.Add(stateRegion);
                }

                Regions.Add(stateRegion);
            }

            foreach(var link in facilityLinks)
            {
                var facilityA = Regions.SingleOrDefault(r => r.FacilityId == link.FacilityIdA);
                var facilityB = Regions.SingleOrDefault(r => r.FacilityId == link.FacilityIdB);

                if (facilityA != null && facilityB != null)
                {
                    facilityA.Links.Add(facilityB);
                    facilityB.Links.Add(facilityA);
                }
            }
        }
    }

    public class WorldZoneRegion
    {
        public string RegionId { get; set; }
        public string FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public float XPos { get; set; }
        public float YPos { get; set; }
        public float ZPos { get; set; }
        public List<WorldZoneRegion> Links { get; set; } = new List<WorldZoneRegion>();
    }
}
