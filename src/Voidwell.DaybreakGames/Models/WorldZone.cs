using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldZone
    {
        public List<WorldZoneRegion> Warpgates { get; private set; }
        public List<WorldZoneRegion> Regions { get; private set; }

        public WorldZone(ZoneMap zoneMap)
        {
            Warpgates = new List<WorldZoneRegion>();
            Regions = new List<WorldZoneRegion>();

            foreach(var region in zoneMap.Regions)
            {
                var stateRegion = new WorldZoneRegion
                {
                    RegionId = region.RegionId,
                    FacilityId = region.FacilityId,
                    FacilityName = region.FacilityName,
                    FacilityType = region.FacilityType,
                    X = region.X,
                    Y = region.Y,
                    Z = region.Z
                };

                if (region.FacilityType == "Warpgate")
                {
                    Warpgates.Add(stateRegion);
                }

                Regions.Add(stateRegion);
            }

            foreach(var link in zoneMap.Links)
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
}
