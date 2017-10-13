namespace Voidwell.DaybreakGames.Models
{
    public class MapOwnership
    {
        public MapOwnership(string regionId, string factionId)
        {
            RegionId = regionId;
            FactionId = factionId;
        }

        public string RegionId { get; set; }
        public string FactionId { get; set; }
    }
}
