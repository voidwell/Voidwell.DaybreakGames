using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class CharacterDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BattleRank { get; set; }
        public int BattleRankPercentToNext { get; set; }
        public int CertsEarned { get; set; }
        public string Faction { get; set; }
        public int FactionId { get; set; }
        public int FactionImageId { get; set; }
        public string Title { get; set; }
        public int WorldId { get; set; }

        /*
        public CharacterLifetimeStats LifetimeStats { get; set; }
        public CharacterOutfit Outfit { get; set; }
        public IEnumerable<CharacterStat> ProfileStats { get; set; }
        public IEnumerable<CharacterStatByFaction> ProfileStatsByFaction { get; set; }
        public CharacterTimes Times { get; set; }
        public IEnumerable<CharacterVehicleStats> VehicleStats { get; set; }
        public IEnumerable<CharacterWeaponStats> WeaponStats { get; set; }
        */
    }
}
