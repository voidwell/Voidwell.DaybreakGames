using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusMapModel
    {
        [JsonPropertyName("ZoneId")]
        public int ZoneId { get; set; }
        [JsonPropertyName("Regions")]
        public CensusMapRegionSet Regions { get; set; }

        public class CensusMapRegionSet
        {
            [JsonPropertyName("IsList")]
            public bool IsList { get; set; }
            [JsonPropertyName("Row")]
            public IEnumerable<CensusMapRegionSetRow> Row { get; set; }
        }

        public class CensusMapRegionSetRow
        {
            [JsonPropertyName("RowData")]
            public CensusMapRegionSetRowData RowData { get; set; }
        }

        public class CensusMapRegionSetRowData
        {
            [JsonPropertyName("RegionId")]
            public int RegionId { get; set; }
            [JsonPropertyName("FactionId")]
            public int FactionId { get; set; }
        }
    }
}
