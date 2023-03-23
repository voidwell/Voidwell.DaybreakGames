using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;

namespace Voidwell.DaybreakGames.Test.MapTests
{
    public static class MapHelper
    {
        private const string DataRoot = "MapTests/TestData/";
        private static readonly JsonSerializerOptions jsonSerializerOptions;

        static MapHelper()
        {
            jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new UnderscorePropertyJsonNamingPolicy(),
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Converters =
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
            };
        }

        public static WorldZoneState GetWorldZoneState(int zoneId)
        {
            var zoneMap = GetZoneMap(zoneId);
            var ownership = GetMapOwnership(zoneId);

            var zone = new Zone { Id = zoneId };

            return new WorldZoneState(1, zone.Id, zone.Name, zoneMap, ownership);
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
            return mapList.First(a => a.ZoneId == zoneId).Regions.Row.Select(a => new ZoneRegionOwnership(a.RowData.RegionId, a.RowData.FactionId));
        }

        public static IEnumerable<FacilityLink> GetFacilityLinks(int zoneId)
        {
            var model = LoadJson<IEnumerable<CensusFacilityLinkModel>>($"{DataRoot}FacilityLinks.json", "facility_link_list");
            return model.Select(a => new FacilityLink
            {
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
                return JsonSerializer.Deserialize<JsonElement>(r.ReadToEnd())
                    .GetProperty(rootProperty)
                    .Deserialize<T>(jsonSerializerOptions);
            }
        }
    }
}
