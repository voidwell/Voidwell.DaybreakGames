namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusMapHexModel
    {
        public int ZoneId { get; set; }
        public int MapRegionId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int HexType { get; set; }
        public string TypeName { get; set; }
    }
}
