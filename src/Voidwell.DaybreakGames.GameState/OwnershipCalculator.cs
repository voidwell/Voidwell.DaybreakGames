using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.GameState
{
    public static class OwnershipCalculator
    {
        public static MapScore Calculate(WorldZone zoneMap, Dictionary<int, int> ownership)
        {
            var _ownership = ownership;

            var territories = new[] { 0, 0, 0, 0, 0 };
            var ampStations = new[] { 0, 0, 0, 0, 0 };
            var techPlants = new[] { 0, 0, 0, 0, 0 };
            var bioLabs = new[] { 0, 0, 0, 0, 0 };
            var largeOutposts = new[] { 0, 0, 0, 0, 0 };
            var smallOutposts = new[] { 0, 0, 0, 0, 0 };

            foreach (var region in ownership)
            {
                territories[region.Value]++;

                var facilityType = zoneMap.Regions.FirstOrDefault(a => a.RegionId == region.Key)?.FacilityType;
                switch (facilityType)
                {
                    case "Amp Station":
                        ampStations[region.Value]++;
                        break;
                    case "Tech Plant":
                        techPlants[region.Value]++;
                        break;
                    case "Bio Lab":
                        bioLabs[region.Value]++;
                        break;
                    case "Large Outpost":
                        largeOutposts[region.Value]++;
                        break;
                    case "Small Outpost":
                        smallOutposts[region.Value]++;
                        break;
                }
            }

            var connectedTerritories = CalculateConnectedTerritories(zoneMap, _ownership);

            return new MapScore
            {
                Territories = new OwnershipScoreFactions(territories[1], territories[2], territories[3], territories[4], territories[0]),
                ConnectedTerritories = new OwnershipScoreFactions(connectedTerritories[1], connectedTerritories[2], connectedTerritories[3], connectedTerritories[4], connectedTerritories[0]),
                AmpStations = new OwnershipScoreFactions(ampStations[1], ampStations[2], ampStations[3], ampStations[4], ampStations[0]),
                TechPlants = new OwnershipScoreFactions(techPlants[1], techPlants[2], techPlants[3], techPlants[4], techPlants[0]),
                BioLabs = new OwnershipScoreFactions(bioLabs[1], bioLabs[2], bioLabs[3], bioLabs[4], bioLabs[0]),
                LargeOutposts = new OwnershipScoreFactions(largeOutposts[1], largeOutposts[2], largeOutposts[3], largeOutposts[4], largeOutposts[0]),
                SmallOutposts = new OwnershipScoreFactions(smallOutposts[1], smallOutposts[2], smallOutposts[3], smallOutposts[4], smallOutposts[0])
            };
        }

        private static int[] CalculateConnectedTerritories(WorldZone zoneMap, Dictionary<int, int> ownership)
        {
            var connectedTerritories = new[] { 0, 0, 0, 0, 0 };
            var factionsChecked = new Dictionary<int, bool>();

            foreach (var warpgate in zoneMap.Warpgates)
            {
                var checkedRegions = new Dictionary<int, bool>();
                var focusFaction = ownership[warpgate.RegionId];

                if (!factionsChecked.ContainsKey(focusFaction))
                {
                    factionsChecked.Add(focusFaction, true);

                    connectedTerritories[focusFaction]++;
                    checkedRegions.Add(warpgate.RegionId, true);

                    var factionRegions = ownership.Where(a => a.Value == focusFaction).ToDictionary(a => a.Key, a => a.Value);
                    connectedTerritories[focusFaction] += CountConnectedRegions(warpgate, checkedRegions, factionRegions);
                }
            }

            return connectedTerritories;
        }

        private static int CountConnectedRegions(WorldZoneRegion source, Dictionary<int, bool> checkedRegions, Dictionary<int, int> factionRegions)
        {
            var connectedCount = 0;

            foreach (var linkedRegion in source.Links)
            {
                if (checkedRegions.ContainsKey(linkedRegion.RegionId))
                {
                    continue;
                }

                checkedRegions.Add(linkedRegion.RegionId, true);

                if (factionRegions.ContainsKey(linkedRegion.RegionId))
                {
                    connectedCount += 1 + CountConnectedRegions(linkedRegion, checkedRegions, factionRegions);
                }
            }

            return connectedCount;
        }
    }
}
