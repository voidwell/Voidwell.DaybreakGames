namespace Voidwell.DaybreakGames.Models
{
    public class MapOwnership
    {
        public MapOwnership(int regionId, int factionId)
        {
            RegionId = regionId;
            FactionId = factionId;
        }

        public int RegionId { get; set; }
        public int FactionId { get; set; }
    }
}
