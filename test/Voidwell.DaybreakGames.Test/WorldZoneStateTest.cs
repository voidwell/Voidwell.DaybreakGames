using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Census.Test
{
    [TestClass]
    public class WorldZoneStateTest
    {
        [TestMethod]
        public void WorldZoneState_CalculateScore()
        {
            var zoneMap = GetZoneMap();

            var ownership = GetMapOwnership();
            var zone = new Zone { Id = 2, Code = "Indar" };

            var worldZoneState = new WorldZoneState(17, zone, zoneMap, ownership);

            var score = worldZoneState.MapScore;

            var percentTotal = score.Territories.Neutural.Percent + score.Territories.Vs.Percent + score.Territories.Nc.Percent + score.Territories.Tr.Percent;
            var connectedPercentTotal = score.ConnectedTerritories.Neutural.Percent + score.ConnectedTerritories.Vs.Percent + score.ConnectedTerritories.Nc.Percent + score.ConnectedTerritories.Tr.Percent;

            Assert.AreEqual(1, percentTotal);
            Assert.AreEqual(1, connectedPercentTotal);
        }

        private ZoneMap GetZoneMap()
        {
            var links = GetFacilityLinks().Select(a => new ZoneLink { FacilityIdA = a.FacilityIdA, FacilityIdB = a.FacilityIdB });
            var regions = GetMapRegions().Select(a => new ZoneRegion
            {
                RegionId = a.Id,
                FacilityId = a.FacilityId,
                FacilityName = a.FacilityName,
                FacilityType = a.FacilityType,
                FacilityTypeId = a.FacilityTypeId,
                X = a.XPos,
                Y = a.YPos
            });

            return new ZoneMap
            {
                Regions = regions,
                Links = links
            };
        }

        private IEnumerable<FacilityLink> GetFacilityLinks()
        {
            var model = LoadJson<IEnumerable<CensusFacilityLinkModel>>("FacilityLinks.json");
            return model.Select(a =>
            {
                return new FacilityLink
                {
                    Id = Guid.NewGuid().ToString(),
                    ZoneId = a.ZoneId,
                    FacilityIdA = a.FacilityIdA,
                    FacilityIdB = a.FacilityIdB
                };
            });
        }

        private IEnumerable<MapRegion> GetMapRegions()
        {
            var model = LoadJson<IEnumerable<CensusMapRegionModel>>("MapRegions.json");
            return model.Select(a =>
            {
                return new MapRegion
                {
                    Id = a.MapRegionId,
                    ZoneId = a.ZoneId,
                    FacilityId = a.FacilityId,
                    FacilityName = a.FacilityName,
                    FacilityType = a.FacilityType,
                    FacilityTypeId = a.FacilityTypeId,
                    XPos = a.LocationX,
                    YPos = a.LocationY,
                    ZPos = a.LocationZ
                };
            });
        }

        private IEnumerable<MapOwnership> GetMapOwnership()
        {
            var model = LoadJson<CensusMapModel>("MapOwnership.json");
            return model.Regions.Row.Select(a => new MapOwnership(a.RowData.RegionId, a.RowData.FactionId));
        }

        private T LoadJson<T>(string filename) where T: class
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
