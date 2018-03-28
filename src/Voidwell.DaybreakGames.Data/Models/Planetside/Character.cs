using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Character
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public int FactionId { get; set; }
        public int TitleId { get; set; }
        public int WorldId { get; set; }
        public int BattleRank { get; set; }
        public int BattleRankPercentToNext { get; set; }
        public int CertsEarned { get; set; }
        public int PrestigeLevel { get; set; }

        public Title Title { get; set; }
        public World World { get; set; }
        public Faction Faction { get; set; }

        public CharacterTime Time { get; set; }
        public OutfitMember OutfitMembership { get; set; }
        public IEnumerable<CharacterStat> Stats { get; set; }
        public CharacterLifetimeStat LifetimeStats { get; set; }
        public IEnumerable<CharacterStatByFaction> StatsByFaction { get; set; }
        public CharacterLifetimeStatByFaction LifetimeStatsByFaction { get; set; }
        public IEnumerable<CharacterWeaponStat> WeaponStats { get; set; }
        public IEnumerable<CharacterWeaponStatByFaction> WeaponStatsByFaction { get; set; }
    }
}
