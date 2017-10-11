namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusVehicleModel
    {
        public string VehicleId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int Cost { get; set; }
        public string CostResourceId { get; set; }
        public string ImageId { get; set; }
    }
}
