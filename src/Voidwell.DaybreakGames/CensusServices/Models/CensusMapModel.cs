using System.Collections.Generic;

namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusMapModel
    {
        public string ZoneId { get; set; }
        public CensusMapRegionSet Regions { get; set; }

        public class CensusMapRegionSet
        {
            public bool IsList { get; set; }
            public IEnumerable<CensusMapRegionSetRow> Row { get; set; }
        }

        public class CensusMapRegionSetRow
        {
            public CensusMapRegionSetRowData RowData { get; set; }
        }

        public class CensusMapRegionSetRowData
        {
            public string RegionId { get; set; }
            public string FactionId { get; set; }
        }
    }
}
