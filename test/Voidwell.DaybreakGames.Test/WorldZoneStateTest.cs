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
            var facilityLinks = GetFacilityLinks();
            var mapRegions = GetMapRegions();
            var ownership = GetMapOwnership();
            var zone = new Zone { Id = 2, Code = "Indar" };

            var worldZoneState = new WorldZoneState(17, zone, facilityLinks, mapRegions, ownership);

            var score = worldZoneState.MapScore;

            var percentScore = score.Percent.ToArray();
            var percentTotal = percentScore[0] + percentScore[1] + percentScore[2] + percentScore[3];

            var connectedPercentScore = score.ConnectedPercent.ToArray();
            var connectedPercentTotal = connectedPercentScore[0] + connectedPercentScore[1] + connectedPercentScore[2] + connectedPercentScore[3];

            Assert.AreEqual(1, percentTotal);
            Assert.AreEqual(1, connectedPercentTotal);
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
