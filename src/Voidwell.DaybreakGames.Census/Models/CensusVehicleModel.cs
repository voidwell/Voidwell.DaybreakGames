﻿namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusVehicleModel
    {
        public int VehicleId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int Cost { get; set; }
        public int CostResourceId { get; set; }
        public int ImageId { get; set; }
    }
}
