using Newtonsoft.Json;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusMapModel
    {
        [JsonProperty("ZoneId")]
        public int ZoneId { get; set; }
        [JsonProperty("Regions")]
        public CensusMapRegionSet Regions { get; set; }

        public class CensusMapRegionSet
        {
            [JsonProperty("IsList")]
            public bool IsList { get; set; }
            [JsonProperty("Row")]
            public IEnumerable<CensusMapRegionSetRow> Row { get; set; }
        }

        public class CensusMapRegionSetRow
        {
            [JsonProperty("RowData")]
            public CensusMapRegionSetRowData RowData { get; set; }
        }

        public class CensusMapRegionSetRowData
        {
            [JsonProperty("RegionId")]
            public int RegionId { get; set; }
            [JsonProperty("FactionId")]
            public int FactionId { get; set; }
        }
    }
}
