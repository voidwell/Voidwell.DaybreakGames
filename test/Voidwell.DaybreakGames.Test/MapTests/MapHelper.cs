using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.Test.MapTests
{
    public static class MapHelper
    {
        private const string DataRoot = "MapTests/TestData/";
        private static readonly JsonSerializer jsonSerializer;

        static MapHelper()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
                Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
            };

            jsonSerializer = JsonSerializer.Create(settings);
        }

        public static WorldZoneState GetWorldZoneState(int zoneId)
        {
            var zoneMap = GetZoneMap(zoneId);
            var ownership = GetMapOwnership(zoneId);

            var zone = new Zone { Id = zoneId };

            return new WorldZoneState(1, zone, zoneMap, ownership);
        }

        public static ZoneMap GetZoneMap(int zoneId)
        {
            var links = GetFacilityLinks(zoneId).Select(a => new ZoneLink { FacilityIdA = a.FacilityIdA, FacilityIdB = a.FacilityIdB });
            var regions = GetMapRegions(zoneId).Select(a => new ZoneRegion
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

        public static IEnumerable<ZoneRegionOwnership> GetMapOwnership(int zoneId)
        {
            var mapList = LoadJson<IEnumerable<CensusMapModel>>($"{DataRoot}/MapOwnership.json", "map_list");
            return mapList.First(a => a.ZoneId == zoneId).Regions.Row.Select(a => new ZoneRegionOwnership(zoneId, a.RowData.RegionId, a.RowData.FactionId));
        }

        public static IEnumerable<FacilityLink> GetFacilityLinks(int zoneId)
        {
            var model = LoadJson<IEnumerable<CensusFacilityLinkModel>>($"{DataRoot}FacilityLinks.json", "facility_link_list");
            return model.Select(a => new FacilityLink
            {
                Id = Guid.NewGuid().ToString(),
                ZoneId = a.ZoneId,
                FacilityIdA = a.FacilityIdA,
                FacilityIdB = a.FacilityIdB
            }).Where(a => a.ZoneId == zoneId);
        }

        public static IEnumerable<MapRegion> GetMapRegions(int zoneId)
        {
            var model = LoadJson<IEnumerable<CensusMapRegionModel>>($"{DataRoot}MapRegions.json", "map_region_list");
            return model.Select(a => new MapRegion
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
            }).Where(a => a.ZoneId == zoneId);
        }

        private static T LoadJson<T>(string filename, string rootProperty) where T : class
        {
            using (var r = new StreamReader(filename))
            {
                return JToken.Parse(r.ReadToEnd())
                    .SelectToken(rootProperty)
                    .ToObject<T>(jsonSerializer);
            }
        }
    }
}
